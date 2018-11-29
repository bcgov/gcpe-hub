using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CompanyPhoneNumber", Schema = "media")]
    public partial class CompanyPhoneNumber
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        [Required]
        [StringLength(50)]
        public string PhoneNumber { get; set; }
        public Guid PhoneTypeId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        [StringLength(15)]
        public string PhoneNumberExtension { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("CompanyPhoneNumber")]
        public Company Company { get; set; }
        [ForeignKey("PhoneTypeId")]
        [InverseProperty("CompanyPhoneNumber")]
        public PhoneType PhoneType { get; set; }
    }
}
