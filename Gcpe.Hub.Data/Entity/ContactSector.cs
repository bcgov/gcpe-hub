using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ContactSector", Schema = "media")]
    public partial class ContactSector
    {
        public Guid ContactId { get; set; }
        public Guid SectorId { get; set; }

        [ForeignKey("ContactId")]
        [InverseProperty("ContactSector")]
        public Contact Contact { get; set; }
        [ForeignKey("SectorId")]
        [InverseProperty("ContactSector")]
        public Sector Sector { get; set; }
    }
}
