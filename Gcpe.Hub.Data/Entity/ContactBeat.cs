using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ContactBeat", Schema = "media")]
    public partial class ContactBeat
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid BeatId { get; set; }

        [ForeignKey("BeatId")]
        [InverseProperty("ContactBeat")]
        public Beat Beat { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("ContactBeat")]
        public Company Company { get; set; }
        [ForeignKey("ContactId")]
        [InverseProperty("ContactBeat")]
        public Contact Contact { get; set; }
    }
}
