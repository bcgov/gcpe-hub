using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("MediaType", Schema = "media")]
    public partial class MediaType
    {
        public MediaType()
        {
            CompanyMediaType = new HashSet<CompanyMediaType>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(250)]
        public string MediaTypeName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("MediaType")]
        public ICollection<CompanyMediaType> CompanyMediaType { get; set; }
    }
}
