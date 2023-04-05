using IBrowser = IdentityModel.OidcClient.Browser.IBrowser;

namespace OktaMauiSampleApp.Okta
{
    public class OktaClientConfiguration
    {
        public string ClientId { get; set; }

        public string RedirectUri { get; set; }

        public string PostLogoutRedirectUri { get; set; }

        public IList<string> Scope { get; set; } = new string[] { "openid", "profile" };

        public string OktaDomain { get; set; }

        public IBrowser Browser { get; set; }

    }
}
