using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CommunicationMaterial", Schema = "calendar")]
    public partial class CommunicationMaterial
    {
        public CommunicationMaterial()
        {
            ActivityCommunicationMaterials = new HashSet<ActivityCommunicationMaterials>();
        }

        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public int? SortOrder { get; set; }
        [Required]
        public byte[] TimeStamp { get; set; }
        public Guid RowGuid { get; set; }

        [InverseProperty("CommunicationMaterial")]
        public ICollection<ActivityCommunicationMaterials> ActivityCommunicationMaterials { get; set; }
    }
}
