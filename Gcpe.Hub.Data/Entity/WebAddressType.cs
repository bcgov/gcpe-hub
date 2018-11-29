using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("WebAddressType", Schema = "media")]
    public partial class WebAddressType
    {
        public WebAddressType()
        {
            CompanyWebAddress = new HashSet<CompanyWebAddress>();
            ContactWebAddress = new HashSet<ContactWebAddress>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string WebAddressTypeName { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("WebAddressType")]
        public ICollection<CompanyWebAddress> CompanyWebAddress { get; set; }
        [InverseProperty("WebAddressType")]
        public ICollection<ContactWebAddress> ContactWebAddress { get; set; }
    }
}
