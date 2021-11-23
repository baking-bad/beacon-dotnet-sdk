namespace Beacon.Sdk.Core.Domain
{
    using System;
    using Interfaces;
    using Utils;

    public class BeaconPeer
    {
        public BeaconPeer(string name, HexString hexPublicKey, string version, string userId)
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

        internal static class Factory
        {
            public static BeaconPeer Create(ICryptographyService cryptographyService, string name, string relayServer,
                HexString hexPublicKey, string version)
            {
                byte[] hash = cryptographyService.Hash(hexPublicKey.ToByteArray());

                if (!HexString.TryParse(hash, out HexString hexHash))
                    throw new Exception();

                return new BeaconPeer(name, hexPublicKey, version, $"@{hexHash}:{relayServer}");
            }
        }
    }
}