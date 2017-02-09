namespace IdentityAdmin.Api.Models.IdentityResource
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Routing;
    using Core;
    using Core.IdentityResource;

    class IdentityResourceQueryResultResource
    {
        /// <summary>
        /// Needed for Unit Testing
        /// </summary>
        public IdentityResourceQueryResultResource()
        {
        }

        public IdentityResourceQueryResultResource(QueryResult<IdentityResourceSummary> result, UrlHelper url, IdentityResourceMetaData meta)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (meta == null) throw new ArgumentNullException(nameof(meta));

            Data = new IdentityResourceQueryResultResourceData(result, url, meta);

            var links = new Dictionary<string, object>();
            if (meta.SupportsCreate)
            {
                links["create"] = new CreateIdentityResourceLink(url, meta);
            };
            Links = links;
        }

        public IdentityResourceQueryResultResourceData Data { get; set; }
        public object Links { get; set; }
    }
}
