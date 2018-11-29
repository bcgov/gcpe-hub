using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CompanySector", Schema = "media")]
    public partial class CompanySector
    {
        public Guid CompanyId { get; set; }
        public Guid SectorId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("CompanySector")]
        public Company Company { get; set; }
        [ForeignKey("SectorId")]
        [InverseProperty("CompanySector")]
        public Sector Sector { get; set; }
    }
}
