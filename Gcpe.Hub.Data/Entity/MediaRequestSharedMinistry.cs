using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("MediaRequestSharedMinistry", Schema = "media")]
    public partial class MediaRequestSharedMinistry
    {
        public Guid MediaRequestId { get; set; }
        public Guid MinistryId { get; set; }

        [ForeignKey("MediaRequestId")]
        [InverseProperty("MediaRequestSharedMinistry")]
        public MediaRequest MediaRequest { get; set; }
        [ForeignKey("MinistryId")]
        [InverseProperty("MediaRequestSharedMinistry")]
        public Ministry Ministry { get; set; }
    }
}
