using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseDocument
    {
        public NewsReleaseDocument()
        {
            NewsReleaseDocumentContact = new HashSet<NewsReleaseDocumentContact>();
            NewsReleaseDocumentLanguage = new HashSet<NewsReleaseDocumentLanguage>();
        }

        public Guid Id { get; set; }
        public Guid ReleaseId { get; set; }
        public int SortIndex { get; set; }
        public PageLayout PageLayout { get; set; }

        [ForeignKey("ReleaseId")]
        [InverseProperty("NewsReleaseDocument")]
        public NewsRelease Release { get; set; }
        [InverseProperty("Document")]
        public ICollection<NewsReleaseDocumentContact> NewsReleaseDocumentContact { get; set; }
        [InverseProperty("Document")]
        public ICollection<NewsReleaseDocumentLanguage> NewsReleaseDocumentLanguage { get; set; }
    }
}
