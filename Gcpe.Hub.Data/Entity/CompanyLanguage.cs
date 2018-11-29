using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CompanyLanguage", Schema = "media")]
    public partial class CompanyLanguage
    {
        public Guid CompanyId { get; set; }
        public Guid LanguageId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("CompanyLanguage")]
        public Company Company { get; set; }
        [ForeignKey("LanguageId")]
        [InverseProperty("CompanyLanguage")]
        public MediaLanguage Language { get; set; }
    }
}
