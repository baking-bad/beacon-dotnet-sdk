namespace Beacon.Sdk.Core.Domain.Services
{
    using System.Collections.Generic;
    using System.Text;
    using Beacon.Permission;
    using Interfaces;
    using Netezos.Encoding;

    public class AccountService
    {
        private readonly ICryptographyService _cryptographyService;

        public AccountService(ICryptographyService cryptographyService)
        {
            _cryptographyService = cryptographyService;
        }

        public string GetAccountId(string address, Network network)
        {
            var data = new List<string> {address, network.Type.ToString()};

            if (network.Name != null)
                data.Add($"name:{network.Name}");

            if (network.RpcUrl != null)
                data.Add($"rpc:{network.RpcUrl}");

            var m = string.Join("-", data);

            byte[] h = _cryptographyService.Hash(Encoding.UTF8.GetBytes(m), 10);

            return Base58.Convert(h);
        }
    }
}

// private record KeyCurve(string CurveName, int PublicKeyLength, byte[] Prefix);
// public string GetAddressFromPublicKey(string publicKey)
// {
//     // tz1
//     var edpk = new KeyCurve("edpk",54, new byte[] {6, 161, 159});
//
//     // tz2
//     var sppk = new KeyCurve("sppk",55, new byte[] {6, 161, 161});
//
//     // tz3
//     var p2pk = new KeyCurve("p2pk",55, new byte[] {6, 161, 164});
//
//     byte[]? prefix = null;
//     string? plainPublicKey = null;
//
//     if (publicKey.Length == 64)
//     {
//         prefix = edpk.Prefix;
//         plainPublicKey = publicKey;
//     }
//     else
//     {
//         foreach (KeyCurve keyCurve in new List<KeyCurve> {edpk, sppk, p2pk})
//         {
//             if (publicKey.StartsWith(keyCurve.CurveName) && publicKey.Length == keyCurve.PublicKeyLength)
//             {
//                 prefix = keyCurve.Prefix;
//                 
//                 byte[] decoded = Base58.Parse(publicKey);
//                 plainPublicKey = Encoding.UTF8.GetString(decoded[keyCurve.PublicKeyLength..decoded.Length]);
//                 
//                 break;
//             }
//         }
//     }
//
//
//     if (prefix == null || plainPublicKey == null)
//         throw new Exception($"Invalid publicKey: {plainPublicKey}");
//
//     if (!HexString.TryParse(plainPublicKey, out HexString plainHexPublicKey))
//         throw new ArgumentException(nameof(plainPublicKey));
//
//     byte[] payload = _cryptographyService.Hash(plainHexPublicKey.ToByteArray(), 20);
//     byte[] b = prefix.Concat(payload).ToArray();
//
//     return Base58.Convert(b);
// }