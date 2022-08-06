using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuth.Server.Controllers
{
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        [HttpPost("authorization/token")]
        [Authorize]
        public IActionResult GetTokenAsync([FromBody] GetTokenRequest request)
        {
            if (request.Login != "andreyka26_" || request.Password != "mypass1")
            {
                // or 401 or 404
                return Unauthorized();
            }


        }
    }
}
