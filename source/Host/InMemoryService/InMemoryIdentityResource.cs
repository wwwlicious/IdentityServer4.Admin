﻿namespace IdentityAdmin.Host.InMemoryService
{
    public class InMemoryIdentityResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; }
        public bool Required { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
    }
}