using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("SysLog", Schema = "media")]
    public partial class SysLog
    {
        [Key]
        [Column("LogID")]
        public int LogId { get; set; }
        [Required]
        [StringLength(50)]
        public string Action { get; set; }
        [Required]
        [StringLength(50)]
        public string EntityType { get; set; }
        public Guid? EntityId { get; set; }
        [Required]
        [StringLength(150)]
        public string EntityData { get; set; }
        public Guid? EventId { get; set; }
        public string EventData { get; set; }
        [Required]
        [StringLength(150)]
        public string EventUser { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime EventDate { get; set; }
    }
}
