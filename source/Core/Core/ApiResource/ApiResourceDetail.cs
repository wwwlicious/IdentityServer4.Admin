namespace IdentityAdmin.Core.ApiResource
{
    using System.Collections.Generic;
    using Core;

    public class ApiResourceDetail : ApiResourceSummary
    {
        public IEnumerable<PropertyValue> Properties { get; set; }

        public IEnumerable<ApiResourceClaimValue> ResourceClaims { get; set; }

        public IEnumerable<ApiResourceScopeValue> ResourceScopes { get; set; }

        public IEnumerable<ApiResourceSecretValue> ResourceSecrets { get; set; }
    }
}
