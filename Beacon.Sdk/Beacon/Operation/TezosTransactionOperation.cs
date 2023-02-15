using Newtonsoft.Json.Linq;

namespace Beacon.Sdk.Beacon.Operation
{
    public record PartialTezosTransactionOperation(
            string Amount,
            string Destination,
            JObject? Parameters)
        : TezosBaseOperation(TezosOperationType.transaction), IPartialTezosOperation;
}