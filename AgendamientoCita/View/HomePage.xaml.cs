using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

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
        //Navigation.PushAsync(new AgendarCitasPage());
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
        Application.Current.Quit();
    }
}

