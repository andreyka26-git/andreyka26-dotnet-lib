using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OAuth.AuthorizationServer.Pages
{
    public class AuthenticateModel : PageModel
    {
        public string Email { get; set; } = Consts.Email;
        public string Password { get; set; } = Consts.Password;

        public string? ReturnUrl { get; set; }
        public string AuthStatus { get; set; } = "";

        public void OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }
        
        public async Task<IActionResult> OnPostAsync(string email, string password)
        {
            if (email != Consts.Email || password != Consts.Password)
            {
                AuthStatus = "Email or password is invalid";
                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, email),
            };

            var principal = new ClaimsPrincipal(
                new List<ClaimsIdentity> 
                {
                    new(claims)
                });

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }

            AuthStatus = "Successfully authenticated";
            return Page();
        }
    }
}