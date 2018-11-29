using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("MediaLanguage", Schema = "media")]
    public partial class MediaLanguage
    {
        public MediaLanguage()
        {
            CompanyLanguage = new HashSet<CompanyLanguage>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(150)]
        public string LanguageName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("Language")]
        public ICollection<CompanyLanguage> CompanyLanguage { get; set; }
    }
}
