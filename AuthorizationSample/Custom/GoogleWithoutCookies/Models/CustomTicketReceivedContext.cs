using Microsoft.AspNetCore.Authentication;

namespace GoogleWithoutCookies.Models
{
    public class CustomTicketReceivedContext : CustomRemoteAuthenticationContext<CustomRemoteAuthenticationOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TicketReceivedContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The <see cref="AuthenticationScheme"/>.</param>
        /// <param name="options">The <see cref="RemoteAuthenticationOptions"/>.</param>
        /// <param name="ticket">The received ticket.</param>
        public CustomTicketReceivedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            CustomRemoteAuthenticationOptions options,
            AuthenticationTicket ticket)
            : base(context, scheme, options, ticket?.Properties)
            => Principal = ticket?.Principal;

        /// <summary>
        /// Gets or sets the URL to redirect to after signin.
        /// </summary>
        public string? ReturnUri { get; set; }
    }
}
