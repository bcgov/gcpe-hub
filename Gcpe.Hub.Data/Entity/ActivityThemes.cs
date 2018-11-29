using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ActivityThemes", Schema = "calendar")]
    public partial class ActivityThemes
    {
        public int ActivityId { get; set; }
        public Guid ThemeId { get; set; }
        public bool IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDateTime { get; set; }
        public int LastUpdatedBy { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("ActivityThemes")]
        public Activity Activity { get; set; }
        [ForeignKey("ThemeId")]
        [InverseProperty("ActivityThemes")]
        public Theme Theme { get; set; }
    }
}
