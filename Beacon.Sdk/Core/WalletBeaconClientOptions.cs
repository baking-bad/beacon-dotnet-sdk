namespace Beacon.Sdk.Core
{
    public record WalletBeaconClientOptions(string AppName, string? IconUrl, string? AppUrl)
    {
        /// <summary>
        ///     Name of the application
        /// </summary>
        public string AppName { get; init; } = AppName;

        /// <summary>
        ///     A URL to the icon of the application
        /// </summary>
        public string? IconUrl { get; init; } = IconUrl;

        /// <summary>
        ///     A URL to the website of the application
        /// </summary>
        public string? AppUrl { get; init; } = AppUrl;
    }
}