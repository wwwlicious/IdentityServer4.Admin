namespace IdentityAdmin.Core.ApiResource
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ApiResourceSecretValue : BaseApiResourceValue
    {
        [Required]
        public string Type { get; set; }

        [Required]
        public string Value { get; set; }

        public string Description { get; set; }

        public DateTime? Expiration { get; set; }
    }
}
