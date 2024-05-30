using AgendamientoCita.View;



using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AgendamientoCita
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
        }

        private async void OnSignInButtonClicked(object sender, EventArgs e)
        {
            var viewModel = BindingContext as LoginViewModel;
            if (viewModel != null)
            {
                btnSession.IsEnabled = false;
                bool loginSuccessful = await viewModel.LoginAsync();

                if (loginSuccessful)
                {
                    App.Current!.MainPage = new NavigationPage(new HomePage());
                    await DisplayAlert("Success", "Login successful!", "OK");
                    // Navega a otra p�gina o realiza alguna
                     

                }
                else
                {
                    await DisplayAlert("Error", "Login failed. Please check your credentials.", "OK");
                }

                btnSession.IsEnabled = true;
            }
        }
        public async void OnRegisterLabel(object sender, EventArgs e)
        {
            var viewModel = BindingContext as LoginViewModel;
            if (viewModel != null)
            {
                btnSession.IsEnabled = false;
                bool loginSuccessful = await viewModel.RegisterAsync();

                if (loginSuccessful)
                {
                    App.Current!.MainPage = new NavigationPage(new HomePage());
                    _ = DisplayAlert("Success", "Register successful!", "OK");
                    // Navega a otra p�gina o realiza alguna


                }
                else
                {
                    _ = DisplayAlert("Error", "Login failed. Please check your credentials.", "OK");
                }

                btnSession.IsEnabled = true;
            }
        }
    }
}
     

    public class LoginViewModel : BindableObject
    {
        private string _userName;
        private string _password;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }
    

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await LoginAsync());
        }

        public async Task<bool> LoginAsync()
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                return false;
            }

            var loginData = new
            {
                username = UserName,
                password = Password
            };

            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://authenticationmodule.azurewebsites.net/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    // Puedes procesar el contenido de la respuesta aqu� si es necesario
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
        }

    public async Task<bool> RegisterAsync()
    {
        if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
        {
            return false;
        }

        var loginData = new
        {
            username = UserName,
            password = Password
        };

        var json = JsonConvert.SerializeObject(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using (var client = new HttpClient())
        {
            var response = await client.PostAsync("https://authenticationmodule.azurewebsites.net/register", content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Puedes procesar el contenido de la respuesta aqu� si es necesario
                return true;
            }
            else
            {
                return false;
            }
        }

    }

}


//public partial class LoginPage : ContentPage
//{
//	public LoginPage()
//	{
//		InitializeComponent();
//	}
//    private async void OnSignInButtonClicked(object sender, EventArgs e)
//    {
//        // Navegar a la nueva p�gina
//        App.Current!.MainPage = new NavigationPage(new HomePage());
//    }
//}