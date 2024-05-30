using AgendamientoCita.View;

namespace AgendamientoCita
{
    public partial class App : Application
    {
        public static CustomerInSession? CustomerInSession { get; set; }
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new Bienvenido());   
        }
    }
}
