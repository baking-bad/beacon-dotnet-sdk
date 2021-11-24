namespace Beacon.Sdk
{
    public class WalletBeaconClientOptions
    {
        /// <summary>
        ///     Name of the application
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        ///     A URL to the icon of the application
        /// </summary>
        public string? IconUrl { get; set; }

        /// <summary>
        ///     A URL to the website of the application
        /// </summary>
        public string? AppUrl { get; set; }
    }
}