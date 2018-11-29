using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Report", Schema = "media")]
    public partial class Report
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(150)]
        public string ReportName { get; set; }
        [Required]
        [StringLength(150)]
        public string ReportOwner { get; set; }
        [Required]
        public string ReportQueryString { get; set; }
        public bool IsPublic { get; set; }
        public int SortOrder { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
    }
}
