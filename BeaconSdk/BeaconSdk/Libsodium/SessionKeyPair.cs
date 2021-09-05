namespace BeaconSdk.Libsodium
{
    using System;

    public struct SessionKeyPair
    {
        public readonly byte[] Rx;
        public readonly byte[] Tx;
        
        public SessionKeyPair(byte[] rx, byte[] tx)
        {
            Rx = rx;
            Tx = tx;
        }
    }
}