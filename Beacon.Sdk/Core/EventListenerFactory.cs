namespace Beacon.Sdk.Core
{
    using System;
    using Infrastructure.Repositories;
    using Matrix.Sdk.Core.Domain.Room;
    using Matrix.Sdk.Core.Utils;
    using Sodium;

    public interface IEventListenerFactory
    {
        EncryptedMessageListener CreateEncryptedMessageListener(KeyPair keyPair, HexString _publicKeyToListen,
            Action<TextMessageEvent> _onNewTextMessage);
    }

    public class EventListenerFactory : IEventListenerFactory
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly ISessionKeyPairRepository _sessionKeyPairRepository;

        public EventListenerFactory(ICryptographyService cryptographyService,
            ISessionKeyPairRepository sessionKeyPairRepository)
        {
            _cryptographyService = cryptographyService;
            _sessionKeyPairRepository = sessionKeyPairRepository;
        }

        public EncryptedMessageListener CreateEncryptedMessageListener(KeyPair keyPair, HexString _publicKeyToListen,
            Action<TextMessageEvent> _onNewTextMessage)
            => new(_cryptographyService, _sessionKeyPairRepository, _publicKeyToListen,
                _onNewTextMessage);
    }
}