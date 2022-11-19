using Microsoft.AspNetCore.Authentication;

namespace GoogleWithoutCookies.Models
{
    public class CustomRedirectContext<TOptions> : CustomPropertiesContext<TOptions> where TOptions : CustomOAuthOptions
    {
        /// <summary>
        /// Creates a new context object.
        /// </summary>
        /// <param name="context">The HTTP request context</param>
        /// <param name="scheme">The scheme data</param>
        /// <param name="options">The handler options</param>
        /// <param name="redirectUri">The initial redirect URI</param>
        /// <param name="properties">The <see cref="AuthenticationProperties"/>.</param>
        public CustomRedirectContext(
            HttpContext context,
            AuthenticationScheme scheme,
            TOptions options,
            AuthenticationProperties properties,
            string redirectUri)
            : base(context, scheme, options, properties)
        {
            Properties = properties;
            RedirectUri = redirectUri;
        }

        /// <summary>
        /// Gets or Sets the URI used for the redirect operation.
        /// </summary>
        public string RedirectUri { get; set; }
    }
}
