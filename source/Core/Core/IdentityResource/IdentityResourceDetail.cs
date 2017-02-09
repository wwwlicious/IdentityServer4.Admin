namespace IdentityAdmin.Core.IdentityResource
{
    using System.Collections.Generic;
    using Core;

    public class IdentityResourceDetail : IdentityResourceSummary
    {
        public IEnumerable<PropertyValue> Properties { get; set; }

        public IEnumerable<IdentityResourceClaimValue> IdentityResourceClaims { get; set; }
    }
}
