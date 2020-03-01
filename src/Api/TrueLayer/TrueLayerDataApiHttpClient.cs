using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Api.Controllers;
using Api.Models;
using Microsoft.Extensions.Options;

namespace Api.TrueLayer
{
    public class TrueLayerDataApiHttpClient : ITrueLayerDataApiClient, ITransactionReader, IAccountReader
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;
        private readonly TrueLayerSettings _settings;

        public TrueLayerDataApiHttpClient(IOptions<TrueLayerSettings> settingsOptions, HttpClient httpClient,
            IUserService userService)
        {
            _settings = settingsOptions.Value;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<AccessTokenResponse> GetAccessToken(string code)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", _settings.ClientId),
                new KeyValuePair<string, string>("client_secret", _settings.ClientSecret),
                new KeyValuePair<string, string>("redirect_uri", _settings.RedirectPage),
                new KeyValuePair<string, string>("code", code),
            });
            var response = await _httpClient.PostAsync(_settings.AuthApiUrl + "/connect/token", formContent);
            var stream = await response.Content.ReadAsStreamAsync();
            return await stream.ReadAsJson<AccessTokenResponse>();
        }

        public async Task<ResourceCollection<Transaction>> GetAll(
            Guid userId, 
            string accountId, 
            DateTimeOffset from, 
            DateTimeOffset to)
        {
            var user = await _userService.GetById(userId);

            var uri = _settings.DataApiUrl + "/accounts/" + accountId + "/transactions";
            uri += $"?from=${@from:yyyy-MM-dd}&to=${to:yyyy-MM-dd}";
            var reqMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            reqMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);

            var response = await _httpClient.SendAsync(reqMessage, HttpCompletionOption.ResponseHeadersRead);
            var stream = await response.Content.ReadAsStreamAsync();
            var content = await stream.ReadAsJson<TrueLayerResponse<TrueLayerTransaction>>();
            return new ResourceCollection<Transaction>(content.Results.Select(ToTransaction), content.Results.Count());
        }

        async Task<ResourceCollection<Transaction>> ITransactionReader.GetAll(
            Guid userId, 
            DateTimeOffset from, 
            DateTimeOffset to)
        {
            var transactionCollections = new List<ResourceCollection<Transaction>>();
            var accounts = await ((IAccountReader) this).GetAll(userId);
            foreach (var account in accounts.Items)
            {
                var accountTransactions = await GetAll(userId, account.Id, from, to);
                transactionCollections.Add(accountTransactions);
            }

            return transactionCollections.Aggregate(ResourceCollection<Transaction>.Empty(), (acc, curr) =>
            {
                var items = new List<Transaction>();
                items.AddRange(acc.Items);
                items.AddRange(curr.Items);
                return new ResourceCollection<Transaction>(items, acc.Count + curr.Count);
            });
        }

        private static Transaction ToTransaction(TrueLayerTransaction arg)
        {
            return new Transaction
            {
                Id = arg.TransactionId,
                Timestamp = arg.Timestamp,
                Description = arg.Description,
                Amount = arg.Amount,
                Currency = arg.Currency,
                Type = arg.TransactionType,
                Category = arg.TransactionCategory,
                MerchantName = arg.MerchantName,
                Classifications = arg.TransactionClassification
            };
        }

        public async Task<ResourceCollection<Account>> GetAll(Guid userId)
        {
            var user = await _userService.GetById(userId);

            var uri = _settings.DataApiUrl + "/accounts";
            var reqMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            reqMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);

            var response = await _httpClient.SendAsync(reqMessage, HttpCompletionOption.ResponseHeadersRead);
            var stream = await response.Content.ReadAsStreamAsync();
            var content = await stream.ReadAsJson<TrueLayerResponse<TrueLayerAccount>>();
            return new ResourceCollection<Account>(content.Results.Select(ToAccount), content.Results.Count());
        }

        private static Account ToAccount(TrueLayerAccount arg)
        {
            return new Account
            {
                Id = arg.AccountId,
                Name = arg.DisplayName,
                ProviderId = arg.Provider.ProviderId,
                ProviderName = arg.Provider.DisplayName,
                Type = arg.AccountType
            };
        }
    }

    public class TrueLayerAccount
    {
        public string AccountId { get; set; }

        public string AccountType { get; set; }

        public string DisplayName { get; set; }

        public string Currency { get; set; }

        public AccountProvider Provider { get; set; }

        public class AccountProvider
        {
            public string DisplayName { get; set; }
            public string ProviderId { get; set; }
        }
    }

    public class TrueLayerTransaction
    {
        public string TransactionId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string TransactionType { get; set; }
        public string TransactionCategory { get; set; }
        public string MerchantName { get; set; }
        public IEnumerable<string> TransactionClassification { get; set; }
    }

    public class TrueLayerResponse<T>
    {
        public IEnumerable<T> Results { get; set; }
    }
}