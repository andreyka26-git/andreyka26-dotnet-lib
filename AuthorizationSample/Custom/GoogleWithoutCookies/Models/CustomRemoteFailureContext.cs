﻿using Microsoft.AspNetCore.Authentication;

namespace GoogleWithoutCookies.Models
{
    public class CustomRemoteFailureContext : CustomHandleRequestContext<CustomRemoteAuthenticationOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RemoteFailureContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The <see cref="AuthenticationScheme"/>.</param>
        /// <param name="options">The <see cref="RemoteAuthenticationOptions"/>.</param>
        /// <param name="failure">User friendly error message for the error.</param>
        public CustomRemoteFailureContext(
            HttpContext context,
            AuthenticationScheme scheme,
            CustomRemoteAuthenticationOptions options,
            Exception failure)
            : base(context, scheme, options)
        {
            Failure = failure;
        }

        /// <summary>
        /// User friendly error message for the error.
        /// </summary>
        public Exception? Failure { get; set; }

        /// <summary>
        /// Additional state values for the authentication session.
        /// </summary>
        public AuthenticationProperties? Properties { get; set; }
    }
}
