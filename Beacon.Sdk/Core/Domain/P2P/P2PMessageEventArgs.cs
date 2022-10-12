namespace Beacon.Sdk.Core.Domain.P2P
{
    using System;
    using System.Collections.Generic;
    using Dto.Handshake;

    public class P2PMessageEventArgs : EventArgs
    {
        public P2PMessageEventArgs(List<string> messages, P2PPairingResponse? pairingResponse = null)
        {
            Messages = messages;
            PairingResponse = pairingResponse;
        }

        public List<string> Messages { get; }
        public P2PPairingResponse? PairingResponse { get; }
    }
}