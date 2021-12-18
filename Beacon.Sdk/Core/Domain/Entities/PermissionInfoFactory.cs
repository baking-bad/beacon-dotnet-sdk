namespace Beacon.Sdk.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Beacon;
    using Beacon.Permission;
    using Netezos.Keys;
    using Services;

    public class PermissionInfoFactory
    {
        private readonly AccountService _accountService;

        public PermissionInfoFactory(AccountService accountService)
        {
            _accountService = accountService;
        }

        public PermissionInfo Create(string receiverId, AppMetadata metadata, PubKey publicKey, Network network,
            List<PermissionScope> scopes)
        {
            string address = publicKey.Address;
            string accountId = _accountService.GetAccountIdentifier(address, network);

            return new PermissionInfo
            {
                AccountIdentifier = accountId,
                SenderId = receiverId,
                AppMetadata = metadata,
                Website = "",
                Address = address,
                PublicKey = publicKey.ToString(),
                Network = network,
                Scopes = scopes,
                ConnectedAt = DateTime.UtcNow
            };
        }
    }
}