using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("PublicationFrequency", Schema = "media")]
    public partial class PublicationFrequency
    {
        public PublicationFrequency()
        {
            Company = new HashSet<Company>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(250)]
        public string PublicationFrequencyName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("PublicationFrequency")]
        public ICollection<Company> Company { get; set; }
    }
}
