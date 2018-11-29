using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class MediaDistributionList
    {
        public MediaDistributionList()
        {
            NewsReleaseMediaDistribution = new HashSet<NewsReleaseMediaDistribution>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Key { get; set; }
        [Required]
        [StringLength(50)]
        public string DisplayName { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }

        [InverseProperty("MediaDistributionList")]
        public ICollection<NewsReleaseMediaDistribution> NewsReleaseMediaDistribution { get; set; }
    }
}
