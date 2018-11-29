using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class MinistrySector
    {
        public Guid MinistryId { get; set; }
        public Guid SectorId { get; set; }

        [ForeignKey("MinistryId")]
        [InverseProperty("MinistrySector")]
        public Ministry Ministry { get; set; }
        [ForeignKey("SectorId")]
        [InverseProperty("MinistrySector")]
        public Sector Sector { get; set; }
    }
}
