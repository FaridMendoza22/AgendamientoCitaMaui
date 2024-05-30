using Configuration.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using CustomerEntities = Customer.Entities;

namespace AgendamientoCita.View
{
    public partial class VerCitasPage : ContentPage, INotifyPropertyChanged
    {

        private string _backendUrl = "https://customermodulebackend20240520205729.azurewebsites.net/api/Appointment/getDataWithServices/"; // Reemplace con la URL real de su back-end
        private string _backendServices = "https://configurationmodulebackend20240519113346.azurewebsites.net/api/Service/getData";
        private bool HasNoData { get; set; } = true;
        public bool HasData;




        public VerCitasPage()
        {
            InitializeComponent();
            CargarCitas();
        }

        private async void CargarCitas()
        {
            var citas = await ObtenerCitas();
            var services = await ObtenerServicios();

            foreach (var cita in citas)
            {
                foreach (var service in cita.Services!)
                {
                    service.Service = services.FirstOrDefault(s => s.Rowid == service.RowidService);
                }
            }

            HasData = citas.Count > 0;
            HasNoData = !HasData;
            CitasCollectionView.ItemsSource = citas;
        }

        private async Task<List<CustomerEntities.Appointment>> ObtenerCitas()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var solicitud = new HttpRequestMessage(HttpMethod.Get, $"{_backendUrl}{App.CustomerInSession!.Rowid}");

                    var respuesta = await client.SendAsync(solicitud);

                    if (respuesta.IsSuccessStatusCode)
                    {
                        string contenidoRespuesta = await respuesta.Content.ReadAsStringAsync();
                        var citas = JsonConvert.DeserializeObject<List<CustomerEntities.Appointment>>(contenidoRespuesta);
                        return citas;
                    }
                    else
                    {
                        throw new Exception($"No se pudieron obtener las citas: {respuesta.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las citas: {ex.Message}");
                return new List<CustomerEntities.Appointment>();
            }
        }

        private async Task<List<Service>> ObtenerServicios()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var solicitud = new HttpRequestMessage(HttpMethod.Get, $"{ _backendServices }");

                    var respuesta = await client.SendAsync(solicitud);

                    if (respuesta.IsSuccessStatusCode)
                    {
                        string contenidoRespuesta = await respuesta.Content.ReadAsStringAsync();
                        var servicios = JsonConvert.DeserializeObject<List<Service>>(contenidoRespuesta);
                        return servicios;
                    }
                    else
                    {
                        throw new Exception($"No se pudieron obtener los servicios: {respuesta.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los servicios: {ex.Message}");
                return new List<Service>();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    // Convertidor para el estado de la cita
    public class StateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((int)value)
            {
                case 0: return "Programado";
                case 1: return "Cancelado";
                case 2: return "Atendido";
                case 3: return "Expirado";
                default: return "NA";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Convertidor para el estado del pago
    public class PaymentStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value)
            {
                case 0: return "Pendiente";
                case 1: return "Pagado";
                case 2: return "Sin pagar";
                default: return "Unknown";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
