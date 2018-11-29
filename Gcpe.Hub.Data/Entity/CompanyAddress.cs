using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CompanyAddress", Schema = "media")]
    public partial class CompanyAddress
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
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
        [InverseProperty("CompanyAddress")]
        public ContactCity City { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("CompanyAddress")]
        public Company Company { get; set; }
        [ForeignKey("CountryId")]
        [InverseProperty("CompanyAddress")]
        public Country Country { get; set; }
        [ForeignKey("ProvStateId")]
        [InverseProperty("CompanyAddress")]
        public ProvState ProvState { get; set; }
    }
}
