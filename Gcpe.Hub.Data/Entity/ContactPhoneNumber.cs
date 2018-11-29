using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ContactPhoneNumber", Schema = "media")]
    public partial class ContactPhoneNumber
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
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

        [ForeignKey("ContactId")]
        [InverseProperty("ContactPhoneNumber")]
        public Contact Contact { get; set; }
        [ForeignKey("PhoneTypeId")]
        [InverseProperty("ContactPhoneNumber")]
        public PhoneType PhoneType { get; set; }
    }
}
