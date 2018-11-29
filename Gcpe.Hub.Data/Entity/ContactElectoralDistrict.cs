using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ContactElectoralDistrict", Schema = "media")]
    public partial class ContactElectoralDistrict
    {
        public Guid ContactId { get; set; }
        public Guid DistrictId { get; set; }

        [ForeignKey("ContactId")]
        [InverseProperty("ContactElectoralDistrict")]
        public Contact Contact { get; set; }
        [ForeignKey("DistrictId")]
        [InverseProperty("ContactElectoralDistrict")]
        public ElectoralDistrict District { get; set; }
    }
}
