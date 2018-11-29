using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ActivityFiles", Schema = "calendar")]
    public partial class ActivityFiles
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        [Required]
        public byte[] Data { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public string FileType { get; set; }
        public int FileLength { get; set; }
        [Required]
        [StringLength(32)]
        public string Md5 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDateTime { get; set; }
        public int? LastUpdatedBy { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("ActivityFiles")]
        public Activity Activity { get; set; }
    }
}
