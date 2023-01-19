using System.Collections.Generic;
using Beacon.Sdk.Beacon.Permission;

namespace Beacon.Sdk.Beacon.Operation
{
    public record OperationRequest : BaseBeaconMessage
    {
        public OperationRequest(
            BeaconMessageType type,
            string version,
            string id,
            string senderId,
            Network network,
            List<TezosBaseOperation> operationDetails,
            string sourceAddress
        ) : base(type, version, id, senderId)
        {
            Network = network;
            OperationDetails = operationDetails;
            SourceAddress = sourceAddress;
        }

        /// <summary>
        ///     Network on which the operation will be broadcast
        /// </summary>
        public Network Network { get; }

        public List<TezosBaseOperation> OperationDetails { get; set; }

        public string SourceAddress { get; }
    }
}