using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("SocialMediaPost", Schema = "dashboard")]
    public partial class SocialMediaPost
    {
        public Guid Id { get; set; }
        [Required]
        [Column(TypeName = "text")]
        public string EmbedCode { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Timestamp { get; set; }
    }
}
