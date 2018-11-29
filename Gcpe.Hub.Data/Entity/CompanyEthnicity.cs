using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CompanyEthnicity", Schema = "media")]
    public partial class CompanyEthnicity
    {
        public Guid CompanyId { get; set; }
        public Guid EthnicityId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("CompanyEthnicity")]
        public Company Company { get; set; }
        [ForeignKey("EthnicityId")]
        [InverseProperty("CompanyEthnicity")]
        public Ethnicity Ethnicity { get; set; }
    }
}
