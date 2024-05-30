using Appointment.Globals.Enums;
using Configuration.Entities;
using Customer.Entities;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AgendamientoCita.View;

public partial class AgendarCitas : ContentPage
{
    public List<Service> ServicesSelected { get; set; } = new();
    public AgendarCitas()
    {
        InitializeComponent();

        // Cargar empleados desde la API
        _ = CargarEmpleados();

        // Cargar servicios desde la API
        _ = CargarServicios();
    }

    async Task CargarEmpleados()
    {
        var Url = "https://configurationmodulebackend20240519113346.azurewebsites.net/api/employee/getData";
        using var client = new HttpClient();
        var request = await client.GetAsync(Url);

        string contenidoRespuesta = await request.Content.ReadAsStringAsync();

        var Data = JsonConvert.DeserializeObject<List<Employee>>(contenidoRespuesta);

        employeePicker.ItemsSource = Data;
    }

    async Task CargarServicios()
    {
        var Url = "https://configurationmodulebackend20240519113346.azurewebsites.net/api/service/getData";
        using var client = new HttpClient();
        var request = await client.GetAsync(Url);

        string contenidoRespuesta = await request.Content.ReadAsStringAsync();

        var Data = JsonConvert.DeserializeObject<List<Service>>(contenidoRespuesta);

        servicePicker.ItemsSource = Data;
    }

    public void AgregarServicio_Clicked(object sender, EventArgs e)
    {
        ServicesSelected.Add((Service) servicePicker.SelectedItem);
        collectionView.ItemsSource = ServicesSelected.ToList();
    }

    public void GuardarCita_Clicked(object sender, EventArgs e)
    {
        _ = GuardarCitaAsync();
    }

    async Task GuardarCitaAsync()
    {
        if(DateOnly.FromDateTime((startDatePicker.Date + startTimePicker.Time)) < DateOnly.FromDateTime(DateTime.Now))
        {
            _ = DisplayAlert("Error", "Sólo se admiten fechas a partir de hoy", "Ok");
            return;
        }

        var Data = new Customer.Entities.Appointment()
        {
            StartTime = (startDatePicker.Date + startTimePicker.Time).ToUniversalTime(),
            EndTime = (startDatePicker.Date + endTimePicker.Time).ToUniversalTime(),
            State = EnumAppointmentState.Scheduled,
            PaymentState = EnumPaymentState.Pending,
            RowidCustomer = App.CustomerInSession!.Rowid,//Reemplazar por el de la bd,
            RowidEmployee = ((Employee)employeePicker.SelectedItem).Rowid,
            Services = ServicesSelected.Select(x => new AppointmentDetail()
            {
                RowidService = x.Rowid,
            }).ToList(),
        };
        var json = JsonConvert.SerializeObject(Data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using (var client = new HttpClient())
        {
            var response = await client.PostAsync("https://customermodulebackend20240520205729.azurewebsites.net/api/Appointment/SaveWithDetails", content);
            string contenidoRespuesta = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                _ = DisplayAlert("Exito", "Se guardó", "Ok");
                _ = Navigation.PopAsync();
            }
            else
            {
                var Errors = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(contenidoRespuesta);

                if(Errors?.Count > 0)
                {
                    _ = DisplayAlert("Error", string.Join(" \n ", Errors.SelectMany(x => x.Value)), "Ok");
                }
                else
                {
                    _ = DisplayAlert("Error", "No se guardó", "Ok");
                }
            }
        }
    }
}