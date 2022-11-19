using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace GoogleWithoutCookies.Models
{
    public class CustomOAuthPostConfigureOptions<TOptions, THandler> : IPostConfigureOptions<TOptions>
    where TOptions : CustomOAuthOptions, new()
    where THandler : CustomOAuthHandler<TOptions>
    {
        private readonly IDataProtectionProvider _dp;

        /// <summary>
        /// Initializes the <see cref="OAuthPostConfigureOptions{TOptions, THandler}"/>.
        /// </summary>
        /// <param name="dataProtection">The <see cref="IDataProtectionProvider"/>.</param>
        public CustomOAuthPostConfigureOptions(IDataProtectionProvider dataProtection)
        {
            _dp = dataProtection;
        }

        /// <inheritdoc />
        public void PostConfigure(string? name, TOptions options)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            options.DataProtectionProvider = options.DataProtectionProvider ?? _dp;
            if (options.Backchannel == null)
            {
                options.Backchannel = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler());
                options.Backchannel.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft ASP.NET Core OAuth handler");
                options.Backchannel.Timeout = options.BackchannelTimeout;
                options.Backchannel.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB
            }

            if (options.StateDataFormat == null)
            {
                var dataProtector = options.DataProtectionProvider.CreateProtector(
                    typeof(THandler).FullName!, name, "v1");
                options.StateDataFormat = new PropertiesDataFormat(dataProtector);
            }
        }
    }
}
