using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OAuth.OpenIddict.AuthorizationServer;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace OAuth.OpenIddict.AuthorizationServer.Pages;

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
        if (grant == Consts.GrantAccessValue)
        {
            var consentClaim = User.GetClaim(Consts.ConsentNaming);

            if (string.IsNullOrEmpty(consentClaim))
            {
                User.SetClaim(Consts.ConsentNaming, Consts.GrantAccessValue);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, User);
            }

            return Redirect(ReturnUrl);
        }

        return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}