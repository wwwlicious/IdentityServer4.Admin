namespace IdentityAdmin.Api.Models.ApiResource
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Routing;
    using Core.ApiResource;

    public class ApiResourceDetailDataResource : Dictionary<string, object>
    {
        public ApiResourceDetailDataResource(ApiResourceDetail apiResource, UrlHelper url, ApiResourceMetaData metaData)
        {
            if (apiResource == null) throw new ArgumentNullException(nameof(apiResource));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (metaData == null) throw new ArgumentNullException(nameof(metaData));
        }
    }
}
