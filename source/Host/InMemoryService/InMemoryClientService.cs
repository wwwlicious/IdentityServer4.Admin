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
using System.Threading.Tasks;
using AutoMapper;
using IdentityAdmin.Core;
using IdentityAdmin.Core.Client;
using IdentityAdmin.Core.Metadata;
using IdentityAdmin.Extensions;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityAdmin.Host.InMemoryService
{
    internal class InMemoryClientService : IClientService
    {
        private ICollection<InMemoryClient> _clients;        
        public static MapperConfiguration Config;
        public static IMapper Mapper;
        public InMemoryClientService(ICollection<InMemoryClient> clients)
        {
            this._clients = clients;
            Config = new MapperConfiguration(cfg => {
                cfg.CreateMap<InMemoryClientClaim, ClientClaimValue>();
                cfg.CreateMap<ClientClaimValue, InMemoryClientClaim>();
                cfg.CreateMap<InMemoryClientSecret, ClientSecretValue>();
                cfg.CreateMap<ClientSecretValue, InMemoryClientSecret>();
                cfg.CreateMap<InMemoryClientIdPRestriction, ClientIdPRestrictionValue>();
                cfg.CreateMap<ClientIdPRestrictionValue, InMemoryClientIdPRestriction>();
                cfg.CreateMap<InMemoryClientPostLogoutRedirectUri, ClientPostLogoutRedirectUriValue>();
                cfg.CreateMap<ClientPostLogoutRedirectUriValue, InMemoryClientPostLogoutRedirectUri>();
                cfg.CreateMap<InMemoryClientRedirectUri, ClientRedirectUriValue>();
                cfg.CreateMap<ClientRedirectUriValue, InMemoryClientRedirectUri>();
                cfg.CreateMap<InMemoryClientCorsOrigin, ClientCorsOriginValue>();
                cfg.CreateMap<ClientCorsOriginValue, InMemoryClientCorsOrigin>();
                cfg.CreateMap<InMemoryClientCustomGrantType, ClientCustomGrantTypeValue>();
                cfg.CreateMap<ClientCustomGrantTypeValue, InMemoryClientCustomGrantType>();
                cfg.CreateMap<InMemoryClientScope, ClientScopeValue>();
                cfg.CreateMap<ClientScopeValue, InMemoryClientScope>();
            });
            Mapper = Config.CreateMapper();
        }


        private ClientMetaData _metadata;

        private ClientMetaData GetMetadata()
        {
            if (_metadata == null)
            {
                var updateClient = new List<PropertyMetadata>();
                updateClient.AddRange(PropertyMetadata.FromType<InMemoryClient>());

                var createClient = new List<PropertyMetadata>
                {
                    PropertyMetadata.FromProperty<InMemoryClient>(x => x.ClientName, "ClientName", required: true),
                    PropertyMetadata.FromProperty<InMemoryClient>(x => x.ClientId, "ClientId", required: true),
                };

                _metadata = new ClientMetaData
                {
                    SupportsCreate = true,
                    SupportsDelete = true,
                    CreateProperties = createClient,
                    UpdateProperties = updateClient
                };
            }
            return _metadata;
        }

        #region Clients

        public Task<IdentityAdminResult<ClientDetail>> GetClientAsync(string subject)
        {
            int parsedId;
            if (int.TryParse(subject, out parsedId))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedId);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult<ClientDetail>((ClientDetail) null));
                }

                var result = new ClientDetail
                {
                    Subject = subject,
                    ClientId = inMemoryClient.ClientId,
                    ClientName = inMemoryClient.ClientName,
                };
                result.AllowedCorsOrigins = new List<ClientCorsOriginValue>();
                Mapper.Map(inMemoryClient.AllowedCorsOrigins.ToList(), result.AllowedCorsOrigins);
                result.AllowedCustomGrantTypes = new List<ClientCustomGrantTypeValue>();
                Mapper.Map(inMemoryClient.AllowedCustomGrantTypes.ToList(), result.AllowedCustomGrantTypes);
                result.AllowedScopes = new List<ClientScopeValue>();
                Mapper.Map(inMemoryClient.AllowedScopes.ToList(), result.AllowedScopes);
                result.Claims = new List<ClientClaimValue>();
                Mapper.Map(inMemoryClient.Claims.ToList(), result.Claims);
                result.ClientSecrets = new List<ClientSecretValue>();
                Mapper.Map(inMemoryClient.ClientSecrets.ToList(), result.ClientSecrets);
                result.IdentityProviderRestrictions = new List<ClientIdPRestrictionValue>();
                Mapper.Map(inMemoryClient.IdentityProviderRestrictions.ToList(), result.IdentityProviderRestrictions);
                result.PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUriValue>();
                Mapper.Map(inMemoryClient.PostLogoutRedirectUris.ToList(), result.PostLogoutRedirectUris);
                result.RedirectUris = new List<ClientRedirectUriValue>();
                Mapper.Map(inMemoryClient.RedirectUris.ToList(), result.RedirectUris);

                var metadata = GetMetadata();
                var props = from prop in metadata.UpdateProperties
                    select new PropertyValue
                    {
                        Type = prop.Type,
                        Value = GetClientProperty(prop, inMemoryClient),
                    };

                result.Properties = props.ToArray();
                return Task.FromResult(new IdentityAdminResult<ClientDetail>(result));
            }
            return Task.FromResult(new IdentityAdminResult<ClientDetail>((ClientDetail) null));
        }

        public Task<IdentityAdminResult> DeleteClientAsync(string subject)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var client = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (client == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                _clients.Remove(client);
                return Task.FromResult(IdentityAdminResult.Success);
            }

            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        public Task<IdentityAdminResult<QueryResult<ClientSummary>>> QueryClientsAsync(string filter, int start,
            int count)
        {
            var query =
                from client in _clients
                orderby client.ClientName
                select client;

            if (!String.IsNullOrWhiteSpace(filter))
            {
                query =
                    from client in query
                    where client.ClientName.Contains(filter)
                    orderby client.ClientName
                    select client;
            }

            int total = query.Count();
            var clients = query.Skip(start).Take(count).ToArray();

            var result = new QueryResult<ClientSummary>();
            result.Start = start;
            result.Count = count;
            result.Total = total;
            result.Filter = filter;
            result.Items = clients.Select(x =>
            {
                var client = new ClientSummary
                {
                    Subject = x.Id.ToString(),
                    ClientName = x.ClientName,
                    ClientId = x.ClientId
                };

                return client;
            }).ToArray();

            return Task.FromResult(new IdentityAdminResult<QueryResult<ClientSummary>>(result));
        }

        public Task<IdentityAdminResult<CreateResult>> CreateClientAsync(IEnumerable<PropertyValue> properties)
        {
            var clientNameClaim = properties.Single(x => x.Type == "ClientName");
            var clientIdClaim = properties.Single(x => x.Type == "ClientId");

            var clientId = clientNameClaim.Value;
            var clientName = clientIdClaim.Value;

            string[] exclude = new string[] {"ClientName", "ClientId"};
            var otherProperties = properties.Where(x => !exclude.Contains(x.Type)).ToArray();

            var metadata = GetMetadata();
            var createProps = metadata.CreateProperties;
            var client  = new Client();
            var inMemoryClient = new InMemoryClient
            {
                ClientId = clientId,
                ClientName = clientName,
                Id = _clients.Count + 1,
                AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = client.AccessTokenLifetime,
                IdentityTokenLifetime = client.IdentityTokenLifetime,
                SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime,
                Enabled =  true,
                EnableLocalLogin =  true,
            };

            foreach (var prop in otherProperties)
            {
                var propertyResult = SetClientProperty(createProps, inMemoryClient, prop.Type, prop.Value);
                if (!propertyResult.IsSuccess)
                {
                    return Task.FromResult(new IdentityAdminResult<CreateResult>(propertyResult.Errors.ToArray()));
                }
            }

            _clients.Add(inMemoryClient);
            return
                Task.FromResult(
                    new IdentityAdminResult<CreateResult>(new CreateResult {Subject = inMemoryClient.Id.ToString()}));
        }

        public Task<IdentityAdminResult> SetClientPropertyAsync(string subject, string type, string value)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var meta = GetMetadata();

                SetClientProperty(meta.UpdateProperties, inMemoryClient, type, value);
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        #region Client claim

        public Task<IdentityAdminResult> AddClientClaimAsync(string subject, string type, string value)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingClaims = inMemoryClient.Claims;
                if (!existingClaims.Any(x => x.Type == type && x.Value == value))
                {
                    inMemoryClient.Claims.Add(new InMemoryClientClaim()
                    {
                        Type = type,
                        Value = value
                    });
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        public Task<IdentityAdminResult> RemoveClientClaimAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedClientId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedClientId))
            {
                var client = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (client == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingClaim = client.Claims.FirstOrDefault(p => p.Id == parsedClientId);
                if (existingClaim != null)
                {
                    client.Claims.Remove(existingClaim);
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        #endregion

        #region Client Secret

        public Task<IdentityAdminResult> AddClientSecretAsync(string subject, string type, string value)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingSecrets = inMemoryClient.ClientSecrets;
                if (!existingSecrets.Any(x => x.Type == type && x.Value == value))
                {
                    inMemoryClient.ClientSecrets.Add(new InMemoryClientSecret
                    {
                        Type = type,
                        Value = value
                    });
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        public Task<IdentityAdminResult> RemoveClientSecretAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingClientSecret = inMemoryClient.ClientSecrets.FirstOrDefault(p => p.Id == parsedObjectId);
                if (existingClientSecret != null)
                {
                    inMemoryClient.ClientSecrets.Remove(existingClientSecret);
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject or secretId"));
        }

        #endregion

        #region ClientIdPRestriction

        public Task<IdentityAdminResult> AddClientIdPRestrictionAsync(string subject, string provider)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingIdentityProviderRestrictions = inMemoryClient.IdentityProviderRestrictions;
                if (existingIdentityProviderRestrictions.All(x => x.Provider != provider))
                {
                    inMemoryClient.IdentityProviderRestrictions.Add(new InMemoryClientIdPRestriction
                    {
                        Provider = provider,
                    });
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        public Task<IdentityAdminResult> RemoveClientIdPRestrictionAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingIdentityProviderRestrictions =
                    inMemoryClient.IdentityProviderRestrictions.FirstOrDefault(p => p.Id == parsedObjectId);
                if (existingIdentityProviderRestrictions != null)
                {
                    inMemoryClient.IdentityProviderRestrictions.Remove(existingIdentityProviderRestrictions);
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject or secretId"));
        }

        #endregion

        #region PostLogoutRedirectUri

        public Task<IdentityAdminResult> AddPostLogoutRedirectUriAsync(string subject, string uri)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var client = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (client == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingPostLogoutRedirectUris = client.PostLogoutRedirectUris;
                if (existingPostLogoutRedirectUris.All(x => x.Uri != uri))
                {
                    client.PostLogoutRedirectUris.Add(new InMemoryClientPostLogoutRedirectUri
                    {
                        Uri = uri,
                    });
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        public Task<IdentityAdminResult> RemovePostLogoutRedirectUriAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }

                var existingPostLogoutRedirectUris =
                    inMemoryClient.PostLogoutRedirectUris.FirstOrDefault(p => p.Id == parsedObjectId);
                if (existingPostLogoutRedirectUris != null)
                {
                    inMemoryClient.PostLogoutRedirectUris.Remove(existingPostLogoutRedirectUris);
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject or secretId"));
        }

        #endregion

        #region ClientRedirectUri

        public Task<IdentityAdminResult> AddClientRedirectUriAsync(string subject, string uri)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var client = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (client == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingRedirectUris = client.RedirectUris;
                if (existingRedirectUris.All(x => x.Uri != uri))
                {
                    client.RedirectUris.Add(new InMemoryClientRedirectUri
                    {
                        Uri = uri,
                    });
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        public Task<IdentityAdminResult> RemoveClientRedirectUriAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingRedirectUris = inMemoryClient.RedirectUris.FirstOrDefault(p => p.Id == parsedObjectId);
                if (existingRedirectUris != null)
                {
                    inMemoryClient.RedirectUris.Remove(existingRedirectUris);
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject or secretId"));
        }

        #endregion

        #region ClientCorsOrigin

        public Task<IdentityAdminResult> AddClientCorsOriginAsync(string subject, string origin)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingCorsOrigins = inMemoryClient.AllowedCorsOrigins;
                if (existingCorsOrigins.All(x => x.Origin != origin))
                {
                    inMemoryClient.AllowedCorsOrigins.Add(new InMemoryClientCorsOrigin
                    {
                        Origin = origin,
                    });
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        public Task<IdentityAdminResult> RemoveClientCorsOriginAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingCorsOrigins = inMemoryClient.AllowedCorsOrigins.FirstOrDefault(p => p.Id == parsedObjectId);
                if (existingCorsOrigins != null)
                {
                    inMemoryClient.AllowedCorsOrigins.Remove(existingCorsOrigins);
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject or secretId"));
        }

        #endregion

        #region ClientCustomGrantType

        public Task<IdentityAdminResult> AddClientCustomGrantTypeAsync(string subject, string grantType)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingGrantTypes = inMemoryClient.AllowedCustomGrantTypes;
                if (existingGrantTypes.All(x => x.GrantType != grantType))
                {
                    inMemoryClient.AllowedCustomGrantTypes.Add(new InMemoryClientCustomGrantType
                    {
                        GrantType = grantType,
                    });
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        public Task<IdentityAdminResult> RemoveClientCustomGrantTypeAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingGrantTypes =
                    inMemoryClient.AllowedCustomGrantTypes.FirstOrDefault(p => p.Id == parsedObjectId);
                if (existingGrantTypes != null)
                {
                    inMemoryClient.AllowedCustomGrantTypes.Remove(existingGrantTypes);
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject or secretId"));
        }

        #endregion

        #region ClientScope

        public Task<IdentityAdminResult> AddClientScopeAsync(string subject, string scope)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingScopes = inMemoryClient.AllowedScopes;
                if (existingScopes.All(x => x.Scope != scope))
                {
                    inMemoryClient.AllowedScopes.Add(new InMemoryClientScope
                    {
                        Scope = scope,
                    });
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        public Task<IdentityAdminResult> RemoveClientScopeAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                var inMemoryClient = _clients.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryClient == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingScopes = inMemoryClient.AllowedScopes.FirstOrDefault(p => p.Id == parsedObjectId);
                if (existingScopes != null)
                {
                    inMemoryClient.AllowedScopes.Remove(existingScopes);
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject or secretId"));
        }

        #endregion

        #endregion

        public Task<ClientMetaData> GetMetadataAsync()
        {
            return Task.FromResult(GetMetadata());
        }

        #region helperMethods

        protected IdentityAdminResult SetClientProperty(IEnumerable<PropertyMetadata> propsMeta, InMemoryClient client,
            string type, string value)
        {
            IdentityAdminResult result;
            if (propsMeta.TrySet(client, type, value, out result))
            {
                return result;
            }

            throw new Exception("Invalid property type " + type);
        }

        protected string GetClientProperty(PropertyMetadata propMetadata, InMemoryClient client)
        {
            string val;
            if (propMetadata.TryGet(client, out val))
            {
                return val;
            }
            throw new Exception("Invalid property type " + propMetadata.Type);
        }

        private IEnumerable<string> ValidateRoleProperties(IEnumerable<PropertyValue> properties)
        {
            return properties.Select(x => ValidateRoleProperty(x.Type, x.Value)).Aggregate((x, y) => x.Concat(y));
        }

        private IEnumerable<string> ValidateRoleProperty(string type, string value)
        {
            return Enumerable.Empty<string>();
        }

        #endregion
    }
}