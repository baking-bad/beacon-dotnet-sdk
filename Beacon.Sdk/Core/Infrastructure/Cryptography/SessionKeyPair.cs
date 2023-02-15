namespace Beacon.Sdk.Core.Infrastructure.Cryptography
{
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