using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Keyword", Schema = "calendar")]
    public partial class Keyword
    {
        public Keyword()
        {
            ActivityKeywords = new HashSet<ActivityKeywords>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDateTime { get; set; }
        public int LastUpdatedBy { get; set; }

        [InverseProperty("Keyword")]
        public ICollection<ActivityKeywords> ActivityKeywords { get; set; }
    }
}
