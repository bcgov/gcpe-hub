using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("SysConfig", Schema = "media")]
    public partial class SysConfig
    {
        [Key]
        [StringLength(250)]
        public string ConfigKey { get; set; }
        [Required]
        [StringLength(250)]
        public string ConfigValue { get; set; }
        public int ConfigDataType { get; set; }
        [StringLength(500)]
        public string ConfigDescription { get; set; }
    }
}
