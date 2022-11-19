using Microsoft.AspNetCore.Authentication;

namespace GoogleWithoutCookies.Models
{
    public class CustomPropertiesContext<TOptions> : CustomBaseContext<TOptions> where TOptions : CustomAuthenticationSchemeOptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="scheme">The authentication scheme.</param>
        /// <param name="options">The authentication options associated with the scheme.</param>
        /// <param name="properties">The authentication properties.</param>
        protected CustomPropertiesContext(HttpContext context, AuthenticationScheme scheme, TOptions options, AuthenticationProperties? properties)
            : base(context, scheme, options)
        {
            Properties = properties ?? new AuthenticationProperties();
        }

        /// <summary>
        /// Gets or sets the <see cref="AuthenticationProperties"/>.
        /// </summary>
        public virtual AuthenticationProperties Properties { get; protected set; }
    }
}
