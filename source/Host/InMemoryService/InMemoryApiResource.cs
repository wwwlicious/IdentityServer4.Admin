namespace IdentityAdmin.Host.InMemoryService
{
    using System.Collections.Generic;

    public class InMemoryApiResource
    {
        public InMemoryApiResource()
        {
            Claims = new List<InMemoryApiResourceClaim>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        public string Description { get; set; }

        public ICollection<InMemoryApiResourceClaim> Claims { get; set; }
    }

    public class InMemoryApiResourceClaim
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
}