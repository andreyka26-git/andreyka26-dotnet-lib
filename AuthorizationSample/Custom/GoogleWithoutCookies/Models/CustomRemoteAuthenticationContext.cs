﻿using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace GoogleWithoutCookies.Models
{
    public class CustomRemoteAuthenticationContext<TOptions> : CustomHandleRequestContext<TOptions> where TOptions : CustomAuthenticationSchemeOptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="scheme">The authentication scheme.</param>
        /// <param name="options">The authentication options associated with the scheme.</param>
        /// <param name="properties">The authentication properties.</param>
        protected CustomRemoteAuthenticationContext(
            HttpContext context,
            AuthenticationScheme scheme,
            TOptions options,
            AuthenticationProperties? properties)
            : base(context, scheme, options)
            => Properties = properties ?? new AuthenticationProperties();

        /// <summary>
        /// Gets the <see cref="ClaimsPrincipal"/> containing the user claims.
        /// </summary>
        public ClaimsPrincipal? Principal { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AuthenticationProperties"/>.
        /// </summary>
        public virtual AuthenticationProperties? Properties { get; set; }

        /// <summary>
        /// Calls success creating a ticket with the <see cref="Principal"/> and <see cref="Properties"/>.
        /// </summary>
        public void Success() => Result = HandleRequestResult.Success(new AuthenticationTicket(Principal!, Properties, Scheme.Name));

        /// <summary>
        /// Indicates that authentication failed.
        /// </summary>
        /// <param name="failure">The exception associated with the failure.</param>
        public void Fail(Exception failure) => Result = HandleRequestResult.Fail(failure);

        /// <summary>
        /// Indicates that authentication failed.
        /// </summary>
        /// <param name="failureMessage">The exception associated with the failure.</param>
        public void Fail(string failureMessage) => Result = HandleRequestResult.Fail(failureMessage);
    }
}
