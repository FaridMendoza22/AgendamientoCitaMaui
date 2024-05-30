using AgendamientoCita.View;

namespace AgendamientoCita
{
    public partial class App : Application
    {
        readonly LocalDbService dbService;
        public static CustomerInSession? CustomerInSession { get; set; }
        public App()
        {
            InitializeComponent();
            dbService = MauiProgram.Services.GetService<LocalDbService>()!;
            MainPage = new NavigationPage(new Bienvenido());
            _ = SearchUser();        
        }

        public async Task SearchUser()
        {
            var User = await dbService.GetCustomer();

            if (User is not null)
            {
                App.CustomerInSession = User;
                App.Current!.MainPage = new NavigationPage(new HomePage());
            }else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}
