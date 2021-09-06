namespace BeaconSdk.ConsoleApp
{
    public class BeaconViewModel
    {
        private BeaconClient beaconClient;

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