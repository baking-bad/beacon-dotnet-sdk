// ReSharper disable InconsistentNaming

namespace Beacon.Sdk.Core.Domain.Beacon
{
    public record AppMetadata(
        string SenderId,
        string Name,
        string? Icon
    );
}