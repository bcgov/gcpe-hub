using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseDocumentLanguage
    {
        public NewsReleaseDocumentLanguage()
        {
            NewsReleaseDocumentContact = new HashSet<NewsReleaseDocumentContact>();
        }

        public Guid DocumentId { get; set; }
        public int LanguageId { get; set; }
        public Guid? PageImageId { get; set; }
        [Required]
        [StringLength(50)]
        public string PageTitle { get; set; }
        [Required]
        [StringLength(250)]
        public string Organizations { get; set; }
        [Required]
        [StringLength(255)]
        public string Headline { get; set; }
        [Required]
        [StringLength(100)]
        public string Subheadline { get; set; }
        [Required]
        [StringLength(250)]
        public string Byline { get; set; }
        [Required]
        public string BodyHtml { get; set; }

        [ForeignKey("DocumentId")]
        [InverseProperty("NewsReleaseDocumentLanguage")]
        public NewsReleaseDocument Document { get; set; }
        [ForeignKey("LanguageId")]
        [InverseProperty("NewsReleaseDocumentLanguage")]
        public Language Language { get; set; }
        [ForeignKey("PageImageId,LanguageId")]
        [InverseProperty("NewsReleaseDocumentLanguage")]
        public NewsReleaseImageLanguage NewsReleaseImageLanguage { get; set; }
        [InverseProperty("NewsReleaseDocumentLanguage")]
        public ICollection<NewsReleaseDocumentContact> NewsReleaseDocumentContact { get; set; }
    }
}
