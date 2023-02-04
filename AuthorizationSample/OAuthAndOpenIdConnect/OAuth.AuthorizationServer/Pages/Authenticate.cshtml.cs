using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OAuth.AuthorizationServer.Pages
{
    public class AuthenticateModel : PageModel
    {
        public void OnGet()
        {
        }

        public string Email { get; set; } = "andriibui@gmail.com";
        public string Password { get; set; } = "password";

        public void OnPost(string email, string password)
        {
        }
    }
}