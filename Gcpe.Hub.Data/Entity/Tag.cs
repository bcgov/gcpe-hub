using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class Tag
    {
        public Tag()
        {
            NewsReleaseTag = new HashSet<NewsReleaseTag>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Key { get; set; }
        [StringLength(255)]
        public string DisplayName { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }

        [InverseProperty("Tag")]
        public ICollection<NewsReleaseTag> NewsReleaseTag { get; set; }
    }
}
