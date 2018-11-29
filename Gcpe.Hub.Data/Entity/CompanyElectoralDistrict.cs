using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CompanyElectoralDistrict", Schema = "media")]
    public partial class CompanyElectoralDistrict
    {
        public Guid CompanyId { get; set; }
        public Guid DistrictId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("CompanyElectoralDistrict")]
        public Company Company { get; set; }
        [ForeignKey("DistrictId")]
        [InverseProperty("CompanyElectoralDistrict")]
        public ElectoralDistrict District { get; set; }
    }
}
