# okta-maui-sample-app
A MAUI sample application that uses Okta for authentication

## Getting Started

1. Create an Okta account, also know as an _organization_, see [Developer Signup](https://developer.okta.com/signup/).
2. In your `Okta Developer Console` add an application; follow the directions at [Set up your Application](https://developer.okta.com/docs/guides/implement-auth-code-pkce/setup-app/) and accept the defaults.
3. In your `Okta Developer Console` register your application's login and logout redirect callbacks, see [Register Redirects](#register-redirects).
4. Configure your application to use the values registered in the previous step, see [Configure Your Application](#configure-your-application)
5. Go to Visual Studio (I used VS 2022 Community Edition) and update the `OktaClientConfiguration` settings with your application details in the `MauiProgram.cs`:

```csharp
var oktaClientConfiguration = new Okta.OktaClientConfiguration()
        {
            // Use "https://myOktaDomain.com/oauth2/default" for the "default" authorization server, or
            // "https://myOktaDomain.com/oauth2/<MyCustomAuthorizationServerId>"
            OktaDomain = "https://myOktaDomain.com", 
            ClientId = "<MyClientId>",
            RedirectUri = "myapp://callback",
            Browser = new WebBrowserAuthenticator()
        };

```

6. Start an Android emulator of your choice.

> Note: This sample **only** implements the Android platform. You can go ahead and add the bits corresponding to the platform you want to try.

### Register Redirects

To register redirect URIs do the following:

1. Sign in to your `Okta Developer Console` as an administrator.
2. Click the `Applications` tab and select your application.  If you need to set up your application see [Set up your Application](https://developer.okta.com/docs/guides/implement-auth-code-pkce/setup-app/). 
3. Ensure you are on the `General` tab, then go to `General Settings` and click `Edit`.
4. Go to the `Login` section.
5. Below `Login redirect URIs` click the `Add URI` button.
6. Enter a value appropriate for your application, this example uses the following:
    ```
    myApp://callback
    ```
7. Below `Logout redirect URIs` click the `Add URI` button.
8. Enter a value appropriate for your application, this example uses the following:
    ```
    myapp://callback
    ```
9. Click `Save`.
