namespace IdentityAdmin.Host.InMemoryService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Core;
    using Core.ApiResource;
    using Core.Metadata;
    using Extensions;

    public class InMemoryApiResourceService : IApiResourceService
    {
        private readonly ICollection<InMemoryApiResource> _apiResources;

        public InMemoryApiResourceService(ICollection<InMemoryApiResource> apiResources)
        {
            this._apiResources = apiResources;
        }

        private ApiResourceMetaData _metadata;

        private ApiResourceMetaData GetMetadata()
        {
            if (_metadata == null)
            {
                var updateApiResource = new List<PropertyMetadata>();
                updateApiResource.AddRange(PropertyMetadata.FromType<InMemoryApiResource>());

                var createApiResource = new List<PropertyMetadata>
                {
                    PropertyMetadata.FromProperty<InMemoryApiResource>(x => x.Name, "ApiResourceName", required: true),
                };

                _metadata = new ApiResourceMetaData
                {
                    SupportsCreate = true,
                    SupportsDelete = true,
                    CreateProperties = createApiResource,
                    UpdateProperties = updateApiResource
                };
            }
            return _metadata;
        }

        public Task<ApiResourceMetaData> GetMetadataAsync()
        {
            return Task.FromResult(GetMetadata());
        }

        public Task<IdentityAdminResult<CreateResult>> CreateAsync(IEnumerable<PropertyValue> properties)
        {
            var ApiResourceNameClaim = properties.Single(x => x.Type == "ApiResourceName");

            var ApiResourceName = ApiResourceNameClaim.Value;

            string[] exclude = { "ApiResourceName" };
            var otherProperties = properties.Where(x => !exclude.Contains(x.Type)).ToArray();

            var metadata = GetMetadata();
            var createProps = metadata.CreateProperties;
            var inMemoryApiResource = new InMemoryApiResource
            {
                Id = _apiResources.Count + 1,
                Name = ApiResourceName,
                Enabled = true
            };

            foreach (var prop in otherProperties)
            {
                var propertyResult = SetProperty(createProps, inMemoryApiResource, prop.Type, prop.Value);
                if (!propertyResult.IsSuccess)
                {
                    return Task.FromResult(new IdentityAdminResult<CreateResult>(propertyResult.Errors.ToArray()));
                }
            }
            _apiResources.Add(inMemoryApiResource);
            return Task.FromResult(new IdentityAdminResult<CreateResult>(new CreateResult { Subject = inMemoryApiResource.Id.ToString() }));
        }

        public Task<IdentityAdminResult<QueryResult<ApiResourceSummary>>> QueryAsync(string filter, int start, int count)
        {
            var query = from apiResource in _apiResources orderby apiResource.Name select apiResource;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query =
                    from apiResource in query
                    where apiResource.Name.Contains(filter)
                    orderby apiResource.Name
                    select apiResource;
            }

            int total = query.Count();
            var scopes = query.Skip(start).Take(count).ToArray();

            var result = new QueryResult<ApiResourceSummary>
            {
                Start = start,
                Count = count,
                Total = total,
                Filter = filter,
                Items = scopes.Select(x =>
                {
                    var scope = new ApiResourceSummary
                    {
                        Subject = x.Id.ToString(),
                        Name = x.Name,
                        Description = x.Name
                    };

                    return scope;
                }).ToArray()
            };

            return Task.FromResult(new IdentityAdminResult<QueryResult<ApiResourceSummary>>(result));
        }

        public Task<IdentityAdminResult<ApiResourceDetail>> GetAsync(string subject)
        {
            int parsedId;
            if (int.TryParse(subject, out parsedId))
            {
                var inMemoryApiResource = _apiResources.FirstOrDefault(p => p.Id == parsedId);
                if (inMemoryApiResource == null)
                {
                    return Task.FromResult(new IdentityAdminResult<ApiResourceDetail>((ApiResourceDetail)null));
                }

                var result = new ApiResourceDetail
                {
                    Subject = subject,
                    Name = inMemoryApiResource.Name,
                    Description = inMemoryApiResource.Description                    
                };

                return Task.FromResult(new IdentityAdminResult<ApiResourceDetail>(result));
            }
            return Task.FromResult(new IdentityAdminResult<ApiResourceDetail>((ApiResourceDetail)null));
        }

        public Task<IdentityAdminResult> DeleteAsync(string subject)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var inMemoryApiResource = _apiResources.FirstOrDefault(p => p.Id == parsedSubject);
                if (inMemoryApiResource == null)
                {
                    return Task.FromResult(new IdentityAdminResult("Invalid subject"));
                }
                _apiResources.Remove(inMemoryApiResource);
                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        public Task<IdentityAdminResult> SetPropertyAsync(string subject, string type, string value)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                var inMemoryApiResource = _apiResources.FirstOrDefault(p => p.Id == parsedSubject);
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

        protected IdentityAdminResult SetProperty(IEnumerable<PropertyMetadata> propsMeta, InMemoryApiResource apiResource, string type, string value)
        {
            IdentityAdminResult result;
            if (propsMeta.TrySet(apiResource, type, value, out result))
            {
                return result;
            }

            throw new Exception("Invalid property type " + type);
        }
    }
}