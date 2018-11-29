using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("FavoriteActivity", Schema = "calendar")]
    public partial class FavoriteActivity
    {
        public int SystemUserId { get; set; }
        public int ActivityId { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("FavoriteActivity")]
        public Activity Activity { get; set; }
        [ForeignKey("SystemUserId")]
        [InverseProperty("FavoriteActivity")]
        public SystemUser SystemUser { get; set; }
    }
}
