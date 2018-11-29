using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ActivityKeywords", Schema = "calendar")]
    public partial class ActivityKeywords
    {
        public int ActivityId { get; set; }
        public int KeywordId { get; set; }
        public bool IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDateTime { get; set; }
        public int LastUpdatedBy { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("ActivityKeywords")]
        public Activity Activity { get; set; }
        [ForeignKey("KeywordId")]
        [InverseProperty("ActivityKeywords")]
        public Keyword Keyword { get; set; }
    }
}
