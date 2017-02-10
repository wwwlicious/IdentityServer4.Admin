namespace IdentityAdmin.Api.Models.ApiResource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Routing;
    using Core.ApiResource;
    using Extensions;

    public class ApiResourceDetailDataResource : Dictionary<string, object>
    {
        public ApiResourceDetailDataResource(ApiResourceDetail apiResource, UrlHelper url, ApiResourceMetaData metaData)
        {
            if (apiResource == null) throw new ArgumentNullException(nameof(apiResource));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (metaData == null) throw new ArgumentNullException(nameof(metaData));

            this["Name"] = apiResource.Name;
            this["Description"] = apiResource.Description;
            this["Subject"] = apiResource.Subject;

            if (apiResource.Properties != null)
            {
                var props = (from p in apiResource.Properties
                            let m = (from m in metaData.UpdateProperties where m.Type == p.Type select m).SingleOrDefault()
                            where m != null
                            select new
                            {
                                Data = m.Convert(p.Value),
                                Meta = m,
                                Links = new
                                {
                                    update = url.RelativeLink(Constants.RouteNames.UpdateApiResourceProperty,
                                        new
                                        {
                                            subject = apiResource.Subject,
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
        }
    }
}
