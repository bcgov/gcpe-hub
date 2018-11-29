using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseType
    {
        [StringLength(50)]
        public string PageTitle { get; set; }
        public int LanguageId { get; set; }
        public int ReleaseType { get; set; }
        public int SortOrder { get; set; }
        public PageLayout PageLayout { get; set; }
        public Guid? PageImageId { get; set; }

        [ForeignKey("LanguageId")]
        [InverseProperty("NewsReleaseType")]
        public Language Language { get; set; }
        [ForeignKey("PageImageId")]
        [InverseProperty("NewsReleaseType")]
        public NewsReleaseImage PageImage { get; set; }
    }
}
