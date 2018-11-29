using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class MinistryNewsletter
    {
        public Guid MinistryId { get; set; }
        public int NewsletterId { get; set; }

        [ForeignKey("MinistryId")]
        [InverseProperty("MinistryNewsletter")]
        public Ministry Ministry { get; set; }
    }
}
