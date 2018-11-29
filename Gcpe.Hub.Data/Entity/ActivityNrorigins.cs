using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ActivityNROrigins", Schema = "calendar")]
    public partial class ActivityNrorigins
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        [Column("NROriginId")]
        public int NroriginId { get; set; }
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
        [InverseProperty("ActivityNrorigins")]
        public Activity Activity { get; set; }
        [ForeignKey("CreatedBy")]
        [InverseProperty("ActivityNroriginsCreatedByNavigation")]
        public SystemUser CreatedByNavigation { get; set; }
        [ForeignKey("LastUpdatedBy")]
        [InverseProperty("ActivityNroriginsLastUpdatedByNavigation")]
        public SystemUser LastUpdatedByNavigation { get; set; }
        [ForeignKey("NroriginId")]
        [InverseProperty("ActivityNrorigins")]
        public Nrorigin Nrorigin { get; set; }
    }
}
