using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace GoogleWithoutCookies.Models
{
    public class EnsureSignInScheme<TOptions> : IPostConfigureOptions<TOptions> where TOptions : CustomRemoteAuthenticationOptions
    {
        private readonly AuthenticationOptions _authOptions;

        public EnsureSignInScheme(IOptions<AuthenticationOptions> authOptions)
        {
            _authOptions = authOptions.Value;
        }

        public void PostConfigure(string? name, TOptions options)
        {
            options.SignInScheme ??= _authOptions.DefaultSignInScheme ?? _authOptions.DefaultScheme;
        }
    }
}
