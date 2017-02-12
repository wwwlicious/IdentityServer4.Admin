namespace IdentityAdmin.Core
{
    using Client;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IClientService
    {
        Task<ClientMetaData> GetMetadataAsync();
        Task<IdentityAdminResult<ClientDetail>> GetClientAsync(string subject);
        Task<IdentityAdminResult> DeleteClientAsync(string subject);
        Task<IdentityAdminResult> AddClientClaimAsync(string subject, string type, string value);
        Task<IdentityAdminResult> RemoveClientClaimAsync(string subject, string id);
        Task<IdentityAdminResult> AddClientSecretAsync(string subject, string type, string value);
        Task<IdentityAdminResult> RemoveClientSecretAsync(string subject, string id);
        Task<IdentityAdminResult> AddClientIdPRestrictionAsync(string subject, string provider);
        Task<IdentityAdminResult> RemoveClientIdPRestrictionAsync(string subject, string id);
        Task<IdentityAdminResult> AddPostLogoutRedirectUriAsync(string subject, string uri);
        Task<IdentityAdminResult> RemovePostLogoutRedirectUriAsync(string subject, string id);
        Task<IdentityAdminResult> AddClientRedirectUriAsync(string subject, string uri);
        Task<IdentityAdminResult> RemoveClientRedirectUriAsync(string subject, string id);
        Task<IdentityAdminResult> AddClientCorsOriginAsync(string subject, string origin);
        Task<IdentityAdminResult> RemoveClientCorsOriginAsync(string subject, string id);
        Task<IdentityAdminResult> AddClientCustomGrantTypeAsync(string subject, string grantType);
        Task<IdentityAdminResult> RemoveClientCustomGrantTypeAsync(string subject, string id);
        Task<IdentityAdminResult> AddClientScopeAsync(string subject, string scope);
        Task<IdentityAdminResult> RemoveClientScopeAsync(string subject, string id);
        Task<IdentityAdminResult<QueryResult<ClientSummary>>> QueryClientsAsync(string filter, int start, int count);
        Task<IdentityAdminResult<CreateResult>> CreateClientAsync(IEnumerable<PropertyValue> properties);
        Task<IdentityAdminResult> SetClientPropertyAsync(string subject, string type, string value);
    }
}
