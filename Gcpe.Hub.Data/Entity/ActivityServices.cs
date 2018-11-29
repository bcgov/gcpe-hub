using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ActivityServices", Schema = "calendar")]
    public partial class ActivityServices
    {
        public int ActivityId { get; set; }
        public Guid ServiceId { get; set; }
        public bool IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDateTime { get; set; }
        public int CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDateTime { get; set; }
        public int LastUpdatedBy { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("ActivityServices")]
        public Activity Activity { get; set; }
        [ForeignKey("ServiceId")]
        [InverseProperty("ActivityServices")]
        public Service Service { get; set; }
    }
}
