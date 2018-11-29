using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseCollection
    {
        public NewsReleaseCollection()
        {
            NewsRelease = new HashSet<NewsRelease>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [InverseProperty("Collection")]
        public ICollection<NewsRelease> NewsRelease { get; set; }
    }
}
