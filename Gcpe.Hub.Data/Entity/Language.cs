using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class Language
    {
        public Language()
        {
            MinistryLanguage = new HashSet<MinistryLanguage>();
            NewsReleaseDocumentContact = new HashSet<NewsReleaseDocumentContact>();
            NewsReleaseDocumentLanguage = new HashSet<NewsReleaseDocumentLanguage>();
            NewsReleaseImageLanguage = new HashSet<NewsReleaseImageLanguage>();
            NewsReleaseLanguage = new HashSet<NewsReleaseLanguage>();
            NewsReleaseType = new HashSet<NewsReleaseType>();
            SectorLanguage = new HashSet<SectorLanguage>();
        }

        public int Id { get; set; }
        public int SortOrder { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [InverseProperty("Language")]
        public ICollection<MinistryLanguage> MinistryLanguage { get; set; }
        [InverseProperty("Language")]
        public ICollection<NewsReleaseDocumentContact> NewsReleaseDocumentContact { get; set; }
        [InverseProperty("Language")]
        public ICollection<NewsReleaseDocumentLanguage> NewsReleaseDocumentLanguage { get; set; }
        [InverseProperty("Language")]
        public ICollection<NewsReleaseImageLanguage> NewsReleaseImageLanguage { get; set; }
        [InverseProperty("Language")]
        public ICollection<NewsReleaseLanguage> NewsReleaseLanguage { get; set; }
        [InverseProperty("Language")]
        public ICollection<NewsReleaseType> NewsReleaseType { get; set; }
        [InverseProperty("Language")]
        public ICollection<SectorLanguage> SectorLanguage { get; set; }
    }
}
