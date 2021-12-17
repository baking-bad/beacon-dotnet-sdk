namespace Beacon.Sdk.Beacon.Operation
{
    using System.Collections.Generic;
    using Core.Domain;
    using Permission;

    public record OperationRequest : BaseBeaconMessage, IBeaconRequest
    {
        public OperationRequest(
            BeaconMessageType type,
            string version,
            string id,
            string senderId,
            Network network,
            List<PartialTezosTransactionOperation> operationDetails,
            string sourceAddress)
            : base(type, version, id, senderId)
        {
            Network = network;
            OperationDetails = operationDetails;
            SourceAddress = sourceAddress;
        }

        /// <summary>
        ///     Network on which the operation will be broadcast
        /// </summary>
        public Network Network { get; }

        public List<PartialTezosTransactionOperation> OperationDetails { get; }

        public string SourceAddress { get; }
    }
}