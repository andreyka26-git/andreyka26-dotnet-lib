using Digest.Server.Application;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Digest.Server.Infrastructure;

internal class DigestAuthenticationHandler : AuthenticationHandler<DigestAuthenticationOptions>
{
    private readonly IUsernameHashedSecretProvider _usernameHashedSecretProvider;
    private readonly IHashService _hashService;
    private readonly HeaderService _headerService;

    private DigestAuthImplementation _digestAuth;

    public DigestAuthenticationHandler(IOptionsMonitor<DigestAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUsernameHashedSecretProvider usernameHashedSecretProvider,
        IHashService hashService,
        HeaderService headerService)
        : base(options, logger, encoder, clock)
    {
        _usernameHashedSecretProvider = usernameHashedSecretProvider;
        _hashService = hashService;
        _headerService = headerService;
    }

    protected override async Task InitializeHandlerAsync()
    {
        await base.InitializeHandlerAsync();

        if (_usernameHashedSecretProvider != null)
        {
            _digestAuth = new DigestAuthImplementation(Options.Configuration, _usernameHashedSecretProvider, _hashService, _headerService);
        }
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(DigestAuthImplementation.AuthorizationHeaderName, out var headerValue))
        {
            return AuthenticateResult.NoResult();
        }

        if (!DigestChallengeResponse.TryParse(headerValue, out var challengeResponse))
        {
            return AuthenticateResult.NoResult();
        }

        string validatedUsername = await _digestAuth.ValidateChallangeAsync(challengeResponse, Request.Method);

        if (string.IsNullOrEmpty(validatedUsername))
        {
            return AuthenticateResult.NoResult();
        }

        var identity = new ClaimsIdentity(Scheme.Name);
        identity.AddClaim(new Claim(DigestAuthImplementation.DigestAuthenticationClaimName, validatedUsername));
        var principal = new ClaimsPrincipal(identity);

        if (_digestAuth.UseAuthenticationInfoHeader)
        {
            Response.Headers[DigestAuthImplementation.AuthenticationInfoHeaderName] = await _digestAuth.BuildAuthInfoHeaderAsync(challengeResponse);
        }

        return AuthenticateResult.Success(new AuthenticationTicket(principal, new AuthenticationProperties(), Scheme.Name));
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        await base.HandleChallengeAsync(properties);

        if (Response.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
            Response.Headers[DigestAuthImplementation.AuthenticateHeaderName] = _digestAuth.BuildChallengeHeader();
        }
    }
}