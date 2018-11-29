using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("NROrigin", Schema = "calendar")]
    public partial class Nrorigin
    {
        public Nrorigin()
        {
            ActivityNrorigins = new HashSet<ActivityNrorigins>();
        }

        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int? SortOrder { get; set; }
        [Required]
        public byte[] TimeStamp { get; set; }
        public Guid RowGuid { get; set; }

        [InverseProperty("Nrorigin")]
        public ICollection<ActivityNrorigins> ActivityNrorigins { get; set; }
    }
}
