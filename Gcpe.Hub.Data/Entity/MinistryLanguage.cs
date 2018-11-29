using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class MinistryLanguage
    {
        public Guid MinistryId { get; set; }
        public int LanguageId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [ForeignKey("LanguageId")]
        [InverseProperty("MinistryLanguage")]
        public Language Language { get; set; }
        [ForeignKey("MinistryId")]
        [InverseProperty("MinistryLanguage")]
        public Ministry Ministry { get; set; }
    }
}
