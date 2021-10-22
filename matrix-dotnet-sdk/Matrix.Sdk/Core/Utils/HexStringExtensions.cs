namespace Matrix.Sdk.Core.Utils
{
    using System;
    using System.Text;

    public static class HexStringExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static string ToASCII(this HexString hexString)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (hexString.Value == null)
                throw new ArgumentNullException(nameof(hexString));

            var sb = new StringBuilder();
            for (var i = 0; i < hexString.Value.Length; i += 2)
            {
                string hexChar = hexString.Value.Substring(i, 2);
                var k = Convert.ToByte(hexChar, 16);
                var t = (char) k;
                sb.Append(t);
            }

            return sb.ToString();
        }


        // public static HexString FromByteArray(byte[] value) => new HexString(BitConverter.ToString(value));

        public static byte[] ToByteArray(this HexString hexString)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (hexString.Value == null)
                throw new ArgumentNullException(nameof(hexString));

            var bytes = new byte[hexString.Value.Length / 2];

            for (var i = 0; i < hexString.Value.Length; i += 2)
            {
                string hexChar = hexString.Value.Substring(i, 2);
                var @byte = Convert.ToByte(hexChar, 16);

                bytes[i / 2] = @byte;
            }

            return bytes;
        }
    }
}

// ReSharper disable once InconsistentNaming
// private static string ToASCII(string? hexString)
// {
//     if (string.IsNullOrEmpty(hexString))
//         throw new ArgumentNullException(nameof(hexString));
//
//     var sb = new StringBuilder();
//     for (var i = 0; i < hexString.Length; i += 2)
//     {
//         var hexChar = hexString.Substring(i, 2);
//         sb.Append((char)Convert.ToByte(hexChar, 16));
//     }
//
//     return sb.ToString();
// }

// private HexString()
// {
//     
// }

// public static bool TryParse(byte[] input)
// {
//     if (input.Length <= 1)
//     BitConverter.ToString(input);
//     
// }

// public override bool Equals(object obj)
// {
//     if (obj == null)
//         return false;
//
//     var hex = (HexString) obj;
// }