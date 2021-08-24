using System;
using DemoMobileApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DemoMobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<IScanningService, QRScanningService>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
