namespace Beacon.Sdk.Beacon.Operation
{
    using System.Collections.Generic;
    using Core.Domain;
    using Netezos.Forging.Models;
    using Permission;

    public record OperationRequest(
            BeaconMessageType Type,
            string Version,
            string Id,
            string SenderId,
            Network Network,
            List<PartialTezosTransactionOperation> OperationDetails,
            string SourceAddress)
        : BeaconBaseMessage(
            Type,
            Version,
            Id,
            SenderId), IBeaconRequest
    {
        /// <summary>
        ///     Network on which the operation will be broadcast
        /// </summary>
        public Network Network { get; } = Network;

        public string SourceAddress { get; } = SourceAddress;
    }
}

