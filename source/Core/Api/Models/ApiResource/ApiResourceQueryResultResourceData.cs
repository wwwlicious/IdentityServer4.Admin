namespace IdentityAdmin.Api.Models.ApiResource
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Routing;
    using AutoMapper;
    using Core;
    using Core.ApiResource;
    using Extensions;

    public class ApiResourceQueryResultResourceData : QueryResult<ApiResourceSummary>
    {
        public static MapperConfiguration Config;
        public static IMapper Mapper;
        static ApiResourceQueryResultResourceData()
        {
            Config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<QueryResult<ApiResourceSummary>, ApiResourceQueryResultResourceData>()
                .ForMember(x => x.Items, opts => opts.MapFrom(x => x.Items));
                cfg.CreateMap<ApiResourceSummary, ApiResourceResultResource>()
                    .ForMember(x => x.Data, opts => opts.MapFrom(x => x));
            });
            Mapper = Config.CreateMapper();
        }

        /// <summary>
        /// Needed for Unit Testing
        /// </summary>
        public ApiResourceQueryResultResourceData()
        {
        }

        public ApiResourceQueryResultResourceData(QueryResult<ApiResourceSummary> result, UrlHelper url, ApiResourceMetaData meta)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (meta == null) throw new ArgumentNullException(nameof(meta));

            Mapper.Map(result, this);

            foreach (var identityResource in this.Items)
            {
                var links = new Dictionary<string, string>
                {
                    {"detail", url.RelativeLink(Constants.RouteNames.GetApiResource, new { subject = identityResource.Data.Subject })}
                };
                if (meta.SupportsDelete)
                {
                    links.Add("delete", url.RelativeLink(Constants.RouteNames.DeleteApiResource, new { subject = identityResource.Data.Subject }));
                }
                identityResource.Links = links;
            }
        }

        public new IEnumerable<ApiResourceResultResource> Items { get; set; }
    }
}
