using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("MediaDesk", Schema = "media")]
    public partial class MediaDesk
    {
        public MediaDesk()
        {
            CompanyMediaDesk = new HashSet<CompanyMediaDesk>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(250)]
        public string MediaDeskName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("MediaDesk")]
        public ICollection<CompanyMediaDesk> CompanyMediaDesk { get; set; }
    }
}
