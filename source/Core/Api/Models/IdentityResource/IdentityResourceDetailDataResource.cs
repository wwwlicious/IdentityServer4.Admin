/*
 * Copyright 2014 Dominick Baier, Brock Allen, Bert Hoorne
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;
using IdentityAdmin.Core.IdentityResource;
using IdentityAdmin.Extensions;

namespace IdentityAdmin.Api.Models.IdentityResource
{
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
