using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Beat", Schema = "media")]
    public partial class Beat
    {
        public Beat()
        {
            ContactBeat = new HashSet<ContactBeat>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(255)]
        public string BeatName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("Beat")]
        public ICollection<ContactBeat> ContactBeat { get; set; }
    }
}
