using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class CarouselSlide
    {
        public Guid CarouselId { get; set; }
        public Guid SlideId { get; set; }
        public int SortIndex { get; set; }

        [ForeignKey("CarouselId")]
        [InverseProperty("CarouselSlide")]
        public Carousel Carousel { get; set; }
        [ForeignKey("SlideId")]
        [InverseProperty("CarouselSlide")]
        public Slide Slide { get; set; }
    }
}
