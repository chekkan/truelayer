using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers
{
    public class CallbackController : ControllerBase
    {
        private readonly ITrueLayerDataApiClient _trueLayerDataApiClient;
        private readonly IUserService _userService;

        public CallbackController(ITrueLayerDataApiClient trueLayerDataApiClient, IUserService userService)
        {
            _trueLayerDataApiClient =
                trueLayerDataApiClient ?? throw new ArgumentNullException(nameof(trueLayerDataApiClient));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost("/callback")]
        public async Task<IActionResult> Callback([FromForm] CallbackBodyDto body)
        {
            var user = await _trueLayerDataApiClient.GetUserInfo(body.Code, body.Scope);
            await _userService.UpdateAuthCode(user, body.Code, body.Scope);
            return StatusCode(StatusCodes.Status201Created,
                new {message = "Saved the `code` and `scope` successfully."});
        }
    }

    public interface IUserService
    {
        Task UpdateAuthCode(User user, string code, string scope);
    }

    public interface ITrueLayerDataApiClient
    {
        Task<User> GetUserInfo(string code, string bodyScope);
    }

    public class TrueLayerDataApiHttpClient : ITrueLayerDataApiClient
    {
        private readonly HttpClient _httpClient;
        private TrueLayerSettings _settings;

        public TrueLayerDataApiHttpClient(IOptions<TrueLayerSettings> settingsOptions, HttpClient httpClient)
        {
            _settings = settingsOptions.Value;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<User> GetUserInfo(string code, string bodyScope)
        {
            // get access token
            var accessToken = await GetAccessToken(code);

            var reqMessage = new HttpRequestMessage(HttpMethod.Get, "/info");
            reqMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);
            var response = await _httpClient.SendAsync(reqMessage, HttpCompletionOption.ResponseHeadersRead);
            var stream = await response.Content.ReadAsStreamAsync();
            var userInfos = await stream.ReadAsJson<TrueLayerResponse<TrueLayerUser>>();
            return userInfos.Results.Select(x => x.ToUser()).First();
        }

        private async Task<AccessTokenResponse> GetAccessToken(string code)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", _settings.ClientId),
                new KeyValuePair<string, string>("client_secret", _settings.ClientSecret),
                new KeyValuePair<string, string>("redirect_uri", "https://console.truelayer.com/redirect-page"),
                new KeyValuePair<string, string>("code", code),
            });
            var response = await _httpClient.PostAsync("https://auth.truelayer.com/connect/token", formContent);
            var stream = await response.Content.ReadAsStreamAsync();
            return await stream.ReadAsJson<AccessTokenResponse>();
        }
    }

    public class TrueLayerSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public static class StreamExtension
    {
        public static ValueTask<T> ReadAsJson<T>(this Stream stream)
        {
            return JsonSerializer.DeserializeAsync<T>(stream);
        }
    }

    public class TrueLayerUser
    {
        public string FullName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public IEnumerable<string> Emails { get; set; }

        public User ToUser()
        {
            return new User
            {
                FullName = FullName,
                DateOfBirth = DateOfBirth,
                Emails = Emails
            };
        }
    }

    public class TrueLayerResponse<T>
    {
        public IEnumerable<T> Results { get; set; }
    }

    public class User
    {
        public string FullName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public IEnumerable<string> Emails { get; set; }
    }

    public class AccessTokenResponse
    {
        public string AccessToken { get; set; }

        public string ExpiresIn { get; set; }

        public string TokenType { get; set; }

        public string RefreshToken { get; set; }
    }

    public class CallbackBodyDto
    {
        public string Code { get; set; }
        public string Scope { get; set; }
    }

    public class UserService : IUserService
    {
        public Task UpdateAuthCode(User user, string code, string scope)
        {
            throw new NotImplementedException();
        }
    }
}