namespace Beacon.Sdk.WalletBeaconClient
{
    public class BeaconOptions
    {
        private readonly string[]? _knownRelayServers;

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

        /// <summary>
        ///     Optional. You can directly pass Matrix nodes addresses.
        /// </summary>
        public string[] KnownRelayServers
        {
            get => _knownRelayServers ?? Constants.KnownRelayServers;
            init => _knownRelayServers = value;
        }
    }
}