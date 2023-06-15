using Netezos.Forging.Models;

namespace Beacon.Sdk.Beacon.Operation
{
    public record PartialTezosOriginationOperation(
            string Balance,
            Script Script,
            string? Delegate)
        : TezosBaseOperation(TezosOperationType.origination), IPartialTezosOperation;
}
