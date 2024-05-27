using AgendamientoCita.View;

namespace AgendamientoCita;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}
    private async void OnSignInButtonClicked(object sender, EventArgs e)
    {
        // Navegar a la nueva página
        App.Current!.MainPage = new NavigationPage(new HomePage());
    }
}