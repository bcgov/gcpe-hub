using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class Blob
    {
        public Blob()
        {
            NewsReleaseHistory = new HashSet<NewsReleaseHistory>();
            NewsReleaseImage = new HashSet<NewsReleaseImage>();
        }

        public Guid Id { get; set; }
        [Required]
        public byte[] Data { get; set; }

        [InverseProperty("Blob")]
        public ICollection<NewsReleaseHistory> NewsReleaseHistory { get; set; }
        [InverseProperty("Blob")]
        public ICollection<NewsReleaseImage> NewsReleaseImage { get; set; }
    }
}
