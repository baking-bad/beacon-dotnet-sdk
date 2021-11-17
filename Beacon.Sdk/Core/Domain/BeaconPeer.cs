namespace Beacon.Sdk.Core.Domain
{
    using System;
    using Interfaces.Data;
    using Matrix.Sdk.Core.Utils;

    public class BeaconPeer
    {
        private BeaconPeer(string name, HexString hexPublicKey, string version, string userId)
        {
            Name = name;
            HexPublicKey = hexPublicKey;
            Version = version;
            UserId = userId;
        }

        public string Name { get; }
        
        public HexString HexPublicKey { get; }

        public string Version { get; }
        
        public string UserId { get; }
        

        internal static class Factory
        {
            public static BeaconPeer Create(ICryptographyService cryptographyService, string Name, HexString hexPublicKey, string version)
            {
                byte[] hash = cryptographyService.Hash(hexPublicKey.ToByteArray());

                if (!HexString.TryParse(hash, out HexString hexHash))
                    throw new Exception();

                return new BeaconPeer(Name, hexPublicKey, version, $"@{hexHash}");
            } 
        }
    }
}