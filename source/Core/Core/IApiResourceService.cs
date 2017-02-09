namespace IdentityAdmin.Core
{
    using System.Threading.Tasks;
    using ApiResource;

    public interface IApiResourceService
    {
        Task<ApiResourceMetaData> GetMetadataAsync();

        Task<IdentityAdminResult<QueryResult<ApiResourceSummary>>> QueryAsync(string filter, int start, int count);

        Task<IdentityAdminResult<ApiResourceDetail>> GetAsync(string subject);
    }
}
