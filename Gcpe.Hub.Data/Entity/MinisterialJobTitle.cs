using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("MinisterialJobTitle", Schema = "media")]
    public partial class MinisterialJobTitle
    {
        public MinisterialJobTitle()
        {
            Contact = new HashSet<Contact>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(250)]
        public string MinisterialJobTitleName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("MinisterialJobTitle")]
        public ICollection<Contact> Contact { get; set; }
    }
}
