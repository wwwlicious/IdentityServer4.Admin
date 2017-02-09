namespace IdentityAdmin.Api.Models.ApiResource
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Routing;
    using Core.ApiResource;
    using Extensions;

    public class ApiResourceDetailResource
    {
        public ApiResourceDetailResource(ApiResourceDetail apiResource, UrlHelper url, ApiResourceMetaData metaData)
        {
            if (apiResource == null) throw new ArgumentNullException(nameof(apiResource));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (metaData == null) throw new ArgumentNullException(nameof(metaData));

            Data = new ApiResourceDetailDataResource(apiResource, url, metaData);

            var links = new Dictionary<string, string>();
            if (metaData.SupportsDelete)
            {
                links["Delete"] = url.RelativeLink(Constants.RouteNames.DeleteApiResource, new { subject = apiResource.Subject });
            }
            Links = links;
        }

        public ApiResourceDetailDataResource Data { get; set; }
        public object Links { get; set; }
    }
}
