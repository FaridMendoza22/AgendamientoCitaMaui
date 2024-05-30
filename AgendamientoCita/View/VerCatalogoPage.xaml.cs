using Configuration.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Maui.Controls;

namespace AgendamientoCita.View
{
    public partial class VerCatalogoPage : ContentPage
    {
        private string _backendUrl = "https://configurationmodulebackend20240519113346.azurewebsites.net/api/Catalogue/getDataWithServices"; // Reemplace con la URL real de su back-end
        private bool HasNoData { get; set; } = true;
        public bool HasData;
        public VerCatalogoPage()
        {
            InitializeComponent();
            CargarCatalogos();

            // Es posible que desee llamar a un método aquí para obtener datos y completar la interfaz de usuario después de la inicialización
        }

        private async void CargarCatalogos()
        {
            var catalogos = await ObtenerCatalogosYServicios();

            HasData = catalogos.Count > 0;
            HasNoData = !HasData;
            CatalogosCollectionView.ItemsSource = catalogos;
        }

        private async Task<List<Catalogue>> ObtenerCatalogosYServicios()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var solicitud = new HttpRequestMessage(HttpMethod.Get, _backendUrl);

                    var respuesta = await client.SendAsync(solicitud);

                    if (respuesta.IsSuccessStatusCode)
                    {
                        string contenidoRespuesta = await respuesta.Content.ReadAsStringAsync();
                        List<Catalogue> catalogos = JsonConvert.DeserializeObject<List<Catalogue>>(contenidoRespuesta);
                        return catalogos;
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
                return new List<Catalogue>(); // Devuelva una lista vacía en caso de error
            }
        }

        private void OnServicioSelected(object sender, SelectionChangedEventArgs e)
        {
            // Implemente la lógica para manejar la selección de servicios según su interfaz de usuario y estructura de datos
            if (e.CurrentSelection.Count > 0)
            {
                var selectedService = e.CurrentSelection[0] as Service;
                // Implementar lógica adicional cuando se seleccione un servicio.
            }
        }
    }
}
