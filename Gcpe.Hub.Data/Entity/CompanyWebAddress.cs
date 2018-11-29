using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CompanyWebAddress", Schema = "media")]
    public partial class CompanyWebAddress
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        [Required]
        [StringLength(250)]
        public string WebAddress { get; set; }
        public Guid WebAddressTypeId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("CompanyWebAddress")]
        public Company Company { get; set; }
        [ForeignKey("WebAddressTypeId")]
        [InverseProperty("CompanyWebAddress")]
        public WebAddressType WebAddressType { get; set; }
    }
}
