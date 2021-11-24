namespace Beacon.Sdk.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public record HexString
    {
        private const string Prefix = "0x";
        public readonly string Value;

        public HexString(string value)
        {
            Value = value;
        }

        public static bool TryParse(byte[] input, out HexString result)
        {
            if (input.Length <= 1)
            {
                result = new HexString(string.Empty);
                return false;
            }

            string value = BitConverter.ToString(input)
                .Replace("-", string.Empty)
                .ToLower();

            result = new HexString(value);
            return true;
        }

        public static bool TryParse(string? input, out HexString result)
        {
            if (input == null)
            {
                result = new HexString(string.Empty);
                return false;
            }

            if (input.StartsWith(Prefix))
                input = input[2..];

            if (IsHex(input.ToCharArray()))
            {
                result = new HexString(input);
                return true;
            }

            result = new HexString(string.Empty);
            return false;
        }

        public override string ToString() => Value;

        public string ToString(bool withPrefix) => withPrefix ? Prefix + Value : Value;

        private static bool IsHex(IReadOnlyCollection<char> characters) =>
            characters.Count % 2 == 0 &&
            characters.Select(c => c is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F')
                .All(isHex => isHex);
    }
}