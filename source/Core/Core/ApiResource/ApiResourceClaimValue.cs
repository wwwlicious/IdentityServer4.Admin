namespace IdentityAdmin.Core.ApiResource
{
    using System.ComponentModel.DataAnnotations;

    public class ApiResourceClaimValue : BaseApiResourceValue
    {
        [Required]
        public string Type { get; set; }
    }
}
