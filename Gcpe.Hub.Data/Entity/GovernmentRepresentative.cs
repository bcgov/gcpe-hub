using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("GovernmentRepresentative", Schema = "calendar")]
    public partial class GovernmentRepresentative
    {
        public GovernmentRepresentative()
        {
            Activity = new HashSet<Activity>();
        }

        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public int? SortOrder { get; set; }
        public byte[] TimeStamp { get; set; }
        public Guid? RowGuid { get; set; }

        [InverseProperty("GovernmentRepresentative")]
        public ICollection<Activity> Activity { get; set; }
    }
}
