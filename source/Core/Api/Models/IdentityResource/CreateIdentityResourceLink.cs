namespace IdentityAdmin.Api.Models.IdentityResource
{
    using System.Collections.Generic;
    using System.Web.Http.Routing;
    using Core.IdentityResource;
    using Extensions;

    public class CreateIdentityResourceLink : Dictionary<string, object>
    {
        public CreateIdentityResourceLink(UrlHelper url, IdentityResourceMetaData identityResourceMetaData)
        {
            this["href"] = url.RelativeLink(Constants.RouteNames.CreateIdentityResource);
            this["meta"] = identityResourceMetaData.CreateProperties;
        }
    }
}
