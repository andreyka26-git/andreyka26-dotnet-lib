using Microsoft.AspNetCore.Authentication;

namespace Digest.Server.Application;

internal class DigestAuthenticationOptions : AuthenticationSchemeOptions
{
    public DigestAuthenticationConfiguration Configuration { get; set; }
}
