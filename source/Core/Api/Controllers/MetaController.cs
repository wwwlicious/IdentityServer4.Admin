/*
 * Copyright 2014 Dominick Baier, Brock Allen, Bert Hoorne
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
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityAdmin.Api.Filters;
using IdentityAdmin.Api.Models.Client;
using IdentityAdmin.Core;
using IdentityAdmin.Core.Metadata;
using IdentityAdmin.Extensions;

namespace IdentityAdmin.Api.Controllers
{
    using Models.ApiResource;
    using Models.IdentityResource;

    [NoCache]
    [RoutePrefix(Constants.MetadataRoutePrefix)]
    public class MetaController : ApiController
    {
        private readonly IClientService _clientService;
        private readonly IIdentityResourceService _identityResourceService;
        private readonly IApiResourceService _apiResourceService;

        public MetaController(IClientService clientService, IIdentityResourceService identityResourceService, IApiResourceService apiResourceService)
        {
            if (clientService == null) throw new ArgumentNullException(nameof(clientService));
            if (identityResourceService == null) throw new ArgumentNullException(nameof(identityResourceService));
            if (apiResourceService == null) throw new ArgumentNullException(nameof(apiResourceService));

            _clientService = clientService;
            _identityResourceService = identityResourceService;
            _apiResourceService = apiResourceService;
        }

        private IdentityAdminMetadata _metadata;

        private async Task<IdentityAdminMetadata> GetMetadataAsync()
        {
            if (_metadata == null)
            {
                var clientMetadata = await _clientService.GetMetadataAsync();
                var identityResourceMetaData = await _identityResourceService.GetMetadataAsync();
                var apiResourceMetaData = await _apiResourceService.GetMetadataAsync();

                if (clientMetadata == null) throw new InvalidOperationException("Client GetMetadataAsync returned null");
                if (identityResourceMetaData == null) throw new InvalidOperationException("Identity Resource GetMetadataAsync returned null");
                if (apiResourceMetaData == null) throw new InvalidOperationException("Api Resource GetMetadataAsync returned null");

                _metadata = new IdentityAdminMetadata
                {
                    ClientMetaData = clientMetadata,
                    IdentityResourceMetaData = identityResourceMetaData,
                    ApiResourceMetaData = apiResourceMetaData
                };
                if (_metadata == null) throw new InvalidOperationException("GetMetadataAsync returned null");
                _metadata.Validate();
            }

            return _metadata;
        }

        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            var coreMeta = await GetMetadataAsync();
            var data = new Dictionary<string, object>();

            var cp = (ClaimsPrincipal) User;
            var name = cp.Identity.Name;
            data.Add("currentUser", new
            {
                username = name
            });
        
            var links = new Dictionary<string, object>();
                        
            links["clients"] = Url.RelativeLink(Constants.RouteNames.GetClients);
            if (coreMeta.ClientMetaData.SupportsCreate)
            {
                links["createClient"] = new CreateClientLink(Url, coreMeta.ClientMetaData);
            }

            links["identityresources"] = Url.RelativeLink(Constants.RouteNames.GetIdentityResources);
            if (coreMeta.IdentityResourceMetaData.SupportsCreate)
            {
                links["createIdentityResource"] = new CreateIdentityResourceLink(Url, coreMeta.IdentityResourceMetaData);
            }

            links["apiresources"] = Url.RelativeLink(Constants.RouteNames.GetApiResources);
            if (coreMeta.ApiResourceMetaData.SupportsCreate)
            {
                links["createApiResource"] = new CreateApiResourceLink(Url, coreMeta.ApiResourceMetaData);
            }
            
            return Ok(new
            {
                Data = data,
                Links = links
            });
        }
    }
}