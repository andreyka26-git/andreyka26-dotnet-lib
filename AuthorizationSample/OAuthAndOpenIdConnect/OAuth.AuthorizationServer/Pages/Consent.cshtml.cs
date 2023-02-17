using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace OAuth.AuthorizationServer.Pages;

[Authorize]
public class Consent : PageModel
{
    [BindProperty]
    public string? ReturnUrl { get; set; }
    
    public IActionResult OnGet(string returnUrl)
    {
        ReturnUrl = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string grant)
    {
        if (grant == "Grant")
        {
            var consentClaim = User.GetClaim("consent");

            if (string.IsNullOrEmpty(consentClaim))
            {
                User.SetClaim("consent", "Grant");
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, User);
            }

            return Redirect(ReturnUrl);
        }

        return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}