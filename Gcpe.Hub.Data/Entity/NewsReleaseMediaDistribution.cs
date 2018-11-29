using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseMediaDistribution
    {
        public Guid ReleaseId { get; set; }
        public Guid MediaDistributionListId { get; set; }

        [ForeignKey("MediaDistributionListId")]
        [InverseProperty("NewsReleaseMediaDistribution")]
        public MediaDistributionList MediaDistributionList { get; set; }
        [ForeignKey("ReleaseId")]
        [InverseProperty("NewsReleaseMediaDistribution")]
        public NewsRelease Release { get; set; }
    }
}
