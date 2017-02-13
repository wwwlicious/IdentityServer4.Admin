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

namespace IdentityAdmin.Host.InMemoryService
{
    public class InMemoryApiResource
    {
        public InMemoryApiResource()
        {
            Claims = new List<InMemoryApiResourceClaim>();
            Secrets = new List<InMemoryApiResourceSecret>();
            Scopes = new List<InMemoryApiResourceScope>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        public string Description { get; set; }

        public ICollection<InMemoryApiResourceClaim> Claims { get; set; }
        public ICollection<InMemoryApiResourceSecret> Secrets { get; set; }
        public ICollection<InMemoryApiResourceScope> Scopes { get; set; }
    }

    public class InMemoryApiResourceClaim
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class InMemoryApiResourceSecret
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime? Expiration { get; set; }    
        public string Value { get; set; }
    }

    public class InMemoryApiResourceScope
    {
        public InMemoryApiResourceScope()
        {
            Claims = new List<InMemoryApiResourceScopeClaim>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; }
        public bool Required { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }         
        public ICollection<InMemoryApiResourceScopeClaim> Claims { get; set; }
    }

    public class InMemoryApiResourceScopeClaim
    {
        public int Id { get; set; }

        public string Type { get; set; }
    }
}