using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ActivityCommunicationMaterials", Schema = "calendar")]
    public partial class ActivityCommunicationMaterials
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int CommunicationMaterialId { get; set; }
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
        [InverseProperty("ActivityCommunicationMaterials")]
        public Activity Activity { get; set; }
        [ForeignKey("CommunicationMaterialId")]
        [InverseProperty("ActivityCommunicationMaterials")]
        public CommunicationMaterial CommunicationMaterial { get; set; }
        [ForeignKey("CreatedBy")]
        [InverseProperty("ActivityCommunicationMaterialsCreatedByNavigation")]
        public SystemUser CreatedByNavigation { get; set; }
        [ForeignKey("LastUpdatedBy")]
        [InverseProperty("ActivityCommunicationMaterialsLastUpdatedByNavigation")]
        public SystemUser LastUpdatedByNavigation { get; set; }
    }
}
