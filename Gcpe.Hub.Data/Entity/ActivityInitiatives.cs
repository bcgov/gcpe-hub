using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ActivityInitiatives", Schema = "calendar")]
    public partial class ActivityInitiatives
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int InitiativeId { get; set; }
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
        [InverseProperty("ActivityInitiatives")]
        public Activity Activity { get; set; }
        [ForeignKey("CreatedBy")]
        [InverseProperty("ActivityInitiativesCreatedByNavigation")]
        public SystemUser CreatedByNavigation { get; set; }
        [ForeignKey("InitiativeId")]
        [InverseProperty("ActivityInitiatives")]
        public Initiative Initiative { get; set; }
        [ForeignKey("LastUpdatedBy")]
        [InverseProperty("ActivityInitiativesLastUpdatedByNavigation")]
        public SystemUser LastUpdatedByNavigation { get; set; }
    }
}
