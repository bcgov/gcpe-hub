using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseImageLanguage
    {
        public NewsReleaseImageLanguage()
        {
            NewsReleaseDocumentLanguage = new HashSet<NewsReleaseDocumentLanguage>();
        }

        public Guid ImageId { get; set; }
        public int LanguageId { get; set; }
        [Required]
        [StringLength(100)]
        public string AlternateName { get; set; }

        [ForeignKey("ImageId")]
        [InverseProperty("NewsReleaseImageLanguage")]
        public NewsReleaseImage Image { get; set; }
        [ForeignKey("LanguageId")]
        [InverseProperty("NewsReleaseImageLanguage")]
        public Language Language { get; set; }
        [InverseProperty("NewsReleaseImageLanguage")]
        public ICollection<NewsReleaseDocumentLanguage> NewsReleaseDocumentLanguage { get; set; }
    }
}
