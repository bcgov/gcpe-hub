using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("MediaRequestContact", Schema = "media")]
    public partial class MediaRequestContact
    {
        public Guid MediaRequestId { get; set; }
        public Guid ContactId { get; set; }
        public Guid CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("MediaRequestContact")]
        public Company Company { get; set; }
        [ForeignKey("ContactId")]
        [InverseProperty("MediaRequestContact")]
        public Contact Contact { get; set; }
        [ForeignKey("MediaRequestId")]
        [InverseProperty("MediaRequestContact")]
        public MediaRequest MediaRequest { get; set; }
    }
}
