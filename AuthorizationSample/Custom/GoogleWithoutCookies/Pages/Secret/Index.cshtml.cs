using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;

namespace GoogleWithoutCookies.Pages.Secret
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public async void OnGet()
        {
            var principal = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        }
    }
}
