using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class MinistryService
    {
        public Guid MinistryId { get; set; }
        public int SortIndex { get; set; }
        [Required]
        public string LinkText { get; set; }
        [Required]
        [StringLength(255)]
        public string LinkUrl { get; set; }

        [ForeignKey("MinistryId")]
        [InverseProperty("MinistryService")]
        public Ministry Ministry { get; set; }
    }
}
