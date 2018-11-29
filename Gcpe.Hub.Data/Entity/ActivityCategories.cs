using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ActivityCategories", Schema = "calendar")]
    public partial class ActivityCategories
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int CategoryId { get; set; }
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
        [InverseProperty("ActivityCategories")]
        public Activity Activity { get; set; }
        [ForeignKey("CategoryId")]
        [InverseProperty("ActivityCategories")]
        public Category Category { get; set; }
        [ForeignKey("CreatedBy")]
        [InverseProperty("ActivityCategoriesCreatedByNavigation")]
        public SystemUser CreatedByNavigation { get; set; }
        [ForeignKey("LastUpdatedBy")]
        [InverseProperty("ActivityCategoriesLastUpdatedByNavigation")]
        public SystemUser LastUpdatedByNavigation { get; set; }
    }
}
