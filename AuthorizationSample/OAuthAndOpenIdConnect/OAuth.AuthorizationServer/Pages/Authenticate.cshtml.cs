using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OAuth.AuthorizationServer.Pages
{
    public class AuthenticateModel : PageModel
    {
        public void OnGet()
        {
        }

        public string Email { get; set; } = Consts.Email;
        public string Password { get; set; } = Consts.Password;

        public void OnPost(string email, string password)
        {
        }
    }
}