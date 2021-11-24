namespace Beacon.Sdk.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Utils;

    [TestClass]
    public class HexStringTests
    {
        [TestMethod]
        public void FromStringToHexString_ValidASCII()
        {
            const string a1 = "0xFAFA";
            _ = HexString.TryParse(a1, out HexString r1);
            Assert.AreEqual(r1.ToASCII(), "úú");

            const string a2 = "48656c6c6f20576f726c6421";
            _ = HexString.TryParse(a2, out HexString r2);
            Assert.AreEqual(r2.ToASCII(), "Hello World!");

            const string a3 = "0xFAF";
            _ = HexString.TryParse(a3, out HexString r3);
            Assert.AreEqual(r3.ToString(), string.Empty);
        }

        [TestMethod]
        public void EmptyHesStringValue_ToASCII_ReturnsNull()
        {
            // var hexString = new HexString();
            // Assert.AreEqual(hexString.ToASCII(),null );
        }
    }
}