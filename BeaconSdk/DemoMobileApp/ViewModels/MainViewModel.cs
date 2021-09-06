using Xamarin.Forms;
using DemoMobileApp.Services;
using System;

namespace DemoMobileApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public IScanningService ScanningService => DependencyService.Get<IScanningService>();

        public MainViewModel()
        {
            ScanCommand = new Command(OnScan);
        }


        private string scannedText;
        public string ScannedText
        {
            get { return scannedText; }
            set { SetProperty(ref scannedText, value); }
        }

        public Command ScanCommand { get; }

        private async void OnScan()
        {
            try
            {
                var result = await ScanningService.ScanAsync();

                ScannedText = result ?? "Nothing scanned";
                
            } catch (Exception ex)
            {
                throw;
            }
        }
    }
}
