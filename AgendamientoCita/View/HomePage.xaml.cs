using System;

namespace AgendamientoCita.View;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
	}
    private void AgendarCita(object sender, EventArgs e)
    {
        // Navegar a la página de agendar cita
        Navigation.PushAsync(new AgendarCitas());
    }

    private void VerCitas(object sender, EventArgs e)
    {
        // Navegar a la página de ver citas
       //Navigation.PushAsync(new VerCitasPage());
    }

    private void CrearCita(object sender, EventArgs e)
    {
        // Crear una nueva cita
        // ...
    }

    private void ConocerCharlysSalon(object sender, EventArgs e)
    {
        // Abrir una página web con información sobre Charly's Salon
        // ...
    }

    private void Salir(object sender, EventArgs e)
    {
        // Salir de la aplicación
        //Application.Current.Quit();
    }
}

