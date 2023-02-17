using System.Collections.Immutable;
using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace OAuth.AuthorizationServer.Pages;

[Authorize]
public class Consent : PageModel
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly AuthorizationService _authService;
        
    public Consent(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        AuthorizationService authService)
    {
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
        _scopeManager = scopeManager;
        _authService = authService;
    }
    
    public string? ReturnUrl { get; set; }

    public IDictionary<string, StringValues> OAuthParameters { get; set; } = new Dictionary<string, StringValues>();
    
    public void OnGet()
    {
        var req = HttpContext.GetOpenIddictServerRequest(); 
        OAuthParameters = HttpContext.ParseOAuthParameters();
    }

    public async Task<IActionResult> OnPostGrantAsync()
    {
        var context = HttpContext;
        var req = HttpContext.GetOpenIddictServerRequest(); 
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
        
        var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                          throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

        var userId = User.FindFirst(ClaimTypes.Email)!.Value;
        
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        identity.SetClaim(OpenIddictConstants.Claims.Subject, userId)
            .SetClaim(OpenIddictConstants.Claims.Email, userId)
            .SetClaim(OpenIddictConstants.Claims.Name, userId)
            .SetClaims(OpenIddictConstants.Claims.Role, new ImmutableArray<string> { "user", "admin" });

        identity.SetScopes(request.GetScopes());
        identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

        var authorization = await _authorizationManager.CreateAsync(
            identity: identity,
            subject : userId,
            client  : await _applicationManager.GetIdAsync(application),
            type    : OpenIddictConstants.AuthorizationTypes.Permanent,
            scopes  : identity.GetScopes());

        identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));
        identity.SetDestinations(AuthorizationService.GetDestinations);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    public IActionResult OnPostDeny()
    {   
        return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}