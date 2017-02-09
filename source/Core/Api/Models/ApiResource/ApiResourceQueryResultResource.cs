namespace IdentityAdmin.Api.Models.ApiResource
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Routing;
    using Core;
    using Core.ApiResource;

    class ApiResourceQueryResultResource
    {
        /// <summary>
        /// Needed for Unit Testing
        /// </summary>
        public ApiResourceQueryResultResource()
        {
        }

        public ApiResourceQueryResultResource(QueryResult<ApiResourceSummary> result, UrlHelper url, ApiResourceMetaData meta)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (meta == null) throw new ArgumentNullException(nameof(meta));

            Data = new ApiResourceQueryResultResourceData(result, url, meta);

            var links = new Dictionary<string, object>();
            if (meta.SupportsCreate)
            {
                links["create"] = new CreateApiResourceLink(url, meta);
            };
            Links = links;
        }

        public ApiResourceQueryResultResourceData Data { get; set; }
        public object Links { get; set; }
    }
}
