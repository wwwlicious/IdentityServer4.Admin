namespace IdentityAdmin.Host.InMemoryService
{
    using System;
    using System.Collections.Generic;

    public class InMemoryApiResource
    {
        public InMemoryApiResource()
        {
            Claims = new List<InMemoryApiResourceClaim>();
            Secrets = new List<InMemoryApiResourceSecret>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        public string Description { get; set; }

        public ICollection<InMemoryApiResourceClaim> Claims { get; set; }
        public ICollection<InMemoryApiResourceSecret> Secrets { get; set; }
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
}