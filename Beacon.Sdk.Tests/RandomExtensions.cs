using System;

namespace Beacon.Sdk.Tests
{
    internal static class RandomExtensions
    {
        internal static byte[] GetBytes(this Random random, int count)
        {
            var buffer = new byte[count];

            random.NextBytes(buffer);

            return buffer;
        }
    }
}