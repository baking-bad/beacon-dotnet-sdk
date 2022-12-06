namespace Beacon.Sdk.Core.Infrastructure.Cryptography.Libsodium
{
    using System;

    public static class KeyExchange
    {
        // crypto_kx_PUBLICKEYBYTES
        private const int PublicKeyBytes = 32;

        // crypto_kx_SECRETKEYBYTES
        private const int SecretKeyBytes = 32;

        // crypto_kx_SESSIONKEYBYTES
        private const int SessionKeyBytes = 32;

        public static SessionKeyPair CreateClientSessionKeyPair(byte[] serverPublicKey, byte[] serverSecretKey,
            byte[] clientPublicKey)
        {
            var rx = new byte[SessionKeyBytes];
            var tx = new byte[SessionKeyBytes];

            if (serverPublicKey is not {Length: PublicKeyBytes})
                throw new ArgumentException(
                    $"{nameof(serverPublicKey)} size must be {PublicKeyBytes} bytes in length.");

            if (serverSecretKey is not {Length: SecretKeyBytes})
                throw new ArgumentException(
                    $"{nameof(serverSecretKey)} size must be {SecretKeyBytes} bytes in length.");

            if (clientPublicKey is not {Length: PublicKeyBytes})
                throw new ArgumentException(
                    $"{nameof(clientPublicKey)} size must be {PublicKeyBytes} bytes in length.");

            if (Sodium.CryptoKxClientSessionKeys(
                    rx,
                    tx,
                    serverPublicKey,
                    serverSecretKey,
                    clientPublicKey) != 0)
            {
                throw new Exception($"{nameof(Sodium)}: {nameof(KeyExchange)} error.");
            }

            return new SessionKeyPair(rx, tx);
        }

        public static SessionKeyPair CreateServerSessionKeyPair(byte[] serverPublicKey, byte[] serverSecretKey,
            byte[] clientPublicKey)
        {
            var rx = new byte[SessionKeyBytes];
            var tx = new byte[SessionKeyBytes];

            if (serverPublicKey is not {Length: PublicKeyBytes})
                throw new ArgumentException(
                    $"{nameof(serverPublicKey)} size must be {PublicKeyBytes} bytes in length.");

            if (serverSecretKey is not {Length: SecretKeyBytes})
                throw new ArgumentException(
                    $"{nameof(serverSecretKey)} size must be {SecretKeyBytes} bytes in length.");

            if (clientPublicKey is not {Length: PublicKeyBytes})
                throw new ArgumentException(
                    $"{nameof(clientPublicKey)} size must be {PublicKeyBytes} bytes in length.");

            if (Sodium.CryptoKxServerSessionKeys(
                    rx,
                    tx,
                    serverPublicKey,
                    serverSecretKey,
                    clientPublicKey) != 0)
            {
                throw new Exception($"{nameof(Sodium)}: {nameof(KeyExchange)} error.");
            }

            return new SessionKeyPair(rx, tx);
        }
    }
}