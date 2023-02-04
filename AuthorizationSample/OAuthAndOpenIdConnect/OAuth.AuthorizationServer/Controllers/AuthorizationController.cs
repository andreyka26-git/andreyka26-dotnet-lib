using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;

namespace OAuth.AuthorizationServer.Controllers
{
    [ApiController]
    [Route("backend")]
    public class AuthorizationController : Controller
    {
        [HttpGet("html")]
        public async Task<IActionResult> GetAuthenticateHtml()
        {
            var html = 
                $"<html>" +
                $"<form action=\"/backend/authenticate\" method=\"post\">" +
                    $"<input name=\"email\" value=\"andriibui@gmail.com\"/>" +
                    $"<input name=\"password\" value=\"password\" />" +
                    $"<input type=\"submit\" />" +
                $"</form>" +
                $"</html>";

            return Content(html, MediaTypeNames.Text.Html, Encoding.UTF8);
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromForm] IFormCollection formCollection)
        {
            var email = formCollection["email"];
            var password = formCollection["password"];



            return Ok();
        }
    }
}
