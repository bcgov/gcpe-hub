using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Status", Schema = "calendar")]
    public partial class Status
    {
        public Status()
        {
            ActivityHqStatus = new HashSet<Activity>();
            ActivityStatus = new HashSet<Activity>();
        }

        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public byte[] TimeStamp { get; set; }
        public Guid RowGuid { get; set; }

        [InverseProperty("HqStatus")]
        public ICollection<Activity> ActivityHqStatus { get; set; }
        [InverseProperty("Status")]
        public ICollection<Activity> ActivityStatus { get; set; }
    }
}
