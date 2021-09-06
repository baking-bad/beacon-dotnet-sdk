namespace MatrixSdk.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public readonly ref struct BetterHexString
    {
        private const string Prefix = "0x";
        public readonly ReadOnlySpan<byte> Value;

        private BetterHexString(ReadOnlySpan<byte> value)
        {
            Value = value;
        }

        public static bool TryParse(string? input, out BetterHexString result)
        {
            // stackalloc int
            if (input == null)
            {
                result = new BetterHexString();
                return false;
            }

            if (input.StartsWith(Prefix))
                input = input[2..];

            if (!IsHex(input.ToCharArray()))
            {
                result = CreateFrom(input);
                return true;
            }

            result = new BetterHexString();
            return false;
        }

        private static bool IsHex(IReadOnlyCollection<char> characters) =>
            characters.Count % 2 == 0 &&
            characters.Select(c => c is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F')
                .All(isHex => isHex);

        private static BetterHexString CreateFrom(string value)
        {
            var result = new byte[value.Length / 2];

            for (var i = 0; i < value.Length; i += 2)
            {
                var hexChar = value.Substring(i, 2);
                result[i / 2] = Convert.ToByte(hexChar, 16);
            }

            return new BetterHexString(result);
        }
    }
}