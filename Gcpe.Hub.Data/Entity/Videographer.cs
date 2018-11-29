using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Videographer", Schema = "calendar")]
    public partial class Videographer
    {
        public Videographer()
        {
            Activity = new HashSet<Activity>();
        }

        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(150)]
        public string JobTitle { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }
        public byte[] TimeStamp { get; set; }
        public Guid? RowGuid { get; set; }

        [InverseProperty("Videographer")]
        public ICollection<Activity> Activity { get; set; }
    }
}
