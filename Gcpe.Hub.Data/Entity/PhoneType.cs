using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("PhoneType", Schema = "media")]
    public partial class PhoneType
    {
        public PhoneType()
        {
            CompanyPhoneNumber = new HashSet<CompanyPhoneNumber>();
            ContactPhoneNumber = new HashSet<ContactPhoneNumber>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string PhoneTypeName { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("PhoneType")]
        public ICollection<CompanyPhoneNumber> CompanyPhoneNumber { get; set; }
        [InverseProperty("PhoneType")]
        public ICollection<ContactPhoneNumber> ContactPhoneNumber { get; set; }
    }
}
