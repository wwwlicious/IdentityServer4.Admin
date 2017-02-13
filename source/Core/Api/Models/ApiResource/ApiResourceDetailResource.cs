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
using System.Web.Http.Routing;
using IdentityAdmin.Core.ApiResource;
using IdentityAdmin.Extensions;

namespace IdentityAdmin.Api.Models.ApiResource
{
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
