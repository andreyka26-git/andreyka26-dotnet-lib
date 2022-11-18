using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GoogleWithoutCookies.Pages.Authentication
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPost()
        {
            // using Microsoft.AspNetCore.Authentication;
            await HttpContext.SignOutAsync();
            return RedirectToPage();
        }
    }
}
