using Microsoft.AspNetCore.Authentication;

namespace GoogleWithoutCookies.Models
{
    public class CustomAccessDeniedContext : CustomHandleRequestContext<CustomRemoteAuthenticationOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AccessDeniedContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The <see cref="AuthenticationScheme"/>.</param>
        /// <param name="options">The <see cref="RemoteAuthenticationOptions"/>.</param>
        public CustomAccessDeniedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            CustomRemoteAuthenticationOptions options)
            : base(context, scheme, options)
        {
        }

        /// <summary>
        /// Gets or sets the endpoint path the user agent will be redirected to.
        /// By default, this property is set to <see cref="RemoteAuthenticationOptions.AccessDeniedPath"/>.
        /// </summary>
        public PathString AccessDeniedPath { get; set; }

        /// <summary>
        /// Additional state values for the authentication session.
        /// </summary>
        public AuthenticationProperties? Properties { get; set; }

        /// <summary>
        /// Gets or sets the return URL that will be flowed up to the access denied page.
        /// If <see cref="ReturnUrlParameter"/> is not set, this property is not used.
        /// </summary>
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets the parameter name that will be used to flow the return URL.
        /// By default, this property is set to <see cref="RemoteAuthenticationOptions.ReturnUrlParameter"/>.
        /// </summary>
        public string ReturnUrlParameter { get; set; } = default!;
    }
}
