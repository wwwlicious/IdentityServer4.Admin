namespace IdentityAdmin.Api.Models.IdentityResource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Routing;
    using Core.IdentityResource;
    using Extensions;

    class IdentityResourceDetailDataResource : Dictionary<string, object>
    {
        public IdentityResourceDetailDataResource(IdentityResourceDetail identityResource, UrlHelper url, IdentityResourceMetaData metaData)
        {
            if (identityResource == null) throw new ArgumentNullException(nameof(identityResource));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (metaData == null) throw new ArgumentNullException(nameof(metaData));

            this["Name"] = identityResource.Name;
            this["Description"] = identityResource.Description;
            this["Subject"] = identityResource.Subject;

            if (identityResource.Properties != null)
            {
                var props = (from p in identityResource.Properties
                             let m = (from m in metaData.UpdateProperties where m.Type == p.Type select m).SingleOrDefault()
                             where m != null
                             select new
                             {
                                 Data = m.Convert(p.Value),
                                 Meta = m,
                                 Links = new
                                 {
                                     update = url.RelativeLink(Constants.RouteNames.UpdateIdentityResourceProperty,
                                         new
                                         {
                                             subject = identityResource.Subject,
                                             type = p.Type.ToBase64UrlEncoded()
                                         }
                                     )
                                 }
                             }).ToList();

                if (props.Any())
                {
                    this["Properties"] = props.ToArray();
                }
            }

            if (identityResource.IdentityResourceClaims != null)
            {
                var identityResourceClaims = from c in identityResource.IdentityResourceClaims.ToArray()
                    select new
                    {
                        Data = c,
                        Links = new
                        {
                            delete = url.RelativeLink(Constants.RouteNames.RemoveIdentityResourceClaim, new
                            {
                                subject = identityResource.Subject,
                                id = c.Id
                            })
                        }
                    };
                this["Claims"] = new
                {
                    Data = identityResourceClaims.ToArray(),
                    Links = new
                    {
                        create = url.RelativeLink(Constants.RouteNames.AddIdentityResourceClaim, new { subject = identityResource.Subject })
                    }
                };
            }
        }
    }
}
