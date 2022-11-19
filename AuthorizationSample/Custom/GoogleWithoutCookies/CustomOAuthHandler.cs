﻿using GoogleWithoutCookies.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace GoogleWithoutCookies
{
    public class CustomOAuthHandler<TOptions> : CustomRemoteAuthenticationHandler<TOptions> where TOptions : CustomOAuthOptions, new()
    {
        /// <summary>
        /// Gets the <see cref="HttpClient"/> instance used to communicate with the remote authentication provider.
        /// </summary>
        protected HttpClient Backchannel => Options.Backchannel;

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where processing is occurring.
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        protected new CustomOAuthEvents Events
        {
            get { return (CustomOAuthEvents)base.Events; }
            set { base.Events = value; }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="OAuthHandler{TOptions}"/>.
        /// </summary>
        /// <inheritdoc />
        public CustomOAuthHandler(IOptionsMonitor<TOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        /// <summary>
        /// Creates a new instance of the events instance.
        /// </summary>
        /// <returns>A new instance of the events instance.</returns>
        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new OAuthEvents());

        /// <inheritdoc />
        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            var query = Request.Query;

            var state = query["state"];
            var properties = Options.StateDataFormat.Unprotect(state);

            if (properties == null)
            {
                return HandleRequestResult.Fail("Invalid state");
            }

            // OAuth2 10.12 CSRF
            if (!ValidateCorrelationId(properties))
            {
                return HandleRequestResult.Fail("Correlation failed.", properties);
            }

            var error = query["error"];
            if (!StringValues.IsNullOrEmpty(error))
            {
                // Note: access_denied errors are special protocol errors indicating the user didn't
                // approve the authorization demand requested by the remote authorization server.
                // Since it's a frequent scenario (that is not caused by incorrect configuration),
                // denied errors are handled differently using HandleAccessDeniedErrorAsync().
                // Visit https://tools.ietf.org/html/rfc6749#section-4.1.2.1 for more information.
                var errorDescription = query["error_description"];
                var errorUri = query["error_uri"];
                if (StringValues.Equals(error, "access_denied"))
                {
                    var result = await HandleAccessDeniedErrorAsync(properties);
                    if (!result.None)
                    {
                        return result;
                    }
                    var deniedEx = new Exception("Access was denied by the resource owner or by the remote server.");
                    deniedEx.Data["error"] = error.ToString();
                    deniedEx.Data["error_description"] = errorDescription.ToString();
                    deniedEx.Data["error_uri"] = errorUri.ToString();

                    return HandleRequestResult.Fail(deniedEx, properties);
                }

                var failureMessage = new StringBuilder();
                failureMessage.Append(error);
                if (!StringValues.IsNullOrEmpty(errorDescription))
                {
                    failureMessage.Append(";Description=").Append(errorDescription);
                }
                if (!StringValues.IsNullOrEmpty(errorUri))
                {
                    failureMessage.Append(";Uri=").Append(errorUri);
                }

                var ex = new Exception(failureMessage.ToString());
                ex.Data["error"] = error.ToString();
                ex.Data["error_description"] = errorDescription.ToString();
                ex.Data["error_uri"] = errorUri.ToString();

                return HandleRequestResult.Fail(ex, properties);
            }

            var code = query["code"];

            if (StringValues.IsNullOrEmpty(code))
            {
                return HandleRequestResult.Fail("Code was not found.", properties);
            }

            var codeExchangeContext = new OAuthCodeExchangeContext(properties, code.ToString(), BuildRedirectUri(Options.CallbackPath));
            using var tokens = await ExchangeCodeAsync(codeExchangeContext);

            if (tokens.Error != null)
            {
                return HandleRequestResult.Fail(tokens.Error, properties);
            }

            if (string.IsNullOrEmpty(tokens.AccessToken))
            {
                return HandleRequestResult.Fail("Failed to retrieve access token.", properties);
            }

            var identity = new ClaimsIdentity(ClaimsIssuer);

            if (Options.SaveTokens)
            {
                var authTokens = new List<AuthenticationToken>();

                authTokens.Add(new AuthenticationToken { Name = "access_token", Value = tokens.AccessToken });
                if (!string.IsNullOrEmpty(tokens.RefreshToken))
                {
                    authTokens.Add(new AuthenticationToken { Name = "refresh_token", Value = tokens.RefreshToken });
                }

                if (!string.IsNullOrEmpty(tokens.TokenType))
                {
                    authTokens.Add(new AuthenticationToken { Name = "token_type", Value = tokens.TokenType });
                }

                if (!string.IsNullOrEmpty(tokens.ExpiresIn))
                {
                    int value;
                    if (int.TryParse(tokens.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                    {
                        // https://www.w3.org/TR/xmlschema-2/#dateTime
                        // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
                        var expiresAt = Clock.UtcNow + TimeSpan.FromSeconds(value);
                        authTokens.Add(new AuthenticationToken
                        {
                            Name = "expires_at",
                            Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                        });
                    }
                }

                properties.StoreTokens(authTokens);
            }

            var ticket = await CreateTicketAsync(identity, properties, tokens);
            if (ticket != null)
            {
                return HandleRequestResult.Success(ticket);
            }
            else
            {
                return HandleRequestResult.Fail("Failed to retrieve user information from remote server.", properties);
            }
        }

        /// <summary>
        /// Exchanges the authorization code for a authorization token from the remote provider.
        /// </summary>
        /// <param name="context">The <see cref="OAuthCodeExchangeContext"/>.</param>
        /// <returns>The response <see cref="OAuthTokenResponse"/>.</returns>
        protected virtual async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
        {
            var tokenRequestParameters = new Dictionary<string, string>()
            {
                { "client_id", Options.ClientId },
                { "redirect_uri", context.RedirectUri },
                { "client_secret", Options.ClientSecret },
                { "code", context.Code },
                { "grant_type", "authorization_code" },
            };

            // PKCE https://tools.ietf.org/html/rfc7636#section-4.5, see BuildChallengeUrl
            if (context.Properties.Items.TryGetValue(OAuthConstants.CodeVerifierKey, out var codeVerifier))
            {
                tokenRequestParameters.Add(OAuthConstants.CodeVerifierKey, codeVerifier!);
                context.Properties.Items.Remove(OAuthConstants.CodeVerifierKey);
            }

            var requestContent = new FormUrlEncodedContent(tokenRequestParameters!);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Options.TokenEndpoint);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Content = requestContent;
            requestMessage.Version = Backchannel.DefaultRequestVersion;
            var response = await Backchannel.SendAsync(requestMessage, Context.RequestAborted);
            var body = await response.Content.ReadAsStringAsync(Context.RequestAborted);

            return response.IsSuccessStatusCode switch
            {
                true => OAuthTokenResponse.Success(JsonDocument.Parse(body)),
                false => PrepareFailedOAuthTokenReponse(response, body)
            };
        }

        private static OAuthTokenResponse PrepareFailedOAuthTokenReponse(HttpResponseMessage response, string body)
        {
            var exception = GetStandardErrorException(JsonDocument.Parse(body));

            if (exception is null)
            {
                var errorMessage = $"OAuth token endpoint failure: Status: {response.StatusCode};Headers: {response.Headers};Body: {body};";
                return OAuthTokenResponse.Failed(new Exception(errorMessage));
            }

            return OAuthTokenResponse.Failed(exception);

            static Exception? GetStandardErrorException(JsonDocument response)
            {
                var root = response.RootElement;
                var error = root.GetString("error");

                if (error is null)
                {
                    return null;
                }

                var result = new StringBuilder("OAuth token endpoint failure: ");
                result.Append(error);

                if (root.TryGetProperty("error_description", out var errorDescription))
                {
                    result.Append(";Description=");
                    result.Append(errorDescription);
                }

                if (root.TryGetProperty("error_uri", out var errorUri))
                {
                    result.Append(";Uri=");
                    result.Append(errorUri);
                }

                var exception = new Exception(result.ToString());
                exception.Data["error"] = error.ToString();
                exception.Data["error_description"] = errorDescription.ToString();
                exception.Data["error_uri"] = errorUri.ToString();

                return exception;
            }
        }

        /// <summary>
        /// Creates an <see cref="AuthenticationTicket"/> from the specified <paramref name="tokens"/>.
        /// </summary>
        /// <param name="identity">The <see cref="ClaimsIdentity"/>.</param>
        /// <param name="properties">The <see cref="AuthenticationProperties"/>.</param>
        /// <param name="tokens">The <see cref="OAuthTokenResponse"/>.</param>
        /// <returns>The <see cref="AuthenticationTicket"/>.</returns>
        protected virtual async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            using (var user = JsonDocument.Parse("{}"))
            {
                var context = new CustomOAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, user.RootElement);
                await Events.CreatingTicket(context);
                return new AuthenticationTicket(context.Principal!, context.Properties, Scheme.Name);
            }
        }

        /// <inheritdoc />
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (string.IsNullOrEmpty(properties.RedirectUri))
            {
                properties.RedirectUri = OriginalPathBase + OriginalPath + Request.QueryString;
            }

            // OAuth2 10.12 CSRF
            GenerateCorrelationId(properties);

            var authorizationEndpoint = BuildChallengeUrl(properties, BuildRedirectUri(Options.CallbackPath));
            var redirectContext = new CustomRedirectContext<CustomOAuthOptions>(
                Context, Scheme, Options,
                properties, authorizationEndpoint);
            await Events.RedirectToAuthorizationEndpoint(redirectContext);

            var location = Context.Response.Headers.Location;
            if (location == StringValues.Empty)
            {
                location = "(not set)";
            }

            var cookie = Context.Response.Headers.SetCookie;
            if (cookie == StringValues.Empty)
            {
                cookie = "(not set)";
            }

            // Logger.HandleChallenge(location.ToString(), cookie.ToString());
        }

        /// <summary>
        /// Constructs the OAuth challenge url.
        /// </summary>
        /// <param name="properties">The <see cref="AuthenticationProperties"/>.</param>
        /// <param name="redirectUri">The url to redirect to once the challenge is completed.</param>
        /// <returns>The challenge url.</returns>
        protected virtual string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            var scopeParameter = properties.GetParameter<ICollection<string>>(OAuthChallengeProperties.ScopeKey);
            var scope = scopeParameter != null ? FormatScope(scopeParameter) : FormatScope();

            var parameters = new Dictionary<string, string>
            {
                { "client_id", Options.ClientId },
                { "scope", scope },
                { "response_type", "code" },
                { "redirect_uri", redirectUri },
            };

            if (Options.UsePkce)
            {
                var bytes = new byte[32];
                RandomNumberGenerator.Fill(bytes);
                var codeVerifier = Microsoft.AspNetCore.Authentication.Base64UrlTextEncoder.Encode(bytes);

                // Store this for use during the code redemption.
                properties.Items.Add(OAuthConstants.CodeVerifierKey, codeVerifier);

                var challengeBytes = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));
                var codeChallenge = WebEncoders.Base64UrlEncode(challengeBytes);

                parameters[OAuthConstants.CodeChallengeKey] = codeChallenge;
                parameters[OAuthConstants.CodeChallengeMethodKey] = OAuthConstants.CodeChallengeMethodS256;
            }

            parameters["state"] = Options.StateDataFormat.Protect(properties);

            return QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, parameters!);
        }

        /// <summary>
        /// Format a list of OAuth scopes.
        /// </summary>
        /// <param name="scopes">List of scopes.</param>
        /// <returns>Formatted scopes.</returns>
        protected virtual string FormatScope(IEnumerable<string> scopes)
            => string.Join(" ", scopes); // OAuth2 3.3 space separated

        /// <summary>
        /// Format the <see cref="OAuthOptions.Scope"/> property.
        /// </summary>
        /// <returns>Formatted scopes.</returns>
        /// <remarks>Subclasses should rather override <see cref="FormatScope(IEnumerable{string})"/>.</remarks>
        protected virtual string FormatScope()
            => FormatScope(Options.Scope);
    }
}
