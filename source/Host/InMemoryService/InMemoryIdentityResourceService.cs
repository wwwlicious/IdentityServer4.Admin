namespace IdentityAdmin.Host.InMemoryService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Core;
    using Core.IdentityResource;
    using Core.Metadata;

    public class InMemoryIdentityResourceService : IIdentityResourceService
    {
        private readonly ICollection<InMemoryIdentityResource> _identityResources;
        public static MapperConfiguration Config;
        public static IMapper Mapper;

        public InMemoryIdentityResourceService(ICollection<InMemoryIdentityResource> identityResources)
        {
            this._identityResources = identityResources;

            Config = new MapperConfiguration(cfg => {


                //cfg.CreateMap<InMemoryIdentityResource, Scope>();
                //cfg.CreateMap<Scope, InMemoryIdentityResource>();
            });
            Mapper = Config.CreateMapper();
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

                return Task.FromResult(new IdentityAdminResult<IdentityResourceDetail>(result));
            }
            return Task.FromResult(new IdentityAdminResult<IdentityResourceDetail>((IdentityResourceDetail)null));
        }
    }
}