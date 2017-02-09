namespace IdentityAdmin.Core.IdentityResource
{
    using System.ComponentModel.DataAnnotations;

    public class IdentityResourceClaimValue : BaseIdentityResourceValue
    {
        [Required]
        public string Type { get; set; }
    }
}
