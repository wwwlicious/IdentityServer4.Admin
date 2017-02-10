namespace IdentityAdmin.Api.Models.ApiResource
{
    using System.Collections.Generic;
    using System.Web.Http.Routing;
    using Core.ApiResource;
    using Extensions;

    public class CreateApiResourceLink : Dictionary<string, object>
    {
        public CreateApiResourceLink(UrlHelper url, ApiResourceMetaData apiResourceMetaData)
        {
            this["href"] = url.RelativeLink(Constants.RouteNames.CreateApiResource);
            this["meta"] = apiResourceMetaData.CreateProperties;
        }
    }
}
