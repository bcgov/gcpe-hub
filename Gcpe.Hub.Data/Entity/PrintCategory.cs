using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("PrintCategory", Schema = "media")]
    public partial class PrintCategory
    {
        public PrintCategory()
        {
            Company = new HashSet<Company>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(250)]
        public string PrintCategoryName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("PrintCategory")]
        public ICollection<Company> Company { get; set; }
    }
}
