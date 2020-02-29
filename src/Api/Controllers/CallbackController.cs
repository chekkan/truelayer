using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class CallbackController : ControllerBase
    {
        [HttpPost("/callback")]
        public IActionResult Callback([FromForm] CallbackBodyDto body)
        {
            return StatusCode(StatusCodes.Status201Created,
                new {message = "Saved the `code` and `scope` successfully."});
        }
    }

    public class CallbackBodyDto
    {
        public string Code { get; set; }
        public string Scope { get; set; }
    }
}