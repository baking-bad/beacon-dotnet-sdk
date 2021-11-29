namespace Beacon.Sdk.Core.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Base58Check;
    using Beacon.Permission;
    using Interfaces;
    using Utils;

    public class AccountService
    {
        private readonly ICryptographyService _cryptographyService;

        public AccountService(ICryptographyService cryptographyService)
        {
            _cryptographyService = cryptographyService;
        }

        public string GetAddressFromPublicKey(string publicKey)
        {
            // tz1
            var edpk = new keyCurve(54, new byte[] {6, 161, 159});

            // tz2
            var sppk = new keyCurve(55, new byte[] {6, 161, 161});

            // tz3
            var p2pk = new keyCurve(55, new byte[] {6, 161, 164});

            byte[]? prefix = null;
            string? plainPublicKey = null;

            if (publicKey.Length == 64)
            {
                prefix = edpk.Prefix;
                plainPublicKey = publicKey;
            }
            else
            {
                foreach (keyCurve v in new List<keyCurve> {edpk, sppk, p2pk})
                {
                    string p = Encoding.UTF8.GetString(v.Prefix);
                    if (publicKey.StartsWith(p) && publicKey.Length == v.Length)
                    {
                        prefix = v.Prefix;
                        byte[] decoded = Base58CheckEncoding.Decode(publicKey)!;

                        plainPublicKey = Encoding.UTF8.GetString(decoded[p.Length..decoded.Length]);
                        break;
                    }
                }
            }


            if (prefix == null || plainPublicKey == null)
                throw new Exception($"Invalid publicKey: {plainPublicKey}");

            if (!HexString.TryParse(plainPublicKey, out HexString plainHexPublicKey))
                throw new ArgumentException(nameof(plainPublicKey));

            byte[] payload = _cryptographyService.Hash(plainHexPublicKey.ToByteArray(), 20);
            byte[] b = prefix.Concat(payload).ToArray();

            return Base58CheckEncoding.Encode(b);
        }

        public string GetAccountIdentifier(string address, Network network)
        {
            var data = new List<string> {address, network.Type.ToString()};

            if (network.Name != null)
                data.Add($"name:{network.Name}");

            if (network.RpcUrl != null)
                data.Add($"rpc:{network.RpcUrl}");

            var m = string.Join('-', data);

            byte[] h = _cryptographyService.Hash(Encoding.UTF8.GetBytes(m), 10);

            return Base58CheckEncoding.Encode(h);
        }

        private record keyCurve(int Length, byte[] Prefix);
    }
}