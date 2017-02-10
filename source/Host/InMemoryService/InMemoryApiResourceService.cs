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
        public static MapperConfiguration Config;
        public static IMapper Mapper;

        public InMemoryApiResourceService(ICollection<InMemoryApiResource> apiResources)
        {
            this._apiResources = apiResources;

            Config = new MapperConfiguration(cfg => 
            {


                //cfg.CreateMap<InMemoryIdentityResource, Scope>();
                //cfg.CreateMap<Scope, InMemoryIdentityResource>();
            });
            Mapper = Config.CreateMapper();
        }

        private ApiResourceMetaData _metadata;

        private ApiResourceMetaData GetMetadata()
        {
            if (_metadata == null)
            {
                var updateIdentityResource = new List<PropertyMetadata>();
                updateIdentityResource.AddRange(PropertyMetadata.FromType<InMemoryApiResource>());

                var createIdentityResource = new List<PropertyMetadata>
                {
                    PropertyMetadata.FromProperty<InMemoryApiResource>(x => x.Name, "ApiResourceName", required: true),
                };

                _metadata = new ApiResourceMetaData
                {
                    SupportsCreate = true,
                    SupportsDelete = true,
                    CreateProperties = createIdentityResource,
                    UpdateProperties = updateIdentityResource
                };
            }
            return _metadata;
        }


        public Task<ApiResourceMetaData> GetMetadataAsync()
        {
            return Task.FromResult(GetMetadata());
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

                SetScopeProperty(meta.UpdateProperties, inMemoryApiResource, type, value);

                return Task.FromResult(IdentityAdminResult.Success);
            }
            return Task.FromResult(new IdentityAdminResult("Invalid subject"));
        }

        protected IdentityAdminResult SetScopeProperty(IEnumerable<PropertyMetadata> propsMeta, InMemoryApiResource apiResource, string type, string value)
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