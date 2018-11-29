using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Ethnicity", Schema = "media")]
    public partial class Ethnicity
    {
        public Ethnicity()
        {
            CompanyEthnicity = new HashSet<CompanyEthnicity>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(250)]
        public string EthnicityName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("Ethnicity")]
        public ICollection<CompanyEthnicity> CompanyEthnicity { get; set; }
    }
}
