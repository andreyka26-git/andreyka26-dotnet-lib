using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace GoogleWithoutCookies
{
    public class CustomGoogleHandler : CustomOAuthHandler<CustomGoogleOptions>
    {
        public CustomGoogleHandler(IOptionsMonitor<CustomGoogleOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }
    }
}
