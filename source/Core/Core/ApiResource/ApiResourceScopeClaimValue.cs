namespace IdentityAdmin.Core.ApiResource
{
    using System.ComponentModel.DataAnnotations;

    public class ApiResourceScopeClaimValue : BaseApiResourceValue
    {
        [Required]
        public string Type { get; set; }
    }
}
