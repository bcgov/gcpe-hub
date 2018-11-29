using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("PremierRequested", Schema = "calendar")]
    public partial class PremierRequested
    {
        public PremierRequested()
        {
            Activity = new HashSet<Activity>();
        }

        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int? SortOrder { get; set; }
        [Required]
        public byte[] TimeStamp { get; set; }
        public Guid RowGuid { get; set; }

        [InverseProperty("PremierRequested")]
        public ICollection<Activity> Activity { get; set; }
    }
}
