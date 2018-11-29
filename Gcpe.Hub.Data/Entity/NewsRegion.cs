using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("NewsRegion", Schema = "media")]
    public partial class NewsRegion
    {
        public NewsRegion()
        {
            CompanyRegion = new HashSet<CompanyRegion>();
            ContactRegion = new HashSet<ContactRegion>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(250)]
        public string RegionName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("Region")]
        public ICollection<CompanyRegion> CompanyRegion { get; set; }
        [InverseProperty("Region")]
        public ICollection<ContactRegion> ContactRegion { get; set; }
    }
}
