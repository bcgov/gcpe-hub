using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ContactCity", Schema = "media")]
    public partial class ContactCity
    {
        public ContactCity()
        {
            CompanyAddress = new HashSet<CompanyAddress>();
            ContactAddress = new HashSet<ContactAddress>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(250)]
        public string CityName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("City")]
        public ICollection<CompanyAddress> CompanyAddress { get; set; }
        [InverseProperty("City")]
        public ICollection<ContactAddress> ContactAddress { get; set; }
    }
}
