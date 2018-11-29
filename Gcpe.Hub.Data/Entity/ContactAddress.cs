using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ContactAddress", Schema = "media")]
    public partial class ContactAddress
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        [Required]
        [StringLength(250)]
        public string StreetAddress { get; set; }
        public Guid? CityId { get; set; }
        [StringLength(150)]
        public string CityName { get; set; }
        public Guid? ProvStateId { get; set; }
        [StringLength(150)]
        public string ProvStateName { get; set; }
        [Required]
        [StringLength(50)]
        public string PostalZipCode { get; set; }
        public Guid CountryId { get; set; }
        public int AddressType { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }

        [ForeignKey("CityId")]
        [InverseProperty("ContactAddress")]
        public ContactCity City { get; set; }
        [ForeignKey("ContactId")]
        [InverseProperty("ContactAddress")]
        public Contact Contact { get; set; }
        [ForeignKey("CountryId")]
        [InverseProperty("ContactAddress")]
        public Country Country { get; set; }
        [ForeignKey("ProvStateId")]
        [InverseProperty("ContactAddress")]
        public ProvState ProvState { get; set; }
    }
}
