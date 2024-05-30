using AgendamientoCita.View;



using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Customer.Entities;
using Android.Webkit;
using Configuration.Entities;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

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
                var Customer = await GetCustomer(viewModel.UserName);

                if(Customer is null)
                {
                    await DisplayAlert("Error", "Login failed. Please check your credentials.", "OK");
                    return;
                }

                btnSession.IsEnabled = false;
                var token = await viewModel.LoginAsync();

                if (!string.IsNullOrEmpty(token))
                {
                    //AQUI SE VA A CREAR LA INFO EN LA BD

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

        public async Task<Customer.Entities.Customer?> GetCustomer(string Email)
        {
            try
            {
                var Url = $"https://customermodulebackend20240520205729.azurewebsites.net/api/Customer/getData?email={Email}";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var solicitud = new HttpRequestMessage(HttpMethod.Get, Url);

                    var respuesta = await client.SendAsync(solicitud);

                    if (respuesta.IsSuccessStatusCode)
                    {
                        string contenidoRespuesta = await respuesta.Content.ReadAsStringAsync();
                        List<Customer.Entities.Customer>? customers = JsonConvert.DeserializeObject<List<Customer.Entities.Customer>>(contenidoRespuesta);
                        return customers?.First();
                    }
                    else
                    {
                        throw new Exception($"No se pudieron obtener catálogos y servicios: {respuesta.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Maneje cualquier excepción que ocurra durante la solicitud
                Console.WriteLine($"Error al obtener catálogos y servicios: {ex.Message}");
                return default;
            }
        }

        public async void OnRegisterLabel(object sender, EventArgs e)
        {
            var viewModel = BindingContext as LoginViewModel;
            if (viewModel != null)
            {
                if (string.IsNullOrEmpty(viewModel.UserName) || string.IsNullOrEmpty(viewModel.Password))
                {
                    _ = DisplayAlert("Error", "Llene el formulario para continuar", "OK");
                    return;
                }

                string Id = await DisplayPromptAsync("Código", "¿Cómo es tu identificacion?");
                string Name = await DisplayPromptAsync("Nombre", "¿Cómo es tu nombre?");
                string LastName = await DisplayPromptAsync("Apellido", "¿Cómo es tu apellido?");

                var newClient = new Customer.Entities.Customer()
                {
                    Email = viewModel.UserName,
                    Id = Id,
                    Name = Name,
                    LastName = LastName
                };

                newClient = await CreateCustomer(newClient);

                if(newClient is null)
                {
                    _ = DisplayAlert("Error", "No se pudo crear al cliente", "OK");
                    return;
                }

                btnSession.IsEnabled = false;
                var token = await viewModel.RegisterAsync();

                if (!string.IsNullOrEmpty(token))
                {
                    //AQUI SE VA A CREAR LA INFO EN LA BD
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

        public async Task<Customer.Entities.Customer?> CreateCustomer(Customer.Entities.Customer Customer)
        {
            var Url = "https://customermodulebackend20240520205729.azurewebsites.net/api/Customer";

            var json = JsonConvert.SerializeObject(Customer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(Url, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    // Puedes procesar el contenido de la respuesta aqu� si es necesario
                    return JsonConvert.DeserializeObject<Customer.Entities.Customer>(responseContent);
                }
                else
                {
                    return null;
                }
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

        public async Task<string?> LoginAsync()
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                return string.Empty;
            }

            var loginData = new
            {
                Email = UserName,
                Password
            };

            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://authenticationmodule.azurewebsites.net/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var Object = (JObject?)JsonConvert.DeserializeObject(responseContent);
                    // Puedes procesar el contenido de la respuesta aqu� si es necesario
                    var value = Object?.GetValue("data");

                    return value?.ToString();
            }
                else
                {
                    return string.Empty;
                }
            }
            
        }

    public async Task<string?> RegisterAsync()
    {
        if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
        {
            return string.Empty;
        }

        var loginData = new
        {
            Email = UserName,
            Password
        };

        var json = JsonConvert.SerializeObject(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using (var client = new HttpClient())
        {
            var response = await client.PostAsync("https://authenticationmodule.azurewebsites.net/register", content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var Object = (JObject?) JsonConvert.DeserializeObject(responseContent);
                // Puedes procesar el contenido de la respuesta aqu� si es necesario
                var value = Object?.GetValue("data");

                return value?.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

    }

}


