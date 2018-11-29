using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ElectoralDistrict", Schema = "media")]
    public partial class ElectoralDistrict
    {
        public ElectoralDistrict()
        {
            CompanyElectoralDistrict = new HashSet<CompanyElectoralDistrict>();
            Contact = new HashSet<Contact>();
            ContactElectoralDistrict = new HashSet<ContactElectoralDistrict>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(250)]
        public string DistrictName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("District")]
        public ICollection<CompanyElectoralDistrict> CompanyElectoralDistrict { get; set; }
        [InverseProperty("Mlaassignment")]
        public ICollection<Contact> Contact { get; set; }
        [InverseProperty("District")]
        public ICollection<ContactElectoralDistrict> ContactElectoralDistrict { get; set; }
    }
}
