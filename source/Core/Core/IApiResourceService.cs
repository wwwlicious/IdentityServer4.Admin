namespace IdentityAdmin.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ApiResource;

    public interface IApiResourceService
    {
        Task<ApiResourceMetaData> GetMetadataAsync();

        Task<IdentityAdminResult<CreateResult>> CreateAsync(IEnumerable<PropertyValue> properties);

        Task<IdentityAdminResult<QueryResult<ApiResourceSummary>>> QueryAsync(string filter, int start, int count);

        Task<IdentityAdminResult<ApiResourceDetail>> GetAsync(string subject);

        Task<IdentityAdminResult> DeleteAsync(string subject);

        Task<IdentityAdminResult> SetPropertyAsync(string subject, string type, string value);

        Task<IdentityAdminResult> AddClaimAsync(string subject, string type);

        Task<IdentityAdminResult> RemoveClaimAsync(string subject, string id);

        Task<IdentityAdminResult> AddSecretAsync(string subject, string type, string value, string description, DateTime? expiration);
        Task<IdentityAdminResult> UpdateSecretAsync(string subject, string secretSubject, string type, string value, string description, DateTime? expiration);
        Task<IdentityAdminResult> RemoveSecretAsync(string subject, string id);

        Task<IdentityAdminResult> AddScopeAsync(string subject, string name);
        Task<IdentityAdminResult> UpdateScopeAsync(string subject, string scopeSubject, string name, string description, bool emphasize, bool required, bool showInDiscoveryDocument);
        Task<IdentityAdminResult> RemoveScopeAsync(string subject, string id);
    }
}
