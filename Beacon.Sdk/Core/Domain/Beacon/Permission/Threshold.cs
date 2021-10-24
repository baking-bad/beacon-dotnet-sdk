namespace Beacon.Sdk.Core.Domain.Beacon.Permission
{
    public record Threshold(
        string Amount, // The amount of mutez that can be spent within the timeframe
        string Timeframe // The timeframe within which the spending will be summed up
    );
}