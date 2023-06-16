using Netezos.Forging.Models;

namespace Beacon.Sdk.Beacon.Operation
{
    using Newtonsoft.Json.Linq;

    public record PartialTezosOriginationOperation(
            string Balance,
            JObject Script,
            string? Delegate)
        : TezosBaseOperation(TezosOperationType.origination), IPartialTezosOperation;
}
