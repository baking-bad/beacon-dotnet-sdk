namespace Beacon.Sdk.Core.Domain.Services
{
    using System;
    using System.Linq;
    using Infrastructure.Repositories;
    using Interfaces;
    using Interfaces.Data;
    using Sodium;
    using Utils;

    public class KeyPairService
    {
        private const int SeedBytes = 16;
        private readonly ICryptographyService _cryptographyService;
        private readonly ISeedRepository _seedRepository;

        public KeyPairService(ICryptographyService cryptographyService, LiteDbSeedRepository seedRepository)
        {
            _cryptographyService = cryptographyService;
            _seedRepository = seedRepository;
        }

        public KeyPair KeyPair
        {
            get
            {
                string? seed = _seedRepository.TryRead().Result;
                if (seed != null)
                    return _cryptographyService.GenerateEd25519KeyPair(seed);

                string newSeed = _seedRepository.Create(CreateGuid()).Result ??
                                 throw new ArgumentNullException(nameof(seed));
                return _cryptographyService.GenerateEd25519KeyPair(newSeed);
            }
        }

        public static string CreateGuid()
        {
            // var generator = RandomNumberGenerator.Create();
            // var bytes = new byte[SeedBytes]; 
            // generator.GetBytes(bytes); 

            byte[] bytes = SodiumCore.GetRandomBytes(SeedBytes);
            byte[][] b = {bytes[..4], bytes[4..6], bytes[6..8], bytes[8..10], bytes[10..16]};

            string[] hexStrings = b.Select(x =>
            {
                if (!HexString.TryParse(x, out HexString y)) throw new Exception("y");
                return y.ToString();
            }).ToArray();

            return string.Join("-", hexStrings);
        }
    }
}