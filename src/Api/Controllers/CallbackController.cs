using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
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
            var accessToken = await _trueLayerDataApiClient.GetAccessToken(body.Code);
            await _userService.UpdateAccessToken(new Guid(body.State), accessToken);
            return StatusCode(StatusCodes.Status201Created,
                new {message = "Saved the `access_token` successfully."});
        }
    }

    public interface IUserService
    {
        Task UpdateAccessToken(Guid userId, AccessTokenResponse accessToken);
        Task<User> Authenticate(string username, string password);
        Task<User> GetById(Guid userId);
    }

    public interface ITrueLayerDataApiClient
    {
        Task<AccessTokenResponse> GetAccessToken(string code);
    }

    public static class StreamExtension
    {
        public static ValueTask<T> ReadAsJson<T>(this Stream stream)
        {
            return JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy() 
            });
        }
    }

    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToSnakeCase();
        }
    }

    public static class StringUtils
    {
        public static string ToSnakeCase(this string str)
        {
            return string.Concat(
                str.Select(
                    (x, i) => i > 0 && char.IsUpper(x)
                        ? "_" + x
                        : x.ToString()
                )
            ).ToLower();
        }
    }

    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
    }

    public class AccessTokenResponse
    {
        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }

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