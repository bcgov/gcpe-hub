using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseDocumentContact
    {
        public Guid DocumentId { get; set; }
        public int LanguageId { get; set; }
        public int SortIndex { get; set; }
        [Required]
        [StringLength(250)]
        public string Information { get; set; }

        [ForeignKey("DocumentId")]
        [InverseProperty("NewsReleaseDocumentContact")]
        public NewsReleaseDocument Document { get; set; }
        [ForeignKey("LanguageId")]
        [InverseProperty("NewsReleaseDocumentContact")]
        public Language Language { get; set; }
        [ForeignKey("DocumentId,LanguageId")]
        [InverseProperty("NewsReleaseDocumentContact")]
        public NewsReleaseDocumentLanguage NewsReleaseDocumentLanguage { get; set; }
    }
}
