using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Oidc.AuthorizationServerWithOpenIddict.Areas.OAuth.Pages
{
    public class ConsentModel : PageModel
    {
        public string ApplicationName { get; set; }

        public string Scope { get; set; }

        public void OnGet(string applicationName, string scope)
        {
            ApplicationName = applicationName;
            Scope = scope;
        }
    }
}
