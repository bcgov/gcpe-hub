using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Category", Schema = "calendar")]
    public partial class Category
    {
        public Category()
        {
            ActivityCategories = new HashSet<ActivityCategories>();
        }

        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public int? SortOrder { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public byte[] TimeStamp { get; set; }
        public Guid RowGuid { get; set; }

        [InverseProperty("Category")]
        public ICollection<ActivityCategories> ActivityCategories { get; set; }
    }
}
