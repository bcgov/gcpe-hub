using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseImage
    {
        public NewsReleaseImage()
        {
            NewsReleaseImageLanguage = new HashSet<NewsReleaseImageLanguage>();
            NewsReleaseType = new HashSet<NewsReleaseType>();
        }

        public Guid Id { get; set; }
        public int SortOrder { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string MimeType { get; set; }
        public Guid BlobId { get; set; }

        [ForeignKey("BlobId")]
        [InverseProperty("NewsReleaseImage")]
        public Blob Blob { get; set; }
        [InverseProperty("Image")]
        public ICollection<NewsReleaseImageLanguage> NewsReleaseImageLanguage { get; set; }
        [InverseProperty("PageImage")]
        public ICollection<NewsReleaseType> NewsReleaseType { get; set; }
    }
}
