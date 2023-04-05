using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Results;
using OktaMauiSampleApp.Okta;

namespace OktaMauiSampleApp;

public partial class MainPage : ContentPage
{
	int count = 0;
    private OktaClient _oktaClient;
    private LoginResult _authenticationData;
	public MainPage(OktaClient oktaClient)
	{
		InitializeComponent();
		_oktaClient = oktaClient;
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

    public async void OnLoginClicked(object sender, EventArgs e)
    {
        var loginResult = await _oktaClient.LoginAsync();

        if (!loginResult.IsError)
        {
            _authenticationData = loginResult;
            LoginView.IsVisible = false;
            HomeView.IsVisible = true;

            UserInfoLvw.ItemsSource = loginResult.User.Claims;
            HelloLbl.Text = $"Hello, {loginResult.User.Claims.FirstOrDefault(x => x.Type == "name")?.Value}";
        }
        else
        {
            await DisplayAlert("Error", loginResult.ErrorDescription, "OK");
        }
    }

    public async void OnLogoutClicked(object sender, EventArgs e)
    {
        var logoutResult = await _oktaClient.LogoutAsync(_authenticationData.IdentityToken);

        if (!logoutResult.IsError)
        {
            _authenticationData = null;
            LoginView.IsVisible = true;
            HomeView.IsVisible = false;
        }
        else
        {
            await DisplayAlert("Error", logoutResult.ErrorDescription, "OK");
        }
    }
}

