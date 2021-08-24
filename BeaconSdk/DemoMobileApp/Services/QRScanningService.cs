using System;
using System.Threading.Tasks;
using ZXing.Mobile;

namespace DemoMobileApp.Services
{
    public class QRScanningService : IScanningService
    {
        public async Task<string> ScanAsync()
        {
            var optionsDefault = new MobileBarcodeScanningOptions();
            var optionsCustom = new MobileBarcodeScanningOptions();

            var scanner = new MobileBarcodeScanner()
            {
                TopText = "Scan the QR Code",
                BottomText = "Please Wait",
            };

            var scanResult = await scanner.Scan(optionsCustom);

            if (scanResult == null)
                return "Nothing scanned";

            return scanResult.Text;
        }
    }
}
