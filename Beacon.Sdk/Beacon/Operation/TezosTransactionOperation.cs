using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Beacon.Sdk.Beacon.Operation
{
    public record PartialTezosTransactionOperation : TezosBaseOperation, IPartialTezosOperation
    {
        public string Amount { get; }
        public string Destination { get; }
        public JToken? Parameters { get; }

        [JsonConstructor]
        public PartialTezosTransactionOperation(
            string amount,
            string destination,
            JToken? parameters) : base(
            TezosOperationType.transaction)
        {
            Amount = amount;
            Destination = destination;
            Parameters = parameters;
        }

        public PartialTezosTransactionOperation(
            string amount,
            string destination,
            string parameters) : base(
            TezosOperationType.transaction)
        {
            Amount = amount;
            Destination = destination;
            Parameters = string.IsNullOrEmpty(parameters) ? null : JToken.Parse(parameters);
        }
    }
}