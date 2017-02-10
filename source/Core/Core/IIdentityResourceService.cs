namespace IdentityAdmin.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using IdentityResource;

    public interface IIdentityResourceService
    {
        Task<IdentityResourceMetaData> GetMetadataAsync();

        Task<IdentityAdminResult<CreateResult>> CreateAsync(IEnumerable<PropertyValue> properties);

        Task<IdentityAdminResult<QueryResult<IdentityResourceSummary>>> QueryAsync(string filter, int start, int count);

        Task<IdentityAdminResult<IdentityResourceDetail>> GetAsync(string subject);

        Task<IdentityAdminResult> DeleteAsync(string subject);

        Task<IdentityAdminResult> SetPropertyAsync(string subject, string type, string value);

        Task<IdentityAdminResult> AddClaimAsync(string subject, string type);

        Task<IdentityAdminResult> RemoveClaimAsync(string subject, string id);
    }
}
