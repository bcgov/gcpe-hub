using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Priority", Schema = "calendar")]
    public partial class Priority
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public byte[] TimeStamp { get; set; }
        public Guid RowGuid { get; set; }
    }
}
