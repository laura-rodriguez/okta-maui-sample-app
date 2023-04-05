using Microsoft.Extensions.Logging;
using OktaMauiSampleApp.Okta;
using OktaClientConfiguration = OktaMauiSampleApp.Okta.OktaClientConfiguration;

namespace OktaMauiSampleApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<MainPage>();

        var oktaClientConfiguration = new Okta.OktaClientConfiguration()
        {
            OktaDomain = "https://myOktaDomain.com",
            ClientId = "foo",
            RedirectUri = "myapp://callback",
            Browser = new WebBrowserAuthenticator()
        };

        builder.Services.AddSingleton(new OktaClient(oktaClientConfiguration));

        return builder.Build();
	}
}
