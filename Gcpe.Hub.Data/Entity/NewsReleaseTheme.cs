using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseTheme
    {
        public Guid ReleaseId { get; set; }
        public Guid ThemeId { get; set; }

        [ForeignKey("ReleaseId")]
        [InverseProperty("NewsReleaseTheme")]
        public NewsRelease Release { get; set; }
        [ForeignKey("ThemeId")]
        [InverseProperty("NewsReleaseTheme")]
        public Theme Theme { get; set; }
    }
}
