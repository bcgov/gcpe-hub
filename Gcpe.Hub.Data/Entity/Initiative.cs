using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Initiative", Schema = "calendar")]
    public partial class Initiative
    {
        public Initiative()
        {
            ActivityInitiatives = new HashSet<ActivityInitiatives>();
        }

        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int? SortOrder { get; set; }
        [Required]
        public byte[] TimeStamp { get; set; }
        public Guid RowGuid { get; set; }
        [StringLength(30)]
        public string ShortName { get; set; }

        [InverseProperty("Initiative")]
        public ICollection<ActivityInitiatives> ActivityInitiatives { get; set; }
    }
}
