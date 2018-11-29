using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Log", Schema = "calendar")]
    public partial class Log
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int LogType { get; set; }
        [StringLength(50)]
        public string TableName { get; set; }
        [StringLength(1000)]
        public string FieldName { get; set; }
        [StringLength(1000)]
        public string OldValue { get; set; }
        [StringLength(1000)]
        public string NewValue { get; set; }
        [Required]
        [StringLength(50)]
        public string Operation { get; set; }
        public bool IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateTime { get; set; }
        public int? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDateTime { get; set; }
        public int? LastUpdatedBy { get; set; }
        [Required]
        public byte[] TimeStamp { get; set; }
        public Guid RowGuid { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("Log")]
        public Activity Activity { get; set; }
        [ForeignKey("CreatedBy")]
        [InverseProperty("LogCreatedByNavigation")]
        public SystemUser CreatedByNavigation { get; set; }
        [ForeignKey("LastUpdatedBy")]
        [InverseProperty("LogLastUpdatedByNavigation")]
        public SystemUser LastUpdatedByNavigation { get; set; }
    }
}
