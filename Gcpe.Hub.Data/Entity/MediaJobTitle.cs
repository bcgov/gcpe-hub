using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("MediaJobTitle", Schema = "media")]
    public partial class MediaJobTitle
    {
        public MediaJobTitle()
        {
            ContactMediaJobTitle = new HashSet<ContactMediaJobTitle>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(250)]
        public string MediaJobTitleName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("MediaJobTitle")]
        public ICollection<ContactMediaJobTitle> ContactMediaJobTitle { get; set; }
    }
}
