namespace Beacon.Sdk.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Beacon;
    using Beacon.Permission;

    public class PermissionInfo
    {
        public long Id { get; set; }

        public string AccountIdentifier { get; set; }

        public string SenderId { get; set; }

        public AppMetadata AppMetadata { get; set; }

        public string Website { get; set; }

        public string PublicKey { get; set; }

        public DateTime ConnectedAt { get; set; }

        public string Address { get; set; }

        public Network Network { get; set; }

        public List<PermissionScope> Scopes { get; set; }

        public Threshold Threshold { get; set; }
    }

    // public class Permission 
    // {
    //     public string Address { get; set; }
    //     
    //     public Network Network { get; set; }
    //     
    //     public List<PermissionScope> Scopes { get; set; }
    //     
    //     public Threshold Threshold { get; set; }
    // }
}