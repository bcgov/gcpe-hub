using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class Slide
    {
        public Slide()
        {
            CarouselSlide = new HashSet<CarouselSlide>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Headline { get; set; }
        [Required]
        public string Summary { get; set; }
        [Required]
        [StringLength(255)]
        public string ActionUrl { get; set; }
        [Required]
        [Column(TypeName = "image")]
        public byte[] Image { get; set; }
        [StringLength(255)]
        public string FacebookPostUrl { get; set; }
        public Justify Justify { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        [InverseProperty("Slide")]
        public ICollection<CarouselSlide> CarouselSlide { get; set; }
    }
}
