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
            var accessToken = await GetAccessTokenForUser(userId);

            var uri = $"{_settings.DataApiUrl}/accounts/{accountId}/transactions";
            uri += $"?from={@from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";
            var reqMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            reqMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(reqMessage, HttpCompletionOption.ResponseHeadersRead);
            var stream = await response.Content.ReadAsStreamAsync();
            var content = await stream.ReadAsJson<TrueLayerResponse<TrueLayerTransaction>>();
            return new ResourceCollection<Transaction>(content.Results.Select(ToTransaction), content.Results.Count());
        }

        private async Task<string> GetAccessTokenForUser(Guid userId)
        {
            var currCredential = await _userService.GetAuthCredentials(userId);

            // is current access token expires with a minute
            // request new access token
            if (currCredential.ExpiresAt >= DateTime.Now.AddMinutes(1)) return currCredential.AccessToken;

            currCredential = await RefreshToken(currCredential);
            await _userService.SaveCredential(userId, currCredential);

            return currCredential.AccessToken;
        }

        private async Task<AuthCredential> RefreshToken(AuthCredential credential)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", _settings.ClientId),
                new KeyValuePair<string, string>("client_secret", _settings.ClientSecret),
                new KeyValuePair<string, string>("refresh_token", credential.RefreshToken)
            });
            var response = await _httpClient.PostAsync(_settings.AuthApiUrl + "/connect/token", formContent);
            var stream = await response.Content.ReadAsStreamAsync();
            var accessToken = await stream.ReadAsJson<AccessTokenResponse>();
            return new AuthCredential
            {
                AccessToken = accessToken.AccessToken,
                ExpiresAt = DateTime.Now.AddSeconds(accessToken.ExpiresIn),
                RefreshToken = accessToken.RefreshToken
            };
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

        public async Task<IDictionary<string, decimal>> GetSummary(
            Guid userId, 
            DateTimeOffset from,
            DateTimeOffset to)
        {
            var transactions = await ((ITransactionReader) this).GetAll(userId, from, to);
            var result = transactions.Items
                .Where(t => t.Type == "DEBIT")
                .GroupBy(t => t.Category, CategorySum);
            return new Dictionary<string, decimal>(result);
        }

        private static KeyValuePair<string, decimal> CategorySum(string category, IEnumerable<Transaction> items)
        {
            return new KeyValuePair<string, decimal>(category, items.Sum(x => x.Amount));
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
            var accessToken = await GetAccessTokenForUser(userId);

            var uri = _settings.DataApiUrl + "/accounts";
            var reqMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            reqMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

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
}