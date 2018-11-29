using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("City", Schema = "calendar")]
    public partial class City
    {
        public City()
        {
            Activity = new HashSet<Activity>();
        }

        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public byte[] TimeStamp { get; set; }
        public Guid? RowGuid { get; set; }
        public int? SortOrder { get; set; }

        [InverseProperty("City")]
        public ICollection<Activity> Activity { get; set; }
    }
}
