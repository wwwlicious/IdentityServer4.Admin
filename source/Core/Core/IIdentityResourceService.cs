namespace IdentityAdmin.Core
{
    using System.Threading.Tasks;
    using IdentityResource;

    public interface IIdentityResourceService
    {
        Task<IdentityResourceMetaData> GetMetadataAsync();

        Task<IdentityAdminResult<QueryResult<IdentityResourceSummary>>> QueryAsync(string filter, int start, int count);

        Task<IdentityAdminResult<IdentityResourceDetail>> GetAsync(string subject);
    }
}
