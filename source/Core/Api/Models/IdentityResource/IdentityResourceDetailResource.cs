namespace IdentityAdmin.Api.Models.IdentityResource
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Routing;
    using Core.IdentityResource;
    using Extensions;

    class IdentityResourceDetailResource
    {
        public IdentityResourceDetailResource(IdentityResourceDetail identityResource, UrlHelper url, IdentityResourceMetaData metaData)
        {
            if (identityResource == null) throw new ArgumentNullException(nameof(identityResource));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (metaData == null) throw new ArgumentNullException(nameof(metaData));

            Data = new IdentityResourceDetailDataResource(identityResource, url, metaData);

            var links = new Dictionary<string, string>();
            if (metaData.SupportsDelete)
            {
                links["Delete"] = url.RelativeLink(Constants.RouteNames.DeleteIdentityResource, new { subject = identityResource.Subject });
            }
            Links = links;
        }

        public IdentityResourceDetailDataResource Data { get; set; }
        public object Links { get; set; }
    }
}
