using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsRelease
    {
        public NewsRelease()
        {
            MinistryFeatureRelease = new HashSet<Ministry>();
            MinistryTopRelease = new HashSet<Ministry>();
            NewsReleaseDocument = new HashSet<NewsReleaseDocument>();
            NewsReleaseHistory = new HashSet<NewsReleaseHistory>();
            NewsReleaseLanguage = new HashSet<NewsReleaseLanguage>();
            NewsReleaseLog = new HashSet<NewsReleaseLog>();
            NewsReleaseMediaDistribution = new HashSet<NewsReleaseMediaDistribution>();
            NewsReleaseMinistry = new HashSet<NewsReleaseMinistry>();
            NewsReleaseSector = new HashSet<NewsReleaseSector>();
            NewsReleaseService = new HashSet<NewsReleaseService>();
            NewsReleaseTag = new HashSet<NewsReleaseTag>();
            NewsReleaseTheme = new HashSet<NewsReleaseTheme>();
            SectorFeatureRelease = new HashSet<Sector>();
            SectorTopRelease = new HashSet<Sector>();
            ThemeFeatureRelease = new HashSet<Theme>();
            ThemeTopRelease = new HashSet<Theme>();
        }

        public Guid Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public ReleaseType ReleaseType { get; set; }
        [Required]
        [StringLength(255)]
        public string Key { get; set; }
        public Guid CollectionId { get; set; }
        public int? Year { get; set; }
        public int? YearRelease { get; set; }
        public Guid? MinistryId { get; set; }
        public int? MinistryRelease { get; set; }
        [Required]
        [StringLength(50)]
        public string Reference { get; set; }
        public int? ActivityId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ReleaseDateTime { get; set; }
        public DateTimeOffset? PublishDateTime { get; set; }
        public bool IsCommitted { get; set; }
        public bool IsPublished { get; set; }
        public PublishOptions PublishOptions { get; set; }
        public bool IsActive { get; set; }
        public bool HasMediaAssets { get; set; }
        public int? NodSubscribers { get; set; }
        public int? MediaSubscribers { get; set; }
        [Required]
        [StringLength(255)]
        public string AtomId { get; set; }
        [Required]
        public string Keywords { get; set; }
        [Required]
        public string AssetUrl { get; set; }
        [Required]
        public string RedirectUrl { get; set; }

        [ForeignKey("CollectionId")]
        [InverseProperty("NewsRelease")]
        public NewsReleaseCollection Collection { get; set; }
        [ForeignKey("MinistryId")]
        [InverseProperty("NewsRelease")]
        public Ministry Ministry { get; set; }
        [InverseProperty("FeatureRelease")]
        public ICollection<Ministry> MinistryFeatureRelease { get; set; }
        [InverseProperty("TopRelease")]
        public ICollection<Ministry> MinistryTopRelease { get; set; }
        [InverseProperty("Release")]
        public ICollection<NewsReleaseDocument> NewsReleaseDocument { get; set; }
        [InverseProperty("Release")]
        public ICollection<NewsReleaseHistory> NewsReleaseHistory { get; set; }
        [InverseProperty("Release")]
        public ICollection<NewsReleaseLanguage> NewsReleaseLanguage { get; set; }
        [InverseProperty("Release")]
        public ICollection<NewsReleaseLog> NewsReleaseLog { get; set; }
        [InverseProperty("Release")]
        public ICollection<NewsReleaseMediaDistribution> NewsReleaseMediaDistribution { get; set; }
        [InverseProperty("Release")]
        public ICollection<NewsReleaseMinistry> NewsReleaseMinistry { get; set; }
        [InverseProperty("Release")]
        public ICollection<NewsReleaseSector> NewsReleaseSector { get; set; }
        [InverseProperty("Release")]
        public ICollection<NewsReleaseService> NewsReleaseService { get; set; }
        [InverseProperty("Release")]
        public ICollection<NewsReleaseTag> NewsReleaseTag { get; set; }
        [InverseProperty("Release")]
        public ICollection<NewsReleaseTheme> NewsReleaseTheme { get; set; }
        [InverseProperty("FeatureRelease")]
        public ICollection<Sector> SectorFeatureRelease { get; set; }
        [InverseProperty("TopRelease")]
        public ICollection<Sector> SectorTopRelease { get; set; }
        [InverseProperty("FeatureRelease")]
        public ICollection<Theme> ThemeFeatureRelease { get; set; }
        [InverseProperty("TopRelease")]
        public ICollection<Theme> ThemeTopRelease { get; set; }
    }
}
