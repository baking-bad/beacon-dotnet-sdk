namespace Beacon.Sdk.Beacon.Operation
{
    using Newtonsoft.Json.Linq;

    public record PartialTezosOriginationOperation(
            string Balance,
            JToken Script,
            string? Delegate)
        : TezosBaseOperation(TezosOperationType.origination), IPartialTezosOperation;
}