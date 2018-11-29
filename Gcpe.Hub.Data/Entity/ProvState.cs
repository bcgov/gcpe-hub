using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ProvState", Schema = "media")]
    public partial class ProvState
    {
        public ProvState()
        {
            CompanyAddress = new HashSet<CompanyAddress>();
            ContactAddress = new HashSet<ContactAddress>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(150)]
        public string ProvStateName { get; set; }
        [Required]
        [StringLength(15)]
        public string ProvStateAbbrev { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("ProvState")]
        public ICollection<CompanyAddress> CompanyAddress { get; set; }
        [InverseProperty("ProvState")]
        public ICollection<ContactAddress> ContactAddress { get; set; }
    }
}
