using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("SystemUserMinistry", Schema = "calendar")]
    public partial class SystemUserMinistry
    {
        public int Id { get; set; }
        public int? SystemUserId { get; set; }
        public Guid? MinistryId { get; set; }
        public bool IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateTime { get; set; }
        public int? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDateTime { get; set; }
        public int? LastUpdatedBy { get; set; }
        [Required]
        public byte[] TimeStamp { get; set; }
        public Guid RowGuid { get; set; }

        [ForeignKey("CreatedBy")]
        [InverseProperty("SystemUserMinistryCreatedByNavigation")]
        public SystemUser CreatedByNavigation { get; set; }
        [ForeignKey("LastUpdatedBy")]
        [InverseProperty("SystemUserMinistryLastUpdatedByNavigation")]
        public SystemUser LastUpdatedByNavigation { get; set; }
        [ForeignKey("MinistryId")]
        [InverseProperty("SystemUserMinistry")]
        public Ministry Ministry { get; set; }
        [ForeignKey("SystemUserId")]
        [InverseProperty("SystemUserMinistrySystemUser")]
        public SystemUser SystemUser { get; set; }
    }
}
