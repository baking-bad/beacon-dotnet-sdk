namespace MatrixSdk.Utils
{

    // public static class BetterHexStringExtensions
    // {
    //     // ReSharper disable once InconsistentNaming
    //     public static string ToASCII(this BetterHexString hexString)
    //     {
    //         // ReSharper disable once ConditionIsAlwaysTrueOrFalse
    //         if (hexString.Value == null)
    //             throw new ArgumentNullException(nameof(hexString));
    //
    //         var sb = new StringBuilder();
    //         for (var i = 0; i < hexString.Value.Length; i += 2)
    //         {
    //             var hexChar = hexString.Value.Substring(i, 2);
    //             var k = Convert.ToByte(hexChar, 16);
    //             var t = (char)k;
    //             sb.Append(t);
    //         }
    //
    //         return sb.ToString();
    //     }
    //
    //
    //     // public static HexString FromByteArray(byte[] value) => new HexString(BitConverter.ToString(value));
    //
    //     public static byte[] ToByteArray(this HexString hexString)
    //     {
    //         // ReSharper disable once ConditionIsAlwaysTrueOrFalse
    //         if (hexString.Value == null)
    //             throw new ArgumentNullException(nameof(hexString));
    //
    //         var bytes = new byte[hexString.Value.Length / 2];
    //
    //         for (var i = 0; i < hexString.Value.Length; i += 2)
    //         {
    //             var hexChar = hexString.Value.Substring(i, 2);
    //             var @byte = Convert.ToByte(hexChar, 16);
    //
    //             bytes[i / 2] = @byte;
    //         }
    //
    //         return bytes;
    //     }
    // }
}