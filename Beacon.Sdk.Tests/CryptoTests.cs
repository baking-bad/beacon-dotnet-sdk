namespace Beacon.Sdk.Tests
{
    using System.Text;
    using System;
    using Core.Infrastructure.Cryptography;
    using Core.Infrastructure.Cryptography.NaCl;
    using Core.Infrastructure.Cryptography.BouncyCastle;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CryptoTests
    {
        const int SeedSize = 32;

        private byte[] RandomPubKeyBytes { get; } =
        {
            0x58, 0x96, 0xB4, 0xDE, 0x36, 0x93, 0x5F, 0x5, 0x79, 0x4E, 0xB8, 0x7, 0x3F, 0xB1, 0x7F, 0xE6, 0xAD,
            0x64, 0xFF, 0xE3, 0x48, 0x60, 0x81, 0x1C, 0x9D, 0xDF, 0xB9, 0xE4, 0xF3, 0x2F, 0xF7, 0x5B,
        };

        private byte[] ConvertedPubKeyBytes { get; } =
        {
            0x50, 0x94, 0xD3, 0xF3, 0x88, 0xFD, 0x19, 0xF3, 0x3D, 0xBF, 0xA7, 0x3A, 0x6C, 0x9E, 0xCD, 0x80, 0x7C,
            0x1C, 0x92, 0x74, 0xE3, 0xA0, 0x71, 0x8F, 0xEC, 0xB8, 0xDD, 0x8D, 0x3E, 0x78, 0x66, 0x15
        };

        [TestMethod]
        public void TestSodiumConvertEd25519PublicKeyToCurve25519PublicKey()
        {
            var actual = PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(RandomPubKeyBytes);
            CollectionAssert.AreEqual(ConvertedPubKeyBytes, actual);
        }

        [TestMethod]
        public void TestConvertEd25519PublicKeyToCurve25519PublicKey()
        {
            var actual = new byte[32];
            MontgomeryCurve25519.EdwardsToMontgomery(actual, RandomPubKeyBytes);
            CollectionAssert.AreEqual(ConvertedPubKeyBytes, actual);
        }

        [TestMethod]
        public void CanEncryptAndDecryptBySecretBox()
        {
            var random = new Random(Seed:0);
            var key = random.GetBytes(32);
            var nonce = random.GetBytes(24);
            var message = Encoding.UTF8.GetBytes("Test message for secret box");

            var cipher = SecretBox.Create(message, nonce, key);

            var decrypted = SecretBox.Open(cipher, nonce, key);

            CollectionAssert.AreEqual(message, decrypted);
        }

        [TestMethod]
        public void CanEncryptAndDecryptBySealedSecretBox()
        {
            var random = new Random(Seed: 0);
            var seed = random.GetBytes(SeedSize);

            var ed25519keyPair = PublicKeyAuth.GenerateKeyPair(seed);

            var curve25519sk = Ed25519Extensions.ConvertEd25519SecretKeyToCurve25519SecretKey(ed25519keyPair.PrivateKey);
            var curve25519pk = Ed25519Extensions.ConvertEd25519PublicKeyToCurve25519PublicKey(ed25519keyPair.PublicKey);

            var message = Encoding.UTF8.GetBytes("Test message for secret box");

            var cipher = SealedPublicKeyBox.Create(message, curve25519pk);

            var decrypted = SealedPublicKeyBox.Open(cipher, curve25519sk, curve25519pk);

            CollectionAssert.AreEqual(message, decrypted);
        }

        [TestMethod]
        public void CanGenerateKeyPairLikeSodium()
        {
            Sodium.Initialize();
            var random = new Random(Seed: 0);

            const int Iterations = 10000;

            for (var i = 0; i < Iterations; ++i)
            {
                var seed = random.GetBytes(SeedSize);

                var keyPair = PublicKeyAuth.GenerateKeyPair(seed);

                Assert.IsNotNull(keyPair);

                var sodiumPublicKey = new byte[Sodium.crypto_sign_ed25519_PUBLICKEYBYTES];
                var sodiumPrivateKey = new byte[Sodium.crypto_sign_ed25519_SECRETKEYBYTES];

                var _ = Sodium.crypto_sign_ed25519_seed_keypair(sodiumPublicKey, sodiumPrivateKey, seed);

                CollectionAssert.AreEqual(sodiumPrivateKey, keyPair.PrivateKey);
                CollectionAssert.AreEqual(sodiumPublicKey, keyPair.PublicKey);
            }
        }

        [TestMethod]
        public void CanConvertEd25519PublicKeyToCurve25519PublicKeyLikeSodium()
        {
            Sodium.Initialize();
            var random = new Random(Seed: 0);

            const int Iterations = 10000;

            for (var i = 0; i < Iterations; ++i)
            {
                var seed = random.GetBytes(SeedSize);
                var keyPair = PublicKeyAuth.GenerateKeyPair(seed);
                var curve25519PublicKey = Ed25519Extensions.ConvertEd25519PublicKeyToCurve25519PublicKey(keyPair.PublicKey);

                Assert.IsNotNull(curve25519PublicKey);

                var buffer = new byte[Sodium.crypto_scalarmult_curve25519_BYTES];
                var _ = Sodium.crypto_sign_ed25519_pk_to_curve25519(buffer, keyPair.PublicKey);

                CollectionAssert.AreEqual(buffer, curve25519PublicKey);
            }
        }

        [TestMethod]
        public void CanConvertEd25519PrivateKeyToCurve25519PrivateKeyLikeSodium()
        {
            Sodium.Initialize();
            var random = new Random(Seed: 0);

            const int Iterations = 10000;

            for (var i = 0; i < Iterations; ++i)
            {
                var seed = random.GetBytes(SeedSize);
                var keyPair = PublicKeyAuth.GenerateKeyPair(seed);
                var curve25519PrivateKey = Ed25519Extensions.ConvertEd25519SecretKeyToCurve25519SecretKey(keyPair.PrivateKey);

                Assert.IsNotNull(curve25519PrivateKey);

                var buffer = new byte[Sodium.crypto_scalarmult_curve25519_SCALARBYTES];
                var _ = Sodium.crypto_sign_ed25519_sk_to_curve25519(buffer, keyPair.PrivateKey);

                CollectionAssert.AreEqual(buffer, curve25519PrivateKey);
            }
        }

        [TestMethod]
        public void CanCalculateGenericHashLikeSodium()
        {
            Sodium.Initialize();
            var random = new Random(Seed: 0);

            for (var messageSizeInBytes = 0; messageSizeInBytes < 1024; ++messageSizeInBytes)
            {
                for (var hashSizeInBytes = 1; hashSizeInBytes < 32; ++hashSizeInBytes)
                {
                    var message = random.GetBytes(messageSizeInBytes);
                    var hash = GenericHash.Hash(message, hashSizeInBytes);

                    Assert.IsNotNull(hash);

                    var buffer = new byte[hashSizeInBytes];
                    var _ = Sodium.crypto_generichash(buffer, hashSizeInBytes, message, (ulong)messageSizeInBytes, Array.Empty<byte>(), 0);

                    CollectionAssert.AreEqual(buffer, hash);
                }
            }
        }

        [TestMethod]
        public void CanCreateClientSessionKeyPairLikeSodium()
        {
            Sodium.Initialize();
            var random = new Random(Seed: 0);

            const int Iterations = 10000;

            for (var i = 0; i < Iterations; ++i)
            {
                var serverSeed = random.GetBytes(SeedSize);
                var clientSeed = random.GetBytes(SeedSize);

                var serverKeyPair = PublicKeyAuth.GenerateKeyPair(serverSeed);
                var serverPublicKey = Ed25519Extensions.ConvertEd25519PublicKeyToCurve25519PublicKey(serverKeyPair.PublicKey);
                //var serverPrivateKey = Ed25519Extensions.ConvertEd25519SecretKeyToCurve25519SecretKey(serverKeyPair.PrivateKey);

                var clientKeyPair = PublicKeyAuth.GenerateKeyPair(clientSeed);
                var clientPublicKey = Ed25519Extensions.ConvertEd25519PublicKeyToCurve25519PublicKey(clientKeyPair.PublicKey);
                var clientPrivateKey = Ed25519Extensions.ConvertEd25519SecretKeyToCurve25519SecretKey(clientKeyPair.PrivateKey);

                var sessionKeyPair = KeyExchange.CreateClientSessionKeyPair(clientPublicKey, clientPrivateKey, serverPublicKey);

                var rx = new byte[32];
                var tx = new byte[32];

                var _ = Sodium.crypto_kx_client_session_keys(rx, tx, clientPublicKey, clientPrivateKey, serverPublicKey);

                var sodiumSessionKeyPair = new SessionKeyPair(rx, tx);

                CollectionAssert.AreEqual(sodiumSessionKeyPair.Rx, sessionKeyPair.Rx);
                CollectionAssert.AreEqual(sodiumSessionKeyPair.Tx, sessionKeyPair.Tx);
            }
        }

        [TestMethod]
        public void CanCreateServerSessionKeyPairLikeSodium()
        {
            Sodium.Initialize();
            var random = new Random(Seed: 0);

            const int Iterations = 10000;

            for (var i = 0; i < Iterations; ++i)
            {
                var serverSeed = random.GetBytes(SeedSize);
                var clientSeed = random.GetBytes(SeedSize);

                var serverKeyPair = PublicKeyAuth.GenerateKeyPair(serverSeed);
                var serverPublicKey = Ed25519Extensions.ConvertEd25519PublicKeyToCurve25519PublicKey(serverKeyPair.PublicKey);
                var serverPrivateKey = Ed25519Extensions.ConvertEd25519SecretKeyToCurve25519SecretKey(serverKeyPair.PrivateKey);

                var clientKeyPair = PublicKeyAuth.GenerateKeyPair(clientSeed);
                var clientPublicKey = Ed25519Extensions.ConvertEd25519PublicKeyToCurve25519PublicKey(clientKeyPair.PublicKey);
                //var clientPrivateKey = Ed25519Extensions.ConvertEd25519SecretKeyToCurve25519SecretKey(clientKeyPair.PrivateKey);

                var sessionKeyPair = KeyExchange.CreateServerSessionKeyPair(serverPublicKey, serverPrivateKey, clientPublicKey);

                var rx = new byte[32];
                var tx = new byte[32];

                var _ = Sodium.crypto_kx_server_session_keys(rx, tx, serverPublicKey, serverPrivateKey, clientPublicKey);

                var sodiumSessionKeyPair = new SessionKeyPair(rx, tx);

                CollectionAssert.AreEqual(sodiumSessionKeyPair.Rx, sessionKeyPair.Rx);
                CollectionAssert.AreEqual(sodiumSessionKeyPair.Tx, sessionKeyPair.Tx);
            }
        }

        [TestMethod]
        public void CanCreateAndOpenSealedPublicKeyBoxLikeSodium()
        {
            const int PublicKeyBytes = 32;
            const int MacBytes = 16;

            Sodium.Initialize();
            var random = new Random(Seed: 0);

            const int Iterations = 10;

            for (var i = 0; i < Iterations; ++i)
            {
                var seed = random.GetBytes(SeedSize);
                var keyPair = PublicKeyAuth.GenerateKeyPair(seed);
                var publicKey = Ed25519Extensions.ConvertEd25519PublicKeyToCurve25519PublicKey(keyPair.PublicKey);
                var privateKey = Ed25519Extensions.ConvertEd25519SecretKeyToCurve25519SecretKey(keyPair.PrivateKey);

                for (var messageSizeInBytes = 0; messageSizeInBytes < 1024; ++messageSizeInBytes)
                {
                    var message = random.GetBytes(messageSizeInBytes);

                    // create box
                    var cipher = SealedPublicKeyBox.Create(message, publicKey);
                    Assert.IsNotNull(cipher);

                    var sodiumCipher = new byte[messageSizeInBytes + PublicKeyBytes + MacBytes];
                    _ = Sodium.crypto_box_seal(sodiumCipher, message, (ulong)message.Length, publicKey);

                    // cross open box

                    // try open sodium cipher by NaCl implementation
                    var openedMessage = SealedPublicKeyBox.Open(sodiumCipher, privateKey, publicKey);

                    // try open NaCl cipher by sodium
                    var sodiumOpenedMessage = new byte[sodiumCipher.Length - PublicKeyBytes - MacBytes];
                    _ = Sodium.crypto_box_seal_open(sodiumOpenedMessage, cipher, (ulong)cipher.Length, publicKey, privateKey);

                    CollectionAssert.AreEqual(message, openedMessage);
                    CollectionAssert.AreEqual(sodiumOpenedMessage, openedMessage);
                }
            }
        }
    }
}