using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseTag
    {
        public Guid ReleaseId { get; set; }
        public Guid TagId { get; set; }

        [ForeignKey("ReleaseId")]
        [InverseProperty("NewsReleaseTag")]
        public NewsRelease Release { get; set; }
        [ForeignKey("TagId")]
        [InverseProperty("NewsReleaseTag")]
        public Tag Tag { get; set; }
    }
}
