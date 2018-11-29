using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("SpecialtyPublication", Schema = "media")]
    public partial class SpecialtyPublication
    {
        public SpecialtyPublication()
        {
            CompanySpecialtyPublication = new HashSet<CompanySpecialtyPublication>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(250)]
        public string SpecialtyPublicationName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("SpecialtyPublication")]
        public ICollection<CompanySpecialtyPublication> CompanySpecialtyPublication { get; set; }
    }
}
