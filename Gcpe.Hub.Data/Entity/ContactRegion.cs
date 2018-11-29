using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ContactRegion", Schema = "media")]
    public partial class ContactRegion
    {
        public Guid ContactId { get; set; }
        public Guid RegionId { get; set; }

        [ForeignKey("ContactId")]
        [InverseProperty("ContactRegion")]
        public Contact Contact { get; set; }
        [ForeignKey("RegionId")]
        [InverseProperty("ContactRegion")]
        public NewsRegion Region { get; set; }
    }
}
