using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.DataProtection;
using System.Globalization;
using System.Resources;
using System.Security.Claims;

namespace GoogleWithoutCookies
{
    public class CustomGoogleOptions : CustomOAuthOptions
    {
        public CustomGoogleOptions()
        {
            CallbackPath = new PathString("/signin-google");
            AuthorizationEndpoint = GoogleDefaults.AuthorizationEndpoint;
            TokenEndpoint = GoogleDefaults.TokenEndpoint;
            UserInformationEndpoint = GoogleDefaults.UserInformationEndpoint;
            Scope.Add("openid");
            Scope.Add("profile");
            Scope.Add("email");

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id"); // v2
            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub"); // v3
            ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
            ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
            ClaimActions.MapJsonKey("urn:google:profile", "link");
            ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        }

        public string? AccessType { get; set; }
    }

    public class CustomOAuthOptions : CustomRemoteAuthenticationOptions
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OAuthOptions"/>.
        /// </summary>
        public CustomOAuthOptions()
        {
            Events = new OAuthEvents();
        }

        /// <summary>
        /// Check that the options are valid. Should throw an exception if things are not ok.
        /// </summary>
        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrEmpty(ClientId))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "sdf", nameof(ClientId)), nameof(ClientId));
            }

            if (string.IsNullOrEmpty(ClientSecret))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "sdf", nameof(ClientSecret)), nameof(ClientSecret));
            }

            if (string.IsNullOrEmpty(AuthorizationEndpoint))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "sdf", nameof(AuthorizationEndpoint)), nameof(AuthorizationEndpoint));
            }

            if (string.IsNullOrEmpty(TokenEndpoint))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "sdf", nameof(TokenEndpoint)), nameof(TokenEndpoint));
            }

            if (!CallbackPath.HasValue)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "sdf", nameof(CallbackPath)), nameof(CallbackPath));
            }
        }

        /// <summary>
        /// Gets or sets the provider-assigned client id.
        /// </summary>
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the provider-assigned client secret.
        /// </summary>
        public string ClientSecret { get; set; } = default!;

        /// <summary>
        /// Gets or sets the URI where the client will be redirected to authenticate.
        /// </summary>
        public string AuthorizationEndpoint { get; set; } = default!;

        /// <summary>
        /// Gets or sets the URI the middleware will access to exchange the OAuth token.
        /// </summary>
        public string TokenEndpoint { get; set; } = default!;

        /// <summary>
        /// Gets or sets the URI the middleware will access to obtain the user information.
        /// This value is not used in the default implementation, it is for use in custom implementations of
        /// <see cref="OAuthEvents.OnCreatingTicket" />.
        /// </summary>
        public string UserInformationEndpoint { get; set; } = default!;

        /// <summary>
        /// Gets or sets the <see cref="OAuthEvents"/> used to handle authentication events.
        /// </summary>
        public new OAuthEvents Events
        {
            get { return (OAuthEvents)base.Events; }
            set { base.Events = value; }
        }

        /// <summary>
        /// A collection of claim actions used to select values from the json user data and create Claims.
        /// </summary>
        public ClaimActionCollection ClaimActions { get; } = new ClaimActionCollection();

        /// <summary>
        /// Gets the list of permissions to request.
        /// </summary>
        public ICollection<string> Scope { get; } = new HashSet<string>();

        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; } = default!;

        /// <summary>
        /// Enables or disables the use of the Proof Key for Code Exchange (PKCE) standard. See <see href="https://tools.ietf.org/html/rfc7636"/>.
        /// The default value is `false` but derived handlers should enable this if their provider supports it.
        /// </summary>
        public bool UsePkce { get; set; }
    }

    public class CustomRemoteAuthenticationOptions  : AuthenticationSchemeOptions
    {
        private const string CorrelationPrefix = ".AspNetCore.Correlation.";

        private CookieBuilder _correlationCookieBuilder;

        /// <summary>
        /// Initializes a new <see cref="RemoteAuthenticationOptions"/>.
        /// </summary>
        public CustomRemoteAuthenticationOptions()
        {
            _correlationCookieBuilder = new CorrelationCookieBuilder(this)
            {
                Name = CorrelationPrefix,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                SecurePolicy = CookieSecurePolicy.SameAsRequest,
                IsEssential = true,
            };
        }

        /// <summary>
        /// Checks that the options are valid for a specific scheme
        /// </summary>
        /// <param name="scheme">The scheme being validated.</param>
        public override void Validate(string scheme)
        {
            base.Validate(scheme);
        }

        /// <summary>
        /// Check that the options are valid.  Should throw an exception if things are not ok.
        /// </summary>
        public override void Validate()
        {
            base.Validate();
            if (CallbackPath == null || !CallbackPath.HasValue)
            {
                throw new ArgumentException("Callback");
            }
        }

        /// <summary>
        /// Gets or sets timeout value in milliseconds for back channel communications with the remote identity provider.
        /// </summary>
        /// <value>
        /// The back channel timeout.
        /// </value>
        public TimeSpan BackchannelTimeout { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>
        /// The HttpMessageHandler used to communicate with remote identity provider.
        /// This cannot be set at the same time as BackchannelCertificateValidator unless the value
        /// can be downcast to a WebRequestHandler.
        /// </summary>
        public HttpMessageHandler? BackchannelHttpHandler { get; set; }

        /// <summary>
        /// Used to communicate with the remote identity provider.
        /// </summary>
        public HttpClient Backchannel { get; set; } = default!;

        /// <summary>
        /// Gets or sets the type used to secure data.
        /// </summary>
        public IDataProtectionProvider? DataProtectionProvider { get; set; }

        /// <summary>
        /// The request path within the application's base path where the user-agent will be returned.
        /// The middleware will process this request when it arrives.
        /// </summary>
        public PathString CallbackPath { get; set; }

        /// <summary>
        /// Gets or sets the optional path the user agent is redirected to if the user
        /// doesn't approve the authorization demand requested by the remote server.
        /// This property is not set by default. In this case, an exception is thrown
        /// if an access_denied response is returned by the remote authorization server.
        /// </summary>
        public PathString AccessDeniedPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the parameter used to convey the original location
        /// of the user before the remote challenge was triggered up to the access denied page.
        /// This property is only used when the <see cref="AccessDeniedPath"/> is explicitly specified.
        /// </summary>
        // Note: this deliberately matches the default parameter name used by the cookie handler.
        public string ReturnUrlParameter { get; set; } = "ReturnUrl";

        /// <summary>
        /// Gets or sets the authentication scheme corresponding to the middleware
        /// responsible of persisting user's identity after a successful authentication.
        /// This value typically corresponds to a cookie middleware registered in the Startup class.
        /// When omitted, <see cref="AuthenticationOptions.DefaultSignInScheme"/> is used as a fallback value.
        /// </summary>
        public string? SignInScheme { get; set; }

        /// <summary>
        /// Gets or sets the time limit for completing the authentication flow (15 minutes by default).
        /// </summary>
        public TimeSpan RemoteAuthenticationTimeout { get; set; } = TimeSpan.FromMinutes(15);

        /// <summary>
        /// Gets or sets a value that allows subscribing to remote authentication events.
        /// </summary>
        public new RemoteAuthenticationEvents Events
        {
            get => (RemoteAuthenticationEvents)base.Events!;
            set => base.Events = value;
        }

        /// <summary>
        /// Defines whether access and refresh tokens should be stored in the
        /// <see cref="AuthenticationProperties"/> after a successful authorization.
        /// This property is set to <c>false</c> by default to reduce
        /// the size of the final authentication cookie.
        /// </summary>
        public bool SaveTokens { get; set; }

        /// <summary>
        /// Determines the settings used to create the correlation cookie before the
        /// cookie gets added to the response.
        /// </summary>
        public CookieBuilder CorrelationCookie
        {
            get => _correlationCookieBuilder;
            set => _correlationCookieBuilder = value ?? throw new ArgumentNullException(nameof(value));
        }

        private sealed class CorrelationCookieBuilder : RequestPathBaseCookieBuilder
        {
            private readonly CustomRemoteAuthenticationOptions _options;

            public CorrelationCookieBuilder(CustomRemoteAuthenticationOptions remoteAuthenticationOptions)
            {
                _options = remoteAuthenticationOptions;
            }

            protected override string AdditionalPath => _options.CallbackPath;

            public override CookieOptions Build(HttpContext context, DateTimeOffset expiresFrom)
            {
                var cookieOptions = base.Build(context, expiresFrom);

                if (!Expiration.HasValue || !cookieOptions.Expires.HasValue)
                {
                    cookieOptions.Expires = expiresFrom.Add(_options.RemoteAuthenticationTimeout);
                }

                return cookieOptions;
            }
        }
    }
}
