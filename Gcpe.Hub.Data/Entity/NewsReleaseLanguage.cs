using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseLanguage
    {
        public Guid ReleaseId { get; set; }
        public int LanguageId { get; set; }
        [Required]
        [StringLength(50)]
        public string Location { get; set; }
        [Required]
        public string Summary { get; set; }
        public string SocialMediaHeadline { get; set; }
        public string SocialMediaSummary { get; set; }

        [ForeignKey("LanguageId")]
        [InverseProperty("NewsReleaseLanguage")]
        public Language Language { get; set; }
        [ForeignKey("ReleaseId")]
        [InverseProperty("NewsReleaseLanguage")]
        public NewsRelease Release { get; set; }
    }
}
