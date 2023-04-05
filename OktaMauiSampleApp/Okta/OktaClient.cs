using IdentityModel.OidcClient;
using IdentityModel.Client;

namespace OktaMauiSampleApp.Okta
{
    public class OktaClient
    {
        private readonly OidcClient _oidcClient;
        private readonly OidcClientOptions _oidcClientOptions;

        /// <summary>
        /// Gets the OktaClient's Configuration
        /// </summary>
        public OktaClientConfiguration Configuration { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OktaClient"/> class.
        /// </summary>
        /// <param name="configuration">The OktaClient configuration.</param>
        
        public OktaClient(OktaClientConfiguration configuration)
        {
            Configuration = configuration;
            _oidcClientOptions = BuildOidcClient(configuration);
            _oidcClient = new OidcClient(_oidcClientOptions);
        }


        /// <summary>
        /// Starts the authorization flow.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the request.</param>
        /// <returns>The login result.</returns>
        public async Task<LoginResult> LoginAsync(CancellationToken cancellationToken = default)
        {
            await EnsureProviderInformationAsync(cancellationToken);
            return await _oidcClient.LoginAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Ends the user's Okta session in the browser.
        /// </summary>
        /// <param name="idToken">The id token.</param>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the request.</param>
        /// <returns>The logout result.</returns>
        public async Task<LogoutResult> LogoutAsync(string idToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                throw new ArgumentNullException(nameof(idToken));
            }

            await EnsureProviderInformationAsync(cancellationToken);

            var logoutRequest = new LogoutRequest()
            {
                IdTokenHint = idToken,
            };

            return await _oidcClient.LogoutAsync(logoutRequest, cancellationToken).ConfigureAwait(false);
        }
        

        /// <summary>
        /// Build a new <c>OidcClientOptions</c> instance based on user's configuration.
        /// </summary>
        /// <param name="configuration">The <see cref="OktaClientConfiguration"/> configuration.</param>
        /// <returns>A new instance of <c>OidcClientOptions</c>.</returns>
        private static OidcClientOptions BuildOidcClient(OktaClientConfiguration configuration)
        {
            var scopeString = string.Join(" ", configuration.Scope?.ToArray());

            return new OidcClientOptions
            {
                Authority = configuration.OktaDomain,
                ClientId = configuration.ClientId,
                Scope = scopeString,
                RedirectUri = configuration.RedirectUri,
                PostLogoutRedirectUri = string.IsNullOrEmpty(configuration.PostLogoutRedirectUri) ? configuration.RedirectUri : configuration.PostLogoutRedirectUri,
                Browser = configuration.Browser,
                RefreshDiscoveryDocumentForLogin = false,
            };
        }

        /// <summary>
        /// Retrieves and sets the Provider Information taking into account Okta's multiple authorization servers
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task EnsureProviderInformationAsync(CancellationToken cancellationToken = default)
        {
            var oktaOrgAuthorizationServer =
                Configuration.OktaDomain.Split("/oauth2")?.FirstOrDefault() ?? Configuration.OktaDomain;

            using (var httpClient = new HttpClient())
            {
                var discoveryDocumentResponse = await httpClient.GetDiscoveryDocumentAsync(
                    new DiscoveryDocumentRequest
                    {
                        Address = Configuration.OktaDomain,
                        Policy =
                        {
                            // Okta Org AS must be included to avoid https://stackoverflow.com/questions/56459997/endpoint-belongs-to-different-authority
                            AdditionalEndpointBaseAddresses = new List<string> { oktaOrgAuthorizationServer }
                        }

                    }, cancellationToken).ConfigureAwait(false);

                if (discoveryDocumentResponse.IsError)
                {
                    throw new InvalidOperationException("Error loading discovery document: " +
                                                        discoveryDocumentResponse.Error, discoveryDocumentResponse.Exception);
                }
                
                _oidcClient.Options.ProviderInformation = new ProviderInformation
                {
                    IssuerName = discoveryDocumentResponse.Issuer,
                    KeySet = discoveryDocumentResponse.KeySet,
                    AuthorizeEndpoint = discoveryDocumentResponse.AuthorizeEndpoint,
                    TokenEndpoint = discoveryDocumentResponse.TokenEndpoint,
                    EndSessionEndpoint = discoveryDocumentResponse.EndSessionEndpoint,
                    UserInfoEndpoint = discoveryDocumentResponse.UserInfoEndpoint,
                    TokenEndPointAuthenticationMethods =
                        discoveryDocumentResponse.TokenEndpointAuthenticationMethodsSupported,
                    
                };
            }
        }
    }
}

