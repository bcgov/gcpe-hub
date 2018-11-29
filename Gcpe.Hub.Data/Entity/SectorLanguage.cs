using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class SectorLanguage
    {
        public Guid SectorId { get; set; }
        public int LanguageId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [ForeignKey("LanguageId")]
        [InverseProperty("SectorLanguage")]
        public Language Language { get; set; }
        [ForeignKey("SectorId")]
        [InverseProperty("SectorLanguage")]
        public Sector Sector { get; set; }
    }
}
