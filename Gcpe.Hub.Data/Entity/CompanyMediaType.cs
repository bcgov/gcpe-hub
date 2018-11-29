using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CompanyMediaType", Schema = "media")]
    public partial class CompanyMediaType
    {
        public Guid CompanyId { get; set; }
        public Guid MediaTypeId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("CompanyMediaType")]
        public Company Company { get; set; }
        [ForeignKey("MediaTypeId")]
        [InverseProperty("CompanyMediaType")]
        public MediaType MediaType { get; set; }
    }
}
