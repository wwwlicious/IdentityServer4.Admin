namespace IdentityAdmin.Host.InMemoryService
{
    using System.Collections.Generic;

    public class InMemoryIdentityResource
    {
        public InMemoryIdentityResource()
        {
            Claims = new List<InMemoryIdentityResourceClaim>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; }
        public bool Required { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }

        public ICollection<InMemoryIdentityResourceClaim> Claims { get; set; }
    }

    public class InMemoryIdentityResourceClaim
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
}