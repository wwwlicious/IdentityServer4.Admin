/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Threading.Tasks;

using IdentityAdmin.Core.Client;

namespace IdentityAdmin.Core
{
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
