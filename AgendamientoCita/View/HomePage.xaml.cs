using System;

namespace AgendamientoCita.View;

public partial class HomePage : ContentPage
{
    readonly LocalDbService dbService;
    public HomePage()
	{
        dbService = MauiProgram.Services.GetService<LocalDbService>()!;
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
       Navigation.PushAsync(new VerCitasPage());
    }

    private void VerCatalogo(object sender, EventArgs e)
    {
        // Crear una nueva cita
        // ...
        Navigation.PushAsync(new VerCatalogoPage());

    }

    private void Salir(object sender, EventArgs e)
    {
        _ = dbService.DeleteCustomer();
        App.CustomerInSession = null;
        // Salir de la aplicación
        App.Current!.MainPage = new NavigationPage(new LoginPage());
    }
}

