using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Distribution", Schema = "media")]
    public partial class Distribution
    {
        public Distribution()
        {
            CompanyDistribution = new HashSet<CompanyDistribution>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(250)]
        public string DistributionName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("Distribution")]
        public ICollection<CompanyDistribution> CompanyDistribution { get; set; }
    }
}
