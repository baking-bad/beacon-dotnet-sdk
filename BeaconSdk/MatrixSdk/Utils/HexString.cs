namespace MatrixSdk.Utils
{
    using System;
    using System.Text;

    public readonly struct HexString
    {
        private const string Prefix = "0x";
        public readonly string Value;
     
        private HexString(string value)
        {
            Value = value;
        }

        // public static bool TryParse(byte[] input)
        // {
        //     if (input.Length <= 1)
        //     BitConverter.ToString(input);
        //     
        // }
        
        public static bool TryParse(string? input, out HexString result)
        {
            if (input == null)
            {
                result = new HexString(string.Empty);
                return false;
            }
            
            if (input.StartsWith(Prefix))
                input = input.Substring(2);
            
            if (IsHex(input.ToCharArray()))
            {
                result = new HexString(input);
                return true;
            }

            result = new HexString(string.Empty);
            return false;
        }

        private static bool IsHex(char[] characters)
        {
            foreach (var c in characters)
            {
                var isHex = c is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F';

                if (!isHex)
                    return false;
            }

            return true;
        }

        public override string ToString() => Value;
    }

    public static class HexStringExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static string? ToASCII(this HexString hexString)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (hexString.Value == null)
                throw new ArgumentNullException(nameof(hexString));

            var sb = new StringBuilder();
            for (var i = 0; i < hexString.Value.Length; i += 2)
            {
                var hexChar = hexString.Value.Substring(i, 2);
                sb.Append((char)Convert.ToByte(hexChar, 16));
            }

            return sb.ToString();
        }
        // public static HexString FromByteArray(byte[] value) => new HexString(BitConverter.ToString(value));
        
        public static byte[]? ToByteArray(this HexString hexString)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (hexString.Value == null) 
                throw new ArgumentNullException(nameof(hexString));
            
            var bytes = new byte[hexString.Value.Length / 2];
        //     // for (var i = 0; i < value.Length; i += 2)
        //     // {
        //     //     var hexChar = value.Substring(i, 2);
        //     //     bytes.
        //     //         sb.Append((char)Convert.ToByte(hexChar, 16));
        //     // }
        //     
        //     
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