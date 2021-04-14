namespace BeaconSdk.ConsoleApp
{
    using Sodium;

    public interface ICryptoAlgorithmsProvider
    {
        byte[] GenerateRandomBytes(int count);

        byte[] Hash(string message, int size);

        byte[] Hash(byte[] message, int size);

        KeyPair GenerateEd25519KeyPair(byte[] seed);

        byte[] SignDetached(byte[] message, byte[] key);
    }
}