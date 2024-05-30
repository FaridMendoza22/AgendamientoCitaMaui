namespace AgendamientoCita
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            //si hay info en la bd, iniciar en home
            MainPage = new NavigationPage(new LoginPage());
          
        }
    }
}
