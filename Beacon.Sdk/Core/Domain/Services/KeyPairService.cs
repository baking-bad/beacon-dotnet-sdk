using Beacon.Sdk.Core.Infrastructure.Cryptography;

namespace Beacon.Sdk.Core.Domain.Services
{
    using System;
    using System.Linq;
    using Entities;
    using Interfaces;
    using Interfaces.Data;
    using Utils;

    public class KeyPairService
    {
        private const int SeedBytes = 16;
        private readonly ICryptographyService _cryptographyService;
        private readonly ISeedRepository _seedRepository;

        private KeyPair? _keyPair;

        public KeyPairService(ICryptographyService cryptographyService, ISeedRepository seedRepository)
        {
            _cryptographyService = cryptographyService;
            _seedRepository = seedRepository;
        }

        public KeyPair KeyPair
        {
            get
            {
                if (_keyPair != null)
                    return _keyPair;

                SeedEntity? data = _seedRepository.TryReadAsync().Result ??
                                   _seedRepository.CreateAsync(CreateGuid()).Result;

                _keyPair = _cryptographyService.GenerateEd25519KeyPair(data.Seed);

                return _keyPair;
            }
        }

        public static string CreateGuid()
        {
            // var generator = RandomNumberGenerator.Create();
            // var bytes = new byte[SeedBytes]; 
            // generator.GetBytes(bytes); 

            byte[] bytes = SecureRandom.GetRandomBytes(SeedBytes);
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