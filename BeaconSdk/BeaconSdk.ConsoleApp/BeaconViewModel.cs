namespace BeaconSdk.ConsoleApp
{
    public class BeaconViewModel
    {
        private BaseClient _baseClient;

        private void StartBeacon()
        {
            // On success start
            ListenForRequests();

            // On failure 
            // Log error
        }

        private void ListenForRequests()
        {
        }
    }
}