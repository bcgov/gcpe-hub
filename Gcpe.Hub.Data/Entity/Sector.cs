using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class Sector
    {
        public Sector()
        {
            ActivitySectors = new HashSet<ActivitySectors>();
            CompanySector = new HashSet<CompanySector>();
            ContactSector = new HashSet<ContactSector>();
            MinistrySector = new HashSet<MinistrySector>();
            NewsReleaseSector = new HashSet<NewsReleaseSector>();
            SectorLanguage = new HashSet<SectorLanguage>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Key { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        [StringLength(255)]
        public string DisplayName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Timestamp { get; set; }
        [Required]
        public string MiscHtml { get; set; }
        [Required]
        public string MiscRightHtml { get; set; }
        [Required]
        [StringLength(255)]
        public string TwitterUsername { get; set; }
        [Required]
        [StringLength(255)]
        public string FlickrUrl { get; set; }
        [Required]
        [StringLength(255)]
        public string YoutubeUrl { get; set; }
        [Required]
        [StringLength(255)]
        public string AudioUrl { get; set; }
        [Required]
        public string FacebookEmbedHtml { get; set; }
        [Required]
        public string YoutubeEmbedHtml { get; set; }
        [Required]
        public string AudioEmbedHtml { get; set; }
        public Guid? TopReleaseId { get; set; }
        public Guid? FeatureReleaseId { get; set; }

        [ForeignKey("FeatureReleaseId")]
        [InverseProperty("SectorFeatureRelease")]
        public NewsRelease FeatureRelease { get; set; }
        [ForeignKey("TopReleaseId")]
        [InverseProperty("SectorTopRelease")]
        public NewsRelease TopRelease { get; set; }
        [InverseProperty("Sector")]
        public ICollection<ActivitySectors> ActivitySectors { get; set; }
        [InverseProperty("Sector")]
        public ICollection<CompanySector> CompanySector { get; set; }
        [InverseProperty("Sector")]
        public ICollection<ContactSector> ContactSector { get; set; }
        [InverseProperty("Sector")]
        public ICollection<MinistrySector> MinistrySector { get; set; }
        [InverseProperty("Sector")]
        public ICollection<NewsReleaseSector> NewsReleaseSector { get; set; }
        [InverseProperty("Sector")]
        public ICollection<SectorLanguage> SectorLanguage { get; set; }
    }
}
