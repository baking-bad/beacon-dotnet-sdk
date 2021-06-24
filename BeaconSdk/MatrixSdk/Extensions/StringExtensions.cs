namespace MatrixSdk.Extensions
{
    using System;

    internal static class StringExtensions
    {
        public static string TruncateLongString(this string str, int maxLength) =>
            string.IsNullOrEmpty(str) ? str : str.Substring(0, Math.Min(str.Length, maxLength));
    }
}