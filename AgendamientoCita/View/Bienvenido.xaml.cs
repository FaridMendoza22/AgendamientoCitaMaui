namespace AgendamientoCita.View;

public partial class Bienvenido : ContentPage
{
    readonly LocalDbService dbService;
    public Bienvenido()
	{
        dbService = MauiProgram.Services.GetService<LocalDbService>()!;
        _ = SearchUser();
        InitializeComponent();
    }

    public async Task SearchUser()
    {
        var User = await dbService.GetCustomer();

        if (User is not null)
        {
            App.CustomerInSession = User;
            App.Current!.MainPage = new NavigationPage(new HomePage());
        }
        else
        {
            App.Current!.MainPage = new NavigationPage(new LoginPage());
        }
    }
}