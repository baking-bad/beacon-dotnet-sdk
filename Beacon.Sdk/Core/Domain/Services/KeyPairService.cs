namespace Beacon.Sdk.Core.Domain.Services
{
    using System;
    using Infrastructure.Repositories;
    using Interfaces;
    using Interfaces.Data;
    using Sodium;

    public class KeyPairService
    {
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

                string? newSeed = _seedRepository.Create(Guid.NewGuid().ToString()).Result;
                return _cryptographyService.GenerateEd25519KeyPair(newSeed ??
                                                                   throw new ArgumentNullException(nameof(seed)));
            }
        }
    }
}