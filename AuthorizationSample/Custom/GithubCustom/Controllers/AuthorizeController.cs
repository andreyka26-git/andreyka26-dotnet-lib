using Microsoft.AspNetCore.Mvc;

namespace GithubCustom.Controllers
{
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly AuthorizeService _authorizeService;

        public AuthorizeController(AuthorizeService authorizeService)
        {
            _authorizeService = authorizeService;
        }

        [HttpGet("/signin-github")]
        public async Task<IActionResult> HandleGetCallbackAsync([FromQuery] string code, [FromQuery] string state)
        {
            var callback = new CallbackResponse
            {
                AuthCode = code,
                State = state
            };

            var token = await _authorizeService.GetAuthTokenAsync(callback);

            //do whatever needed with token

            // this one is triggered
            return Ok();
        }

        [HttpPost("/signin-github")]
        public async Task<IActionResult> HandlePostCallbackAsync([FromQuery] string code, [FromQuery] string state)
        {
            var callback = new CallbackResponse
            {
                AuthCode = code,
                State = state
            };

            var token = await _authorizeService.GetAuthTokenAsync(callback);

            //do whatever needed with token
            return Ok();
        }
    }
}
