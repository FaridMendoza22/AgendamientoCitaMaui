using AgendamientoCita.View;



using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace AgendamientoCita
{
    public partial class LoginPage : ContentPage
    {
        readonly LocalDbService dbService;
        public bool ShowLoader { get; set; }
        public bool ShowContent { get; set; } = true;

        public LoginPage()
        {
            InitializeComponent();
            dbService = MauiProgram.Services.GetService<LocalDbService>()!;

            BindingContext = new LoginViewModel();
        }

        private async void OnSignInButtonClicked(object sender, EventArgs e)
        {
            var viewModel = BindingContext as LoginViewModel;
            loaderRef.IsVisible = true;
            loaderRef.IsRunning = true;
            GridRef.IsVisible = false;
            if (viewModel != null)
            {
                var Customer = await GetCustomer(viewModel.UserName);

                if(Customer is null)
                {
                    loaderRef.IsRunning = false;
                    loaderRef.IsVisible = false;
                    GridRef.IsVisible = true;
                    await DisplayAlert("Error", "Credenciales inválidas", "OK");
                    return;
                }

                btnSession.IsEnabled = false;
                var token = await viewModel.LoginAsync();

                if (!string.IsNullOrEmpty(token))
                {
                    await dbService.CreateCustomer(new()
                    {
                        Token = token,
                        Email = viewModel.UserName,
                        Fullname = Customer.Name + " " + Customer.LastName,
                        Rowid = Customer.Rowid
                    });

                    App.CustomerInSession = await dbService.GetCustomer();

                    App.Current!.MainPage = new NavigationPage(new HomePage());
                    await DisplayAlert("Success", "Inicio de sesion exitoso", "OK");
                    // Navega a otra p�gina o realiza alguna
                     

                }
                else
                {
                    await DisplayAlert("Error", "Credenciales inválidas", "OK");
                }

                btnSession.IsEnabled = true;
            }

            loaderRef.IsRunning = false;
            loaderRef.IsVisible = false;
            GridRef.IsVisible = true;
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
                        throw new Exception($"Error al crear al cliente: {respuesta.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Maneje cualquier excepción que ocurra durante la solicitud
                Console.WriteLine($"Error al crear al cliente: {ex.Message}");
                return default;
            }
        }

        public async void OnRegisterLabel(object sender, EventArgs e)
        {
            loaderRef.IsRunning = true;
            loaderRef.IsVisible = true;
            GridRef.IsVisible = false;

            var viewModel = BindingContext as LoginViewModel;
            if (viewModel != null)
            {
                if (string.IsNullOrEmpty(viewModel.UserName) || string.IsNullOrEmpty(viewModel.Password))
                {
                    _ = DisplayAlert("Error", "Llene el formulario para continuar", "OK");
                    loaderRef.IsRunning = false;
                    loaderRef.IsVisible = false;
                    GridRef.IsVisible = true;
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
                    loaderRef.IsVisible = false;
                    GridRef.IsVisible = true;
                    _ = DisplayAlert("Error", "No se pudo crear al cliente", "OK");
                    return;
                }

                btnSession.IsEnabled = false;
                var token = await viewModel.RegisterAsync();

                if (!string.IsNullOrEmpty(token))
                {
                    await dbService.CreateCustomer(new()
                    {
                        Token = token,
                        Email = viewModel.UserName,
                        Fullname = newClient.Name + " " + newClient.LastName,
                        Rowid = newClient.Rowid
                    });
                    App.CustomerInSession = await dbService.GetCustomer();
                    App.Current!.MainPage = new NavigationPage(new HomePage());
                    _ = DisplayAlert("Success", "Registro exitoso", "OK");
                    // Navega a otra p�gina o realiza alguna

                }
                else
                {
                    _ = DisplayAlert("Error", "Credenciales inválidas", "OK");
                }

                btnSession.IsEnabled = true;
            }
            loaderRef.IsRunning = false;
            loaderRef.IsVisible = false;
            GridRef.IsVisible = true;
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


