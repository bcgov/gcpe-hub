using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Country", Schema = "media")]
    public partial class Country
    {
        public Country()
        {
            CompanyAddress = new HashSet<CompanyAddress>();
            ContactAddress = new HashSet<ContactAddress>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(250)]
        public string CountryName { get; set; }
        [Required]
        [StringLength(15)]
        public string CountryAbbrev { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("Country")]
        public ICollection<CompanyAddress> CompanyAddress { get; set; }
        [InverseProperty("Country")]
        public ICollection<ContactAddress> ContactAddress { get; set; }
    }
}
