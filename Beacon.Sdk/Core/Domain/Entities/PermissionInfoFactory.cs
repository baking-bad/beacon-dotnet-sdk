namespace Beacon.Sdk.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Beacon;
    using Beacon.Permission;
    using Services;

    public class PermissionInfoFactory
    {
        private readonly AccountService _accountService;

        public PermissionInfoFactory(AccountService accountService)
        {
            _accountService = accountService;
        }

        public PermissionInfo Create(string receiverId, AppMetadata metadata, string publicKey, Network network,
            List<PermissionScope> scopes)
        {
            string address = _accountService.GetAddressFromPublicKey(publicKey);
            string accountId = _accountService.GetAccountIdentifier(address, network);

            return new PermissionInfo
            {
                AccountIdentifier = accountId,
                SenderId = receiverId,
                AppMetadata = metadata,
                Website = "",
                Address = address,
                PublicKey = publicKey,
                Network = network,
                Scopes = scopes,
                ConnectedAt = DateTimeOffset.Now
            };
        }
    }
}