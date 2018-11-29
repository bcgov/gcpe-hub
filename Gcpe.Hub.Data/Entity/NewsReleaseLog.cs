using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseLog
    {
        public int Id { get; set; }
        public Guid ReleaseId { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public Guid? UserId { get; set; }
        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [ForeignKey("ReleaseId")]
        [InverseProperty("NewsReleaseLog")]
        public NewsRelease Release { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("NewsReleaseLog")]
        public User User { get; set; }
    }
}
