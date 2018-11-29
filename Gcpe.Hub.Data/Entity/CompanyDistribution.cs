using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CompanyDistribution", Schema = "media")]
    public partial class CompanyDistribution
    {
        public Guid CompanyId { get; set; }
        public Guid DistributionId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("CompanyDistribution")]
        public Company Company { get; set; }
        [ForeignKey("DistributionId")]
        [InverseProperty("CompanyDistribution")]
        public Distribution Distribution { get; set; }
    }
}
