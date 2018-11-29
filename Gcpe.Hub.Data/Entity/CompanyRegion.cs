using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CompanyRegion", Schema = "media")]
    public partial class CompanyRegion
    {
        public Guid CompanyId { get; set; }
        public Guid RegionId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("CompanyRegion")]
        public Company Company { get; set; }
        [ForeignKey("RegionId")]
        [InverseProperty("CompanyRegion")]
        public NewsRegion Region { get; set; }
    }
}
