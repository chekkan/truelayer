using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.TrueLayer
{
    [Controller]
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

        [Route("/callback")]
        [HttpPost]
        public async Task<IActionResult> Callback([FromForm] CallbackBodyDto body)
        {
            await _userService.UpdateAuthCode(new Guid(body.State), body.Code, body.Scope);
            return StatusCode(StatusCodes.Status201Created,
                new {message = "Saved the `code` and `scope` successfully."});
        }
    }

    public interface IUserService
    {
        Task UpdateAuthCode(Guid userId, string code, string scope);
        Task<User> Authenticate(string username, string password);
    }

    public interface ITrueLayerDataApiClient
    {
        Task<AccessTokenResponse> GetAccessToken(string code);
    }

    public class TrueLayerDataApiHttpClient : ITrueLayerDataApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly TrueLayerSettings _settings;

        public TrueLayerDataApiHttpClient(IOptions<TrueLayerSettings> settingsOptions, HttpClient httpClient)
        {
            _settings = settingsOptions.Value;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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
    }

    public static class StreamExtension
    {
        public static ValueTask<T> ReadAsJson<T>(this Stream stream)
        {
            return JsonSerializer.DeserializeAsync<T>(stream);
        }
    }

    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
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
        // State contains the user id the code is for
        public string State { get; set; }
    }
}