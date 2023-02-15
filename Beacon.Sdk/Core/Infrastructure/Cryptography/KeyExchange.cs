namespace Beacon.Sdk.Core.Infrastructure.Cryptography
{
    using System;
    using Org.BouncyCastle.Crypto.Digests;
    using Org.BouncyCastle.Math.EC.Rfc7748;

    public static class KeyExchange
    {
        // crypto_kx_PUBLICKEYBYTES
        private const int PublicKeyBytes = 32;

        // crypto_kx_SECRETKEYBYTES
        private const int SecretKeyBytes = 32;

        // crypto_kx_SESSIONKEYBYTES
        private const int SessionKeyBytes = 32;

        private const int ScalarMultCurve25519Bytes = 32;

        public static SessionKeyPair CreateClientSessionKeyPair(
            byte[] clientPublicKey,
            byte[] clientSecretKey,
            byte[] serverPublicKey)
        {
            var rx = new byte[SessionKeyBytes];
            var tx = new byte[SessionKeyBytes];

            if (clientPublicKey is not { Length: PublicKeyBytes })
                throw new ArgumentException(
                    $"{nameof(clientPublicKey)} size must be {PublicKeyBytes} bytes in length.");

            if (clientSecretKey is not { Length: SecretKeyBytes })
                throw new ArgumentException(
                    $"{nameof(clientSecretKey)} size must be {SecretKeyBytes} bytes in length.");

            if (serverPublicKey is not { Length: PublicKeyBytes })
                throw new ArgumentException(
                    $"{nameof(serverPublicKey)} size must be {PublicKeyBytes} bytes in length.");

            var q = new byte[ScalarMultCurve25519Bytes];
            X25519.ScalarMult(clientSecretKey, 0, serverPublicKey, 0, q, 0);

            var h = new byte[2 * SessionKeyBytes];

            var blake2bDigest = new Blake2bDigest(h.Length * 8);
            blake2bDigest.BlockUpdate(q, 0, q.Length);
            blake2bDigest.BlockUpdate(clientPublicKey, 0, clientPublicKey.Length);
            blake2bDigest.BlockUpdate(serverPublicKey, 0, serverPublicKey.Length);
            blake2bDigest.DoFinal(h, 0);

            Buffer.BlockCopy(h, 0, rx, 0, SessionKeyBytes);
            Buffer.BlockCopy(h, SessionKeyBytes, tx, 0, SessionKeyBytes);

            return new SessionKeyPair(rx, tx);
        }

        public static SessionKeyPair CreateServerSessionKeyPair(
            byte[] serverPublicKey,
            byte[] serverSecretKey,
            byte[] clientPublicKey)
        {
            var rx = new byte[SessionKeyBytes];
            var tx = new byte[SessionKeyBytes];

            if (serverPublicKey is not { Length: PublicKeyBytes })
                throw new ArgumentException(
                    $"{nameof(serverPublicKey)} size must be {PublicKeyBytes} bytes in length.");

            if (serverSecretKey is not { Length: SecretKeyBytes })
                throw new ArgumentException(
                    $"{nameof(serverSecretKey)} size must be {SecretKeyBytes} bytes in length.");

            if (clientPublicKey is not { Length: PublicKeyBytes })
                throw new ArgumentException(
                    $"{nameof(clientPublicKey)} size must be {PublicKeyBytes} bytes in length.");

            var q = new byte[ScalarMultCurve25519Bytes];
            X25519.ScalarMult(serverSecretKey, 0, clientPublicKey, 0, q, 0);

            var h = new byte[2 * SessionKeyBytes];

            var blake2bDigest = new Blake2bDigest(h.Length * 8);
            blake2bDigest.BlockUpdate(q, 0, q.Length);
            blake2bDigest.BlockUpdate(clientPublicKey, 0, clientPublicKey.Length);
            blake2bDigest.BlockUpdate(serverPublicKey, 0, serverPublicKey.Length);
            blake2bDigest.DoFinal(h, 0);

            Buffer.BlockCopy(h, 0, tx, 0, SessionKeyBytes);
            Buffer.BlockCopy(h, SessionKeyBytes, rx, 0, SessionKeyBytes);

            return new SessionKeyPair(rx, tx);
        }
    }
}