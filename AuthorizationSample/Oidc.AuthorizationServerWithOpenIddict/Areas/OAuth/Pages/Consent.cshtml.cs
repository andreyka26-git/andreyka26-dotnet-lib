using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace Oidc.AuthorizationServerWithOpenIddict.Areas.OAuth.Pages
{
    //TODO think about the way to improve this.
    public class ConsentModel : PageModel
    {
        public string ApplicationName { get; set; }

        public string Scope { get; set; }

        public Dictionary<string, string> FormParameters { get; set; }

        public void OnGet(string applicationName,
            string scope,
            string formParams)
        {
            ApplicationName = applicationName;
            Scope = scope;
            FormParameters = new();

            var values = formParams.Split("&");

            foreach (var value in values.Where(v => !string.IsNullOrEmpty(v)))
            {
                var keyValue = value.Split('=');
                FormParameters.Add(keyValue[0], keyValue[1]);
            }
        }
    }
}
