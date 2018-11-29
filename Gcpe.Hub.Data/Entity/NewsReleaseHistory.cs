using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class NewsReleaseHistory
    {
        public Guid ReleaseId { get; set; }
        public DateTimeOffset PublishDateTime { get; set; }
        [StringLength(100)]
        public string MimeType { get; set; }
        public Guid BlobId { get; set; }

        [ForeignKey("BlobId")]
        [InverseProperty("NewsReleaseHistory")]
        public Blob Blob { get; set; }
        [ForeignKey("ReleaseId")]
        [InverseProperty("NewsReleaseHistory")]
        public NewsRelease Release { get; set; }
    }
}
