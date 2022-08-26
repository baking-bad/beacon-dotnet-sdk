using System.Collections.Generic;
using System.Linq;

namespace Beacon.Sdk.Beacon.Permission
{
    public static class BeaconHelper
    {
        public static List<string> GetPermissionStrings(IEnumerable<PermissionScope> permissions)
        {
            return permissions.Select(p => p switch
            {
                PermissionScope.sign => "Sign transactions",
                PermissionScope.operation_request => "Operation request",
                PermissionScope.encrypt => "Encrypt",
                PermissionScope.threshold => "Treshold",
                _ => "Unknown permission"
            }).ToList();
        }
    }
}