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
        // Navegar a la p�gina de agendar cita
        Navigation.PushAsync(new AgendarCitas());
    }

    private void VerCitas(object sender, EventArgs e)
    {
        // Navegar a la p�gina de ver citas
       //Navigation.PushAsync(new VerCitasPage());
    }

    private void CrearCita(object sender, EventArgs e)
    {
        // Crear una nueva cita
        // ...
    }

    private void ConocerCharlysSalon(object sender, EventArgs e)
    {
        // Abrir una p�gina web con informaci�n sobre Charly's Salon
        // ...
    }

    private void Salir(object sender, EventArgs e)
    {
        // Salir de la aplicaci�n
        //Application.Current.Quit();
    }
}

