namespace IdentityAdmin.Host.InMemoryService
{
    public class InMemoryApiResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        public string Description { get; set; }
    }
}