namespace Beacon.Sdk.Core.Domain
{
    using System;
    using Interfaces;
    using Utils;

    public class Peer
    {
        public Peer(string name, HexString hexPublicKey, string version, string userId)
        {
            Name = name;
            HexPublicKey = hexPublicKey;
            Version = version;
            UserId = userId;
        }

        public long Id { get; set; }

        public string Name { get; }

        public HexString HexPublicKey { get; }

        public string Version { get; }

        public string UserId { get; }

        public string RelayServer { get; }

        internal static class Factory
        {
            public static Peer Create(ICryptographyService cryptographyService, string name, string relayServer,
                HexString hexPublicKey, string version)
            {
                byte[] hexBytes = hexPublicKey.ToByteArray();
                byte[] hash = cryptographyService.Hash(hexBytes, hexBytes.Length);

                if (!HexString.TryParse(hash, out HexString hexHash))
                    throw new Exception();

                return new Peer(name, hexPublicKey, version, $"@{hexHash}:{relayServer}");
            }
        }
    }
}