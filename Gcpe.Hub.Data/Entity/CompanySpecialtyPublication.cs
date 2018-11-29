using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CompanySpecialtyPublication", Schema = "media")]
    public partial class CompanySpecialtyPublication
    {
        public Guid CompanyId { get; set; }
        public Guid SpecialtyPublicationId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("CompanySpecialtyPublication")]
        public Company Company { get; set; }
        [ForeignKey("SpecialtyPublicationId")]
        [InverseProperty("CompanySpecialtyPublication")]
        public SpecialtyPublication SpecialtyPublication { get; set; }
    }
}
