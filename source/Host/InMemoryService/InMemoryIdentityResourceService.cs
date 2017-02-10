namespace IdentityAdmin.Host.InMemoryService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Core;
    using Core.IdentityResource;
    using Core.Metadata;
    using Extensions;

    public class InMemoryIdentityResourceService : IIdentityResourceService
    {
        private readonly ICollection<InMemoryIdentityResource> _identityResources;
        public static MapperConfiguration Config;

        public InMemoryIdentityResourceService(ICollection<InMemoryIdentityResource> identityResources)
        {
            this._identityResources = identityResources;
        }

        private IdentityResourceMetaData _metadata;

        private IdentityResourceMetaData GetMetadata()
        {
            if (_metadata == null)
            {
                var updateIdentityResource = new List<PropertyMetadata>();
                updateIdentityResource.AddRange(PropertyMetadata.FromType<InMemoryIdentityResource>());

                var createIdentityResource = new List<PropertyMetadata>
                {
                    PropertyMetadata.FromProperty<InMemoryIdentityResource>(x => x.Name, "IdentityResourceName", required: true),
                };

                _metadata = new IdentityResourceMetaData
                {
                    SupportsCreate = true,
                    SupportsDelete = true,
                    CreateProperties = createIdentityResource,
                    UpdateProperties = updateIdentityResource
                };
            }
            return _metadata;
        }

        public Task<IdentityResourceMetaData> GetMetadataAsync()
        {
            return Task.FromResult(GetMetadata());
        }

        public Task<IdentityAdminResult<CreateResult>> CreateAsync(IEnumerable<PropertyValue> properties)
        {
            var IdentityResourceNameClaim = properties.Single(x => x.Type == "IdentityResourceName");

            var IdentityResourceName = IdentityResourceNameClaim.Value;
            

            string[] exclude = { "IdentityResourceName" };
            var otherProperties = properties.Where(x => !exclude.Contains(x.Type)).ToArray();

            var metadata = GetMetadata();
            var createProps = metadata.CreateProperties;
            var inMemoryIdentityResource = new InMemoryIdentityResource
            {
                Id = _identityResources.Count + 1,
                Name = IdentityResourceName,
                Enabled = true,
                Required = false,
                ShowInDiscoveryDocument = true
            };
            
            foreach (var prop in otherProperties)
            {
                var propertyResult = SetProperty(createProps, inMemoryIdentityResource, prop.Type, prop.Value);
                if (!propertyResult.IsSuccess)
                {
                    return Task.FromResult(new IdentityAdminResult<CreateResult>(propertyResult.Errors.ToArray()));
                }
            }
            _identityResources.Add(inMemoryIdentityResource);
            return Task.FromResult(new IdentityAdminResult<CreateResult>(new CreateResult { Subject = inMemoryIdentityResource.Id.ToString() }));
        }

        public Task<IdentityAdminResult<QueryResult<IdentityResourceSummary>>> QueryAsync(string filter, int start, int count)
        {
            var query = from identityResource in _identityResources orderby identityResource.Name select identityResource;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query =
                    from identityResource in query
                    where identityResource.Name.Contains(filter)
                    orderby identityResource.Name
                    select identityResource;
            }

            int total = query.Count();
            var scopes = query.Skip(start).Take(count).ToArray();

            var result = new QueryResult<IdentityResourceSummary>
            {
                Start = start,
                Count = count,
                Total = total,
                Filter = filter,
                Items = scopes.Select(x =>
                {
                    var scope = new IdentityResourceSummary
                    {
                        Subject = x.Id.ToString(),
                        Name = x.Name,
                        Description = x.Name
                    };

                    return scope;
                }).ToArray()
            };

            return Task.FromResult(new IdentityAdminResult<QueryResult<IdentityResourceSummary>>(result));
        }

        public Task<IdentityAdminResult<IdentityResourceDetail>> GetAsync(string subject)
        {
            int parsedId;
            if (int.TryParse(subject, out parsedId))
            {
                var inMemoryApiResource = _identityResources.FirstOrDefault(p => p.Id == parsedId);
                if (inMemoryApiResource == null)
                {
                    return Task.FromResult(new IdentityAdminResult<IdentityResourceDetail>((IdentityResourceDetail)null));
                }

                var result = new IdentityResourceDetail
                {
                    Subject = subject,
                    Name = inMemoryApiResource.Name,
                    Description = inMemoryApiResource.Description
                };

                var metadata = GetMetadata();
                var props = from prop in metadata.UpdateProperties
                            select new PropertyValue
                            {
                                Type = prop.Type,
                                Value = GetProperty(prop, inMemoryApiResource),
                            };

                result.Properties = props.ToArray();
                result.IdentityResourceClaims = inMemoryApiResource.Claims.Select(x => new IdentityResourceClaimValue
                {
                    Id = x.Id.ToString(),
                    Type = x.Type
                });

                return Task.FromResult(new IdentityAdminResult<IdentityResourceDetail>(result));
            }
            return Task.FromResult(new IdentityAdminResult<IdentityResourceDetail>((IdentityResourceDetail)null));
        }

        public Task<IdentityAdminResult> DeleteAsync(string subject)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var inMemoryIdentityResource = _identityResources.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryIdentityResource == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                _identityResources.Remove(inMemoryIdentityResource);
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        public Task<IdentityAdminResult> SetPropertyAsync(string subject, string type, string value)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var inMemoryApiResource = _identityResources.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryApiResource == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var meta = GetMetadata();

                SetProperty(meta.UpdateProperties, inMemoryApiResource, type, value);

                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        protected string GetProperty(PropertyMetadata propMetadata, InMemoryIdentityResource identityResource)
        {
            string val;
            if (propMetadata.TryGet(identityResource, out val))
            {
                return val;
            }
            throw new Exception("Invalid property type " + propMetadata.Type);
        }

        protected IdentityAdminResult SetProperty(IEnumerable<PropertyMetadata> propsMeta, InMemoryIdentityResource identityResource, string type, string value)
        {
            IdentityAdminResult result;
            if (propsMeta.TrySet(identityResource, type, value, out result))
            {
                return result;
            }

            throw new Exception("Invalid property type " + type);
        }

        public Task<IdentityAdminResult> AddClaimAsync(string subject, string type)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var inMemoryIdentityResource = _identityResources.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryIdentityResource == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingClaims = inMemoryIdentityResource.Claims;
                if (existingClaims.All(x => x.Type != type))
                {
                    inMemoryIdentityResource.Claims.Add(new InMemoryIdentityResourceClaim
                    {
                        Id = inMemoryIdentityResource.Claims.Count + 1,
                        Type = type
                    });
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }

            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        public Task<IdentityAdminResult> RemoveClaimAsync(string subject, string id)
        {
            int parsedSubject;
            int parseClaimId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parseClaimId))
            {
                var identityResource = _identityResources.FirstOrDefault(p => p.Id == parsedSubject);
                if (identityResource == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                var existingClaim = identityResource.Claims.FirstOrDefault(p => p.Id == parseClaimId);
                if (existingClaim != null)
                {
                    identityResource.Claims.Remove(existingClaim);
                }
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }
    }
}