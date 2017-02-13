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
using IdentityAdmin.Core;
using IdentityAdmin.Core.IdentityResource;

namespace IdentityAdmin.Api.Models.IdentityResource
{
    class IdentityResourceQueryResultResource
    {
        /// <summary>
        /// Needed for Unit Testing
        /// </summary>
        public IdentityResourceQueryResultResource()
        {
        }

        public IdentityResourceQueryResultResource(QueryResult<IdentityResourceSummary> result, UrlHelper url, IdentityResourceMetaData meta)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (meta == null) throw new ArgumentNullException(nameof(meta));

            Data = new IdentityResourceQueryResultResourceData(result, url, meta);

            var links = new Dictionary<string, object>();
            if (meta.SupportsCreate)
            {
                links["create"] = new CreateIdentityResourceLink(url, meta);
            };
            Links = links;
        }

        public IdentityResourceQueryResultResourceData Data { get; set; }
        public object Links { get; set; }
    }
}
