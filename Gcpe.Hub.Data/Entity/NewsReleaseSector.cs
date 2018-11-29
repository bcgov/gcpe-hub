using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseSector
    {
        public Guid ReleaseId { get; set; }
        public Guid SectorId { get; set; }

        [ForeignKey("ReleaseId")]
        [InverseProperty("NewsReleaseSector")]
        public NewsRelease Release { get; set; }
        [ForeignKey("SectorId")]
        [InverseProperty("NewsReleaseSector")]
        public Sector Sector { get; set; }
    }
}
