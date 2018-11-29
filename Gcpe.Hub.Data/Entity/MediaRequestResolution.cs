using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class MediaRequestResolution
    {
        public MediaRequestResolution()
        {
            MediaRequest = new HashSet<MediaRequest>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string DisplayAs { get; set; }

        [InverseProperty("Resolution")]
        public ICollection<MediaRequest> MediaRequest { get; set; }
    }
}
