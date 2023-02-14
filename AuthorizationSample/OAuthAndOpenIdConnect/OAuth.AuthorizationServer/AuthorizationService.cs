using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using OpenIddict.Abstractions;

namespace OAuth.AuthorizationServer;

public class AuthorizationService
{
    public bool IsAuthenticated(AuthenticateResult authenticateResult, OpenIddictRequest request)
    {
        if (!authenticateResult.Succeeded)
        {
            return false;
        }
        
        if (request.MaxAge.HasValue && authenticateResult.Properties != null)
        {
            var maxAgeSeconds = TimeSpan.FromSeconds(request.MaxAge.Value);

            var expired = !authenticateResult.Properties.IssuedUtc.HasValue ||
                          DateTimeOffset.UtcNow - authenticateResult.Properties.IssuedUtc > maxAgeSeconds;
            if (expired)
            {
                return false;
            }
        }

        return true;
    }
    
    public static List<string> GetDestinations(Claim claim)
    {
        var destinations = new List<string>();
        if (claim.Type is OpenIddictConstants.Claims.Name or OpenIddictConstants.Claims.Email)
        {
            destinations.Add(OpenIddictConstants.Destinations.AccessToken);
        }

        return destinations;
    }
}