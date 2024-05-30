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
        CargarServicios();
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

    void AgregarServicio_Clicked(object sender, EventArgs e)
    {
        collectionView.ItemsSource ??= new List<Service>();

        ServicesSelected.Add((Service) servicePicker.SelectedItem);
    }

    async Task GuardarCita_Clicked(object sender, EventArgs e)
    {
        var Data = new Customer.Entities.Appointment()
        {
            StartTime = startDatePicker.Date + startTimePicker.Time,
            EndTime = endDatePicker.Date + endTimePicker.Time,
            State = EnumAppointmentState.Scheduled,
            PaymentState = EnumPaymentState.Pending,
            RowidCustomer = 34,//Reemplazar por el de la bd,
            RowidEmployee = ((Employee)employeePicker.SelectedItem).Rowid,
            Services = ServicesSelected.Select(x => new AppointmentDetail()
            {
                RowidService = x.Rowid,
            }).ToList(),
        };
        var json = JsonConvert.SerializeObject(new { });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using (var client = new HttpClient())
        {
            var response = await client.PostAsync("https://customermodulebackend20240520205729.azurewebsites.net/api/Appointment", content);
            if (response.IsSuccessStatusCode)
            {
                _ = Navigation.PopAsync();
            }else
            {
                _ = DisplayAlert("Error", "No se guard�", "Ok");
            }
        }
    }
}