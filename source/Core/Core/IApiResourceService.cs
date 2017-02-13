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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityAdmin.Core.ApiResource;

namespace IdentityAdmin.Core
{
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
        Task<IdentityAdminResult> AddScopeClaimAsync(string subject, string id, string type);    
        Task<IdentityAdminResult> RemoveScopeClaimAsync(string subject, string id, string scopeId);
    }
}
