using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace OpenIdConnect.OpenIddict.ResourceServer2.Controllers;

[Route("api")]
public class ResourceApi : Controller
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("message")]
    public async Task<IActionResult> GetMessage()
    {
        return Ok("response from SECOND resource server.");
    }
}
