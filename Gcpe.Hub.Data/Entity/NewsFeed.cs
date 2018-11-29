using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("NewsFeed", Schema = "calendar")]
    public partial class NewsFeed
    {
        public int Id { get; set; }
        public int? ActivityId { get; set; }
        public Guid MinistryId { get; set; }
        [StringLength(1000)]
        public string Text { get; set; }
        [StringLength(50)]
        public string Description { get; set; }
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

        [ForeignKey("ActivityId")]
        [InverseProperty("NewsFeed")]
        public Activity Activity { get; set; }
        [ForeignKey("CreatedBy")]
        [InverseProperty("NewsFeedCreatedByNavigation")]
        public SystemUser CreatedByNavigation { get; set; }
        [ForeignKey("LastUpdatedBy")]
        [InverseProperty("NewsFeedLastUpdatedByNavigation")]
        public SystemUser LastUpdatedByNavigation { get; set; }
        [ForeignKey("MinistryId")]
        [InverseProperty("NewsFeed")]
        public Ministry Ministry { get; set; }
    }
}
