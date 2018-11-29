using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("ContactMediaJobTitle", Schema = "media")]
    public partial class ContactMediaJobTitle
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid MediaJobTitleId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("ContactMediaJobTitle")]
        public Company Company { get; set; }
        [ForeignKey("ContactId")]
        [InverseProperty("ContactMediaJobTitle")]
        public Contact Contact { get; set; }
        [ForeignKey("MediaJobTitleId")]
        [InverseProperty("ContactMediaJobTitle")]
        public MediaJobTitle MediaJobTitle { get; set; }
    }
}
