using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("PublicationDays", Schema = "media")]
    public partial class PublicationDays
    {
        public PublicationDays()
        {
            CompanyPublicationDays = new HashSet<CompanyPublicationDays>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(250)]
        public string PublicationDaysName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("PublicationDays")]
        public ICollection<CompanyPublicationDays> CompanyPublicationDays { get; set; }
    }
}
