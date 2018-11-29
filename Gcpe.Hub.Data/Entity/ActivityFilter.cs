using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ActivityFilter", Schema = "calendar")]
    public partial class ActivityFilter
    {
        public int Id { get; set; }
        [StringLength(300)]
        public string QueryString { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateTime { get; set; }
        public int? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDateTime { get; set; }
        public int? LastUpdatedBy { get; set; }
        public byte[] TimeStamp { get; set; }
        public Guid? RowGuid { get; set; }

        [ForeignKey("CreatedBy")]
        [InverseProperty("ActivityFilterCreatedByNavigation")]
        public SystemUser CreatedByNavigation { get; set; }
        [ForeignKey("LastUpdatedBy")]
        [InverseProperty("ActivityFilterLastUpdatedByNavigation")]
        public SystemUser LastUpdatedByNavigation { get; set; }
    }
}
