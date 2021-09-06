namespace BeaconSdk.Infrastructure.Cryptography.Libsodium
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
        
        public static SessionKeyPair CreateServerSessionKeyPair(byte[] serverPublicKey, byte[] serverSecretKey, byte[] clientPublicKey)
        {
            var rx = new byte[SessionKeyBytes];
            var tx = new byte[SessionKeyBytes];

            if (serverPublicKey == null || serverPublicKey.Length != PublicKeyBytes) 
                throw new ArgumentException($"{nameof(serverPublicKey)} size must be {PublicKeyBytes} bytes in length.");
            
            if (serverSecretKey == null || serverSecretKey.Length != SecretKeyBytes) 
                throw new ArgumentException($"{nameof(serverSecretKey)} size must be {SecretKeyBytes} bytes in length.");
            
            if (clientPublicKey == null || clientPublicKey.Length != PublicKeyBytes) 
                throw new ArgumentException($"{nameof(clientPublicKey)} size must be {PublicKeyBytes} bytes in length.");

            if (SodiumLibrary.crypto_kx_server_session_keys(rx, tx, serverPublicKey, serverSecretKey, clientPublicKey) != 0)
                throw new Exception($"{nameof(SodiumLibrary)}: {nameof(KeyExchange)} error.");

            return new SessionKeyPair(rx, tx);
        }
        
        // // A client <-> B server
        // public static void GenerateServerSessionKeyPair(byte[]clientPublicKey, KeyPair serverKeyPair)
        // {
        //     
        // }
        
        // private static SessionKeyPair CreateClientSessionKeyPair(byte[] clientPublicKey, byte[] clientSecretKey, byte[] clientPublicKey1)
        // {
        //     var rx = new byte[SESSION_KEY_BYTES];
        //     var tx = new byte[SESSION_KEY_BYTES];
        //
        //     if (clientPublicKey == null || clientPublicKey.Length != PUBLIC_KEY_BYTES) 
        //         throw new ArgumentException($"{nameof(clientPublicKey)} size must be {PUBLIC_KEY_BYTES} bytes in length.");
        //     
        //     if (clientSecretKey == null || clientSecretKey.Length != SECRET_KEY_BYTES) 
        //         throw new ArgumentException($"{nameof(clientSecretKey)} size must be {SECRET_KEY_BYTES} bytes in length.");
        //     
        //     if (clientPublicKey1 == null || clientPublicKey1.Length != PUBLIC_KEY_BYTES) 
        //         throw new ArgumentException($"{nameof(clientPublicKey1)} size must be {PUBLIC_KEY_BYTES} bytes in length.");
        //
        //     if (SodiumLibrary.crypto_kx_client_session_keys(rx, tx, clientPublicKey, clientSecretKey, clientPublicKey1) != 0)
        //         throw new Exception($"{nameof(SodiumLibrary)}: {nameof(KeyExchange)} error.");
        //
        //     return new SessionKeyPair(rx, tx);
        // }
    }
}