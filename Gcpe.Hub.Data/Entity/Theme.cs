using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class Theme
    {
        public Theme()
        {
            ActivityThemes = new HashSet<ActivityThemes>();
            NewsReleaseTheme = new HashSet<NewsReleaseTheme>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Key { get; set; }
        [StringLength(255)]
        public string DisplayName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Timestamp { get; set; }
        public Guid? TopReleaseId { get; set; }
        public Guid? FeatureReleaseId { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("FeatureReleaseId")]
        [InverseProperty("ThemeFeatureRelease")]
        public NewsRelease FeatureRelease { get; set; }
        [ForeignKey("TopReleaseId")]
        [InverseProperty("ThemeTopRelease")]
        public NewsRelease TopRelease { get; set; }
        [InverseProperty("Theme")]
        public ICollection<ActivityThemes> ActivityThemes { get; set; }
        [InverseProperty("Theme")]
        public ICollection<NewsReleaseTheme> NewsReleaseTheme { get; set; }
    }
}
