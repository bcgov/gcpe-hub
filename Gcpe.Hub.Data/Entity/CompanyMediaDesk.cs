using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CompanyMediaDesk", Schema = "media")]
    public partial class CompanyMediaDesk
    {
        public Guid CompanyId { get; set; }
        public Guid MediaDeskId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("CompanyMediaDesk")]
        public Company Company { get; set; }
        [ForeignKey("MediaDeskId")]
        [InverseProperty("CompanyMediaDesk")]
        public MediaDesk MediaDesk { get; set; }
    }
}
