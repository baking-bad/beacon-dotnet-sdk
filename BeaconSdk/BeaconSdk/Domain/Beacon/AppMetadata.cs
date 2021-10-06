// ReSharper disable InconsistentNaming

namespace BeaconSdk.Domain.Beacon
{

    public record AppMetadata(
        string SenderId,
        string Name,
        string? Icon
    );

}