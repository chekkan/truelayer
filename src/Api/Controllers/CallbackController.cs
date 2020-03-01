using System;
using System.Threading.Tasks;
using Api.Application;
using Api.TrueLayer;
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
}