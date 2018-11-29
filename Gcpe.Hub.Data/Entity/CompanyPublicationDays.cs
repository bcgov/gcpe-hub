using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CompanyPublicationDays", Schema = "media")]
    public partial class CompanyPublicationDays
    {
        public Guid CompanyId { get; set; }
        public Guid PublicationDaysId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("CompanyPublicationDays")]
        public Company Company { get; set; }
        [ForeignKey("PublicationDaysId")]
        [InverseProperty("CompanyPublicationDays")]
        public PublicationDays PublicationDays { get; set; }
    }
}
