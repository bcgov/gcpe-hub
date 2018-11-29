using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Message", Schema = "dashboard")]
    public partial class Message
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsHighlighted { get; set; }
        public bool IsPublished { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Timestamp { get; set; }
    }
}
