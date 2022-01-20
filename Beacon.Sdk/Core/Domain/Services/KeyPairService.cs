namespace Beacon.Sdk.Core.Domain.Services
{
    using System;
    using System.Linq;
    using Entities;
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

        public KeyPairService(ICryptographyService cryptographyService, ISeedRepository seedRepository)
        {
            _cryptographyService = cryptographyService;
            _seedRepository = seedRepository;
        }

        public KeyPair KeyPair
        {
            get
            {
                SeedEntity? data = _seedRepository.TryRead().Result;

                if (data != null)
                    return _cryptographyService.GenerateEd25519KeyPair(data.Seed);

                SeedEntity newSeedEntity = _seedRepository.Create(CreateGuid()).Result ??
                                           throw new ArgumentNullException(nameof(data));

                return _cryptographyService.GenerateEd25519KeyPair(newSeedEntity.Seed);
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