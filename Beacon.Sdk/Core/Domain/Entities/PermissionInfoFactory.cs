using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public async Task<PermissionInfo> Create(string receiverId, AppMetadata metadata, string address,
            string publicKey, Network network, List<PermissionScope> scopes)
        {
            var permissionInfo = new PermissionInfo
            {
                AccountId = _accountService.GetAccountId(address, network),
                SenderId = receiverId,
                AppMetadata = metadata,
                Website = string.Empty,
                Address = address,
                PublicKey = publicKey,
                Network = network,
                Scopes = scopes,
                ConnectedAt = DateTime.UtcNow
            };

            try
            {
                var dappResponse = await new HttpClient().GetAsync(
                    "https://bcd-static-assets.fra1.digitaloceanspaces.com/dapps.json");

                if (dappResponse.IsSuccessStatusCode)
                {
                    var dappsString = await dappResponse.Content.ReadAsStringAsync();
                    var dapps = JsonConvert.DeserializeObject<JArray>(dappsString);

                    foreach (var dapp in dapps)
                    {
                        var dappName = metadata.Name.ToLower();
                        if (dappName == "smak staking")
                            dappName = "vortex";

                        if (dapp["name"]!.ToString().ToLower().Contains(dappName))
                        {
                            permissionInfo.Website = dapp["dapp_url"]!.ToString();
                            permissionInfo.AppMetadata.Icon = dapp["logo_url"]!.ToString();
                        }
                        
                        if (dappName == "rarible")
                        {
                            permissionInfo.Website = "https://rarible.com";
                            permissionInfo.AppMetadata.Icon =
                                "https://services.tzkt.io/v1/avatars/KT18pVpRXKPY2c4U2yFEGSH3ZnhB2kL8kwXS";
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return permissionInfo;
        }
    }
}