namespace IdentityAdmin.Core.ApiResource
{
    using System.ComponentModel.DataAnnotations;

    public class ApiResourceScopeValue : BaseApiResourceValue
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool Emphasize { get; set; }

        public bool Required { get; set; }

        public bool ShowInDiscoveryDocument { get; set; }
    }
}
