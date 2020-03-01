using System.Security.Claims;
using Api.TrueLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Api.Controllers
{
    [Route("v1/providers")]
    [Controller]
    public class ProvidersController : ControllerBase
    {
        private readonly TrueLayerSettings _settings;

        public ProvidersController(IOptions<TrueLayerSettings> settingsOptions)
        {
            _settings = settingsOptions.Value;
        }

        [HttpPost("{id}/connect")]
        [Authorize]
        public IActionResult Connect(string id)
        {
            // get system user's id
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Response.Headers[HeaderNames.Location] = _settings.AuthApiUrl + "/" +
                                                     "?response_type=code" +
                                                     "&response_mode=form_post" +
                                                     "&client_id=" + _settings.ClientId +
                                                     "&scope=offline_access transactions accounts" +
                                                     "&redirect_uri=" + _settings.RedirectPage +
                                                     "&provider_id=" + id +
                                                     "&providers=" + _settings.Providers +
                                                     "&state=" + userId;
            // return 301 Redirect to TrueLayer with the user's id in state query string
            return StatusCode(StatusCodes.Status303SeeOther);
        }
    }
}