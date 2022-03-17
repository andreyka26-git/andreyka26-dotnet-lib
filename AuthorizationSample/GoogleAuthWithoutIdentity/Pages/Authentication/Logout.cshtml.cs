using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GoogleAuthWithoutIdentity.Pages.Authentication
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
