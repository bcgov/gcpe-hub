using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ContactWebAddress", Schema = "media")]
    public partial class ContactWebAddress
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        [Required]
        [StringLength(250)]
        public string WebAddress { get; set; }
        public Guid WebAddressTypeId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }

        [ForeignKey("ContactId")]
        [InverseProperty("ContactWebAddress")]
        public Contact Contact { get; set; }
        [ForeignKey("WebAddressTypeId")]
        [InverseProperty("ContactWebAddress")]
        public WebAddressType WebAddressType { get; set; }
    }
}
