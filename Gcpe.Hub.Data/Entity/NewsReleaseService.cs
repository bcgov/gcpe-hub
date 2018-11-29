using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseService
    {
        public Guid ReleaseId { get; set; }
        public Guid ServiceId { get; set; }

        [ForeignKey("ReleaseId")]
        [InverseProperty("NewsReleaseService")]
        public NewsRelease Release { get; set; }
        [ForeignKey("ServiceId")]
        [InverseProperty("NewsReleaseService")]
        public Service Service { get; set; }
    }
}
