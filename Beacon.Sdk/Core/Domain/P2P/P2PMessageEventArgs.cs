namespace Beacon.Sdk.Core.Domain.P2P
{
    using System;
    using System.Collections.Generic;

    public class P2PMessageEventArgs : EventArgs
    {
        public P2PMessageEventArgs(List<string> messages)
        {
            Messages = messages;
        }

        public List<string> Messages { get; }
    }
}