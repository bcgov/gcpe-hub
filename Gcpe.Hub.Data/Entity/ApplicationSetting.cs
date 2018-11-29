using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class ApplicationSetting
    {
        [Key]
        [StringLength(100)]
        public string SettingName { get; set; }
        [Required]
        public string SettingValue { get; set; }
    }
}
