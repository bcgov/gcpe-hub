using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseMinistry
    {
        public Guid ReleaseId { get; set; }
        public Guid MinistryId { get; set; }

        [ForeignKey("MinistryId")]
        [InverseProperty("NewsReleaseMinistry")]
        public Ministry Ministry { get; set; }
        [ForeignKey("ReleaseId")]
        [InverseProperty("NewsReleaseMinistry")]
        public NewsRelease Release { get; set; }
    }
}
