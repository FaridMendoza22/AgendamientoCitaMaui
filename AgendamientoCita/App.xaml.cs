﻿namespace AgendamientoCita
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginPage());
          
        }
    }
}
