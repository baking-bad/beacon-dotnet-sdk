namespace Beacon.Sdk.Tests
{
    using Core.Infrastructure.Cryptography;
    using Core.Infrastructure.Cryptography.NaCl;
    using Xunit;

    public class CryptoTests
    {
        private byte[] randomPubKeyBytes { get; } =
        {
            0x58, 0x96, 0xB4, 0xDE, 0x36, 0x93, 0x5F, 0x5, 0x79, 0x4E, 0xB8, 0x7, 0x3F, 0xB1, 0x7F, 0xE6, 0xAD,
            0x64, 0xFF, 0xE3, 0x48, 0x60, 0x81, 0x1C, 0x9D, 0xDF, 0xB9, 0xE4, 0xF3, 0x2F, 0xF7, 0x5B,
        };

        private byte[] convertedPubKeyBytes { get; } =
        {
            0x50, 0x94, 0xD3, 0xF3, 0x88, 0xFD, 0x19, 0xF3, 0x3D, 0xBF, 0xA7, 0x3A, 0x6C, 0x9E, 0xCD, 0x80, 0x7C,
            0x1C, 0x92, 0x74, 0xE3, 0xA0, 0x71, 0x8F, 0xEC, 0xB8, 0xDD, 0x8D, 0x3E, 0x78, 0x66, 0x15
        };

        [Fact]
        public void TestSodiumConvertEd25519PublicKeyToCurve25519PublicKey()
        {
            var actual = PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(randomPubKeyBytes);
            Assert.Equal(convertedPubKeyBytes, actual);
        }

        [Fact]
        public void TestConvertEd25519PublicKeyToCurve25519PublicKey()
        {
            var actual = new byte[32];
            MontgomeryCurve25519.EdwardsToMontgomery(actual, randomPubKeyBytes);
            Assert.Equal(convertedPubKeyBytes, actual);
        }
    }
}