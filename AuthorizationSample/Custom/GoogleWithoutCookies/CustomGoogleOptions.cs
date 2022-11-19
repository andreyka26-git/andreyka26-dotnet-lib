using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

namespace GoogleWithoutCookies
{
    public class CustomGoogleOptions : CustomOAuthOptions
    {
        public CustomGoogleOptions()
        {
            CallbackPath = new PathString("/signin-google");
            AuthorizationEndpoint = GoogleDefaults.AuthorizationEndpoint;
            TokenEndpoint = GoogleDefaults.TokenEndpoint;
            UserInformationEndpoint = GoogleDefaults.UserInformationEndpoint;
            Scope.Add("openid");
            Scope.Add("profile");
            Scope.Add("email");

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id"); // v2
            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub"); // v3
            ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
            ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
            ClaimActions.MapJsonKey("urn:google:profile", "link");
            ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        }

        public string? AccessType { get; set; }
    }
}

    

    
