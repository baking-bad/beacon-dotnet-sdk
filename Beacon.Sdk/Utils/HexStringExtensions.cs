namespace Beacon.Sdk.Utils
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
                var b = Convert.ToByte(hexChar, 16);
                var c = (char) b;
                sb.Append(c);
            }

            return sb.ToString();
        }

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