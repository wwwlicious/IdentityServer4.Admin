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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityAdmin.Api.Filters;
using IdentityAdmin.Api.Models.Client;
using IdentityAdmin.Core;
using IdentityAdmin.Core.Client;
using IdentityAdmin.Core.Metadata;
using IdentityAdmin.Extensions;
using IdentityAdmin.Resources;

namespace IdentityAdmin.Api.Controllers
{
    [RoutePrefix(Constants.ClientsRoutePrefix)]
    [NoCache]
    public class ClientController : ApiController
    {
        readonly IClientService _clientService;
        public ClientController(IClientService clientService)
        {
            if (clientService == null) throw new ArgumentNullException(nameof(clientService));
            _clientService = clientService;
        }

        public IHttpActionResult BadRequest<T>(T data)
        {
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, data));
        }
        
        public IHttpActionResult NoContent()
        {
            return StatusCode(HttpStatusCode.NoContent);
        }

        public IHttpActionResult MethodNotAllowed()
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        ClientMetaData _clientMetadata;
        
        async Task<ClientMetaData> GetCoreMetaDataAsync()
        {
            if (_clientMetadata == null)
            {
                _clientMetadata = await _clientService.GetMetadataAsync();
                if (_clientMetadata == null) throw new InvalidOperationException("coreMetaData returned null");
                _clientMetadata.Validate();

                return _clientMetadata;
            }

            return _clientMetadata;
        }

        #region Client

        [HttpGet, Route("", Name = Constants.RouteNames.GetClients)]
        public async Task<IHttpActionResult> GetClientsAsync(string filter = null, int start = 0, int count = 100)
        {
            var result = await _clientService.QueryClientsAsync(filter, start, count);
            if (result.IsSuccess)
            {
                var meta = await GetCoreMetaDataAsync();
                var resource = new ClientQueryResultResource(result.Result, Url, _clientMetadata);
                return Ok(resource);
            }

            return BadRequest(result.ToError());
        }

        [HttpGet, Route("{subject}", Name = Constants.RouteNames.GetClient)]
        public async Task<IHttpActionResult> GetClientAsync(string subject)
        {
            if (String.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToError());
            }

            var result = await _clientService.GetClientAsync(subject);
            if (result.IsSuccess)
            {
                if (result.Result == null)
                {
                    return NotFound();
                }

                var meta = await GetCoreMetaDataAsync();
                return Ok(new ClientDetailResource(result.Result, Url, meta));
            }

            return BadRequest(result.ToError());
        }

        [HttpPost, Route("", Name = Constants.RouteNames.CreateClient)]
        public async Task<IHttpActionResult> CreateClientAsync(PropertyValue[] properties)
        {
            var coreMetadata = await GetCoreMetaDataAsync();
            if (!coreMetadata.SupportsCreate)
            {
                return MethodNotAllowed();
            }
            if (properties == null)
            {
                ModelState.AddModelError("", Messages.ClientDataRequired);
            }

            var errors = ValidateCreateProperties(coreMetadata, properties);
            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                var result = await _clientService.CreateClientAsync(properties);
                if (result.IsSuccess)
                {
                    var url = Url.RelativeLink(Constants.RouteNames.GetClient, new { subject = result.Result.Subject });
                    var resource = new
                    {
                        Data = new { subject = result.Result.Subject },
                        Links = new { detail = url }
                    };
                    return Created(url, resource);
                }

                ModelState.AddErrors(result);
            }

            return BadRequest(ModelState.ToError());
        }
        
        [HttpDelete, Route("{subject}", Name = Constants.RouteNames.DeleteClient)]
        public async Task<IHttpActionResult> DeleteClientAsync(string subject)
        {
            var meta = await GetCoreMetaDataAsync();
            if (!meta.SupportsDelete)
            {
                return MethodNotAllowed();
            }

            if (String.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToError());
            }

            var result = await _clientService.DeleteClientAsync(subject);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.ToError());
        }

        [HttpPut, Route("{subject}/properties/{type}", Name = Constants.RouteNames.UpdateClientProperty)]
        public async Task<IHttpActionResult> SetPropertyAsync(string subject, string type)
        {
            if (String.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            type = type.FromBase64UrlEncoded();

            string value = await Request.Content.ReadAsStringAsync();
            var meta = await GetCoreMetaDataAsync();
            ValidateUpdateProperty(meta, type, value);

            if (ModelState.IsValid)
            {
                var result = await _clientService.SetClientPropertyAsync(subject, type, value);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                ModelState.AddErrors(result);
            }

            return BadRequest(ModelState.ToError());
        }

        #region ClientClaim
        
        [HttpPost, Route("{subject}/claim", Name = Constants.RouteNames.AddClientClaim)]
        public async Task<IHttpActionResult> AddClientClaimAsync(string subject, ClientClaimValue model)
        {
            if (String.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            if (model == null)
            {
                ModelState.AddModelError("", Messages.ClaimDataRequired);
            }

            if (ModelState.IsValid)
            {
                var result = await _clientService.AddClientClaimAsync(subject, model.Type, model.Value);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                ModelState.AddErrors(result);
            }

            return BadRequest(ModelState.ToError());
        }

        [HttpDelete, Route("{subject}/claim/{id}", Name = Constants.RouteNames.RemoveClientClaim)]
        public async Task<IHttpActionResult> RemoveClaimAsync(string subject, string id)
        {
            if (String.IsNullOrWhiteSpace(subject))
            {
                ModelState.AddModelError("", Messages.SubjectRequired);
            }
            if (String.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError("", Messages.IdIsRequired);
            }
            if (ModelState.IsValid)
            {
                var result = await _clientService.RemoveClientClaimAsync(subject, id);
                if (result.IsSuccess)
                {
                    return NoContent();
                }
                return BadRequest(result.ToError());
            }

            return BadRequest(ModelState.ToError());
        }
        
        #endregion
        
        #region ClientSecret

        [HttpPost, Route("{subject}/clientSecret", Name = Constants.RouteNames.AddClientSecret)]
        public async Task<IHttpActionResult> AddClientSecretAsync(string subject, ClientSecretValue model)
        {
            if (String.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            if (model == null)
            {
                ModelState.AddModelError("", "Model required");
            }

            if (ModelState.IsValid)
            {
                var result = await _clientService.AddClientSecretAsync(subject, model.Type, model.Value);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                ModelState.AddErrors(result);
            }

            return BadRequest(ModelState.ToError());
        }

        [HttpDelete, Route("{subject}/clientSecret/{id}", Name = Constants.RouteNames.RemoveClientSecret)]
        public async Task<IHttpActionResult> RemoveClientSecretAsync(string subject, string id)
        {
            if (String.IsNullOrWhiteSpace(subject) ||
                String.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var result = await _clientService.RemoveClientSecretAsync(subject, id);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.ToError());
        }

        #endregion  

        #region ClientIdPRestriction

        [HttpPost, Route("{subject}/clientIdPRestriction", Name = Constants.RouteNames.AddClientIdPRestriction)]
        public async Task<IHttpActionResult> AddClientIdPRestrictionAsync(string subject, ClientIdPRestrictionValue model)
        {
            if (String.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            if (model == null)
            {
                ModelState.AddModelError("", "Model required");
            }

            if (ModelState.IsValid)
            {
                var result = await _clientService.AddClientIdPRestrictionAsync(subject, model.Provider);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                ModelState.AddErrors(result);
            }

            return BadRequest(ModelState.ToError());
        }

        [HttpDelete, Route("{subject}/clientIdPRestriction/{id}", Name = Constants.RouteNames.RemoveClientIdPRestriction)]
        public async Task<IHttpActionResult> RemoveClientIdPRestrictionAsync(string subject, string id)
        {
           if (String.IsNullOrWhiteSpace(subject) || String.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var result = await _clientService.RemoveClientIdPRestrictionAsync(subject, id);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.ToError());
        }

        #endregion

        #region PostLogoutRedirectUri

        [HttpPost, Route("{subject}/clientPostLogoutRedirectUri", Name = Constants.RouteNames.AddClientPostLogoutRedirectUri)]
        public async Task<IHttpActionResult> AddClientPostLogoutRedirectUriAsync(string subject, ClientPostLogoutRedirectUriValue model)
        {
            if (String.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            if (model == null)
            {
                ModelState.AddModelError("", "Model required");
            }

            if (ModelState.IsValid)
            {
                var result = await _clientService.AddPostLogoutRedirectUriAsync(subject, model.Uri);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                ModelState.AddErrors(result);
            }

            return BadRequest(ModelState.ToError());
        }

        [HttpDelete, Route("{subject}/clientPostLogoutRedirectUri/{id}", Name = Constants.RouteNames.RemoveClientPostLogoutRedirectUri)]
        public async Task<IHttpActionResult> RemoveClientPostLogoutRedirectUriAsync(string subject, string id)
        {
            if (String.IsNullOrWhiteSpace(subject) || String.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var result = await _clientService.RemovePostLogoutRedirectUriAsync(subject, id);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.ToError());
        }

        #endregion

        #region ClientRedirectUri

        [HttpPost, Route("{subject}/clientRedirectUri", Name = Constants.RouteNames.AddClientRedirectUri)]
        public async Task<IHttpActionResult> AddClientRedirectUriAsync(string subject, ClientRedirectUriValue model)
        {
            if (String.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            if (model == null)
            {
                ModelState.AddModelError("", "Model required");
            }

            if (ModelState.IsValid)
            {
                var result = await _clientService.AddClientRedirectUriAsync(subject, model.Uri);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                ModelState.AddErrors(result);
            }

            return BadRequest(ModelState.ToError());
        }

        [HttpDelete, Route("{subject}/clientRedirectUri/{id}", Name = Constants.RouteNames.RemoveClientRedirectUri)]
        public async Task<IHttpActionResult> RemoveClientRedirectUriAsync(string subject, string id)
        {
            if (String.IsNullOrWhiteSpace(subject) || String.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var result = await _clientService.RemoveClientRedirectUriAsync(subject, id);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.ToError());
        }

        #endregion

        #region ClientCorsOrigin
        [HttpPost, Route("{subject}/clientCorsOrigin", Name = Constants.RouteNames.AddClientCorsOrigin)]
        public async Task<IHttpActionResult> AddClientCorsOriginAsync(string subject, ClientCorsOriginValue model)
        {
            if (String.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            if (model == null)
            {
                ModelState.AddModelError("", "Model required");
            }

            if (ModelState.IsValid)
            {
                var result = await _clientService.AddClientCorsOriginAsync(subject, model.Origin);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                ModelState.AddErrors(result);
            }

            return BadRequest(ModelState.ToError());
        }

        [HttpDelete, Route("{subject}/clientCorsOrigin/{id}", Name = Constants.RouteNames.RemoveClientCorsOrigin)]
        public async Task<IHttpActionResult> RemoveClientCorsOriginAsync(string subject, string id)
        {
            if (String.IsNullOrWhiteSpace(subject) || String.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var result = await _clientService.RemoveClientCorsOriginAsync(subject, id);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.ToError());
        }
        #endregion

        #region ClientCustomGrantType

        [HttpPost, Route("{subject}/clientCustomGrantType", Name = Constants.RouteNames.AddClientCustomGrantType)]
        public async Task<IHttpActionResult> AddClientCustomGrantTypeAsync(string subject, ClientCustomGrantTypeValue model)
        {
            if (String.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            if (model == null)
            {
                ModelState.AddModelError("", "Model required");
            }

            if (ModelState.IsValid)
            {
                var result = await _clientService.AddClientCustomGrantTypeAsync(subject, model.GrantType);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                ModelState.AddErrors(result);
            }

            return BadRequest(ModelState.ToError());
        }

        [HttpDelete, Route("{subject}/clientCustomGrantType/{id}", Name = Constants.RouteNames.RemoveClientCustomGrantType)]
        public async Task<IHttpActionResult> RemoveClientCustomGrantTypeAsync(string subject, string id)
        {
            if (String.IsNullOrWhiteSpace(subject) || String.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var result = await _clientService.RemoveClientCustomGrantTypeAsync(subject, id);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.ToError());
        }
        #endregion

        #region ClientScope
        [HttpPost, Route("{subject}/clientScope", Name = Constants.RouteNames.AddClientScope)]
        public async Task<IHttpActionResult> AddClientScopeAsync(string subject, ClientScopeValue model)
        {
            if (String.IsNullOrWhiteSpace(subject))
            {
                ModelState["subject.String"].Errors.Clear();
                ModelState.AddModelError("", Messages.SubjectRequired);
            }

            if (model == null)
            {
                ModelState.AddModelError("", "Model required");
            }

            if (ModelState.IsValid)
            {
                var result = await _clientService.AddClientScopeAsync(subject, model.Scope);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                ModelState.AddErrors(result);
            }

            return BadRequest(ModelState.ToError());
        }

        [HttpDelete, Route("{subject}/clientScope/{id}", Name = Constants.RouteNames.RemoveClientScope)]
        public async Task<IHttpActionResult> RemoveClientScopeAsync(string subject, string id)
        {
            if (String.IsNullOrWhiteSpace(subject) || String.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var result = await _clientService.RemoveClientScopeAsync(subject, id);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.ToError());
        }
        #endregion

        #endregion

        private void ValidateUpdateProperty(ClientMetaData clientMetaData, string type, string value)
        {
            if (clientMetaData == null) throw new ArgumentNullException("clientMetaData");

            if (String.IsNullOrWhiteSpace(type))
            {
                ModelState.AddModelError("", Messages.PropertyTypeRequired);
                return;
            }

            var prop = clientMetaData.UpdateProperties.SingleOrDefault(x => x.Type == type);
            if (prop == null)
            {
                ModelState.AddModelError("", String.Format(Messages.PropertyInvalid, type));
            }
            else
            {
                var error = prop.Validate(value);
                if (error != null)
                {
                    ModelState.AddModelError("", error);
                }
            }
        }

        private IEnumerable<string> ValidateCreateProperties(ClientMetaData clientMetaData, IEnumerable<PropertyValue> properties)
        {
            if (clientMetaData == null) throw new ArgumentNullException("clientMetaData");
            properties = properties ?? Enumerable.Empty<PropertyValue>();

            var meta = clientMetaData.CreateProperties;
            return meta.Validate(properties);
        }

    }
}
