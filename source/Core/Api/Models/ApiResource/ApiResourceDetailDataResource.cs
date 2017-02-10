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

            this["Claims"] = new
            {
                Data = GetClaims(apiResource, url).ToArray(),
                Links = new
                {
                    create = url.RelativeLink(Constants.RouteNames.AddApiResourceClaim, new { subject = apiResource.Subject })
                }
            };

            this["Secrets"] = new
            {
                Data = GetSecrets(apiResource, url).ToArray(),
                Links = new
                {
                    create = url.RelativeLink(Constants.RouteNames.AddApiResourceSecret, new { subject = apiResource.Subject })
                }
            };

            this["Scopes"] = new
            {
                Data = GetScopes(apiResource, url).ToArray(),
                Links = new
                {
                    create = url.RelativeLink(Constants.RouteNames.AddApiResourceScope, new { subject = apiResource.Subject })                    
                }
            };
        }

        private IEnumerable<object> GetClaims(ApiResourceDetail apiResource, UrlHelper url)
        {
            if (apiResource.ResourceClaims != null)
            {
                return from c in apiResource.ResourceClaims.ToArray()
                    select new
                    {
                        Data = c,
                        Links = new
                        {
                            delete = url.RelativeLink(Constants.RouteNames.RemoveApiResourceClaim, new
                            {
                                subject = apiResource.Subject,
                                id = c.Id
                            })
                        }
                    };
            }
            return new object[0];
        }

        private IEnumerable<object> GetSecrets(ApiResourceDetail apiResource, UrlHelper url)
        {
            if (apiResource.ResourceSecrets != null)
            {
                return from c in apiResource.ResourceSecrets
                    select new
                    {
                        Data = c,
                        Links = new
                        {
                            update = url.RelativeLink(Constants.RouteNames.UpdateApiResourceSecret, new
                            {
                               subject = apiResource.Subject,
                               id = c.Id 
                            }),
                            delete = url.RelativeLink(Constants.RouteNames.RemoveApiResourceSecret, new
                            {
                                subject = apiResource.Subject,
                                id = c.Id
                            })
                        }
                    };
            }
            return new object[0];            
        }

        private IEnumerable<object> GetScopes(ApiResourceDetail apiResource, UrlHelper url)
        {
            if (apiResource.ResourceScopes != null)
            {
                return from c in apiResource.ResourceScopes
                        select new
                        {
                            Data = c,
                            Links = new
                            {
                                update = url.RelativeLink(Constants.RouteNames.UpdateApiResourceScope, new
                                {
                                    subject = apiResource.Subject,
                                    id = c.Id
                                }),
                                delete = url.RelativeLink(Constants.RouteNames.RemoveApiResourceScope, new
                                {
                                    subject = apiResource.Subject,
                                    id = c.Id
                                })
                            }
                        };
            }
            return new object[0];
        }
    }
}
