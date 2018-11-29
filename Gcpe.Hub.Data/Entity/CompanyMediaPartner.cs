using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CompanyMediaPartner", Schema = "media")]
    public partial class CompanyMediaPartner
    {
        public Guid CompanyId { get; set; }
        public Guid MediaPartnerId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("CompanyMediaPartnerCompany")]
        public Company Company { get; set; }
        [ForeignKey("MediaPartnerId")]
        [InverseProperty("CompanyMediaPartnerMediaPartner")]
        public Company MediaPartner { get; set; }
    }
}
