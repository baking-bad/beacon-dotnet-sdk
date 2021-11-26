namespace Beacon.Sdk.Beacon.Permission
{
    /// <summary>
    /// The threshold is not enforced on the dapp side. It's only as an information to the user
    /// </summary>
    public record Threshold(string Amount, string Timeframe)
    {
        /// <summary>
        /// The amount of mutez that can be spent within the timeframe
        /// </summary>
        public string Amount { get; } = Amount;

        /// <summary>
        /// The timeframe within which the spending will be summed up
        /// </summary>
        public string Timeframe { get; } = Timeframe;
    }
}