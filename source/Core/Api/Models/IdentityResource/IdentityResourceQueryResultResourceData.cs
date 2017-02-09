namespace IdentityAdmin.Api.Models.IdentityResource
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Routing;
    using AutoMapper;
    using Core;
    using Core.IdentityResource;
    using Extensions;


    public class IdentityResourceQueryResultResourceData : QueryResult<IdentityResourceSummary>
    {
        public static MapperConfiguration Config;
        public static IMapper Mapper;
        static IdentityResourceQueryResultResourceData()
        {
            Config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<QueryResult<IdentityResourceSummary>, IdentityResourceQueryResultResourceData>()
                .ForMember(x => x.Items, opts => opts.MapFrom(x => x.Items));
                cfg.CreateMap<IdentityResourceSummary, IdentityResourceResultResource>()
                    .ForMember(x => x.Data, opts => opts.MapFrom(x => x));
            });
            Mapper = Config.CreateMapper();
        }

        /// <summary>
        /// Needed for Unit Testing
        /// </summary>
        public IdentityResourceQueryResultResourceData()
        {

        }

        public IdentityResourceQueryResultResourceData(QueryResult<IdentityResourceSummary> result, UrlHelper url, IdentityResourceMetaData meta)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (meta == null) throw new ArgumentNullException(nameof(meta));

            Mapper.Map(result, this);

            foreach (var identityResource in this.Items)
            {
                var links = new Dictionary<string, string>
                {
                    {"detail", url.RelativeLink(Constants.RouteNames.GetIdentityResource, new { subject = identityResource.Data.Subject })}
                };
                if (meta.SupportsDelete)
                {
                    links.Add("delete", url.RelativeLink(Constants.RouteNames.DeleteIdentityResource, new { subject = identityResource.Data.Subject }));
                }
                identityResource.Links = links;
            }
        }

        public new IEnumerable<IdentityResourceResultResource> Items { get; set; }
    }
}
