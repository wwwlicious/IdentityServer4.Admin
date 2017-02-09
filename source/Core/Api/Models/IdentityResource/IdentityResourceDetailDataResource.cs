namespace IdentityAdmin.Api.Models.IdentityResource
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Routing;
    using Core.IdentityResource;

    class IdentityResourceDetailDataResource : Dictionary<string, object>
    {
        public IdentityResourceDetailDataResource(IdentityResourceDetail identityResource, UrlHelper url, IdentityResourceMetaData metaData)
        {
            if (identityResource == null) throw new ArgumentNullException(nameof(identityResource));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (metaData == null) throw new ArgumentNullException(nameof(metaData));
        }
    }
}
