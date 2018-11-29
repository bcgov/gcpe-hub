using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class Carousel
    {
        public Carousel()
        {
            CarouselSlide = new HashSet<CarouselSlide>();
        }

        public Guid Id { get; set; }
        public DateTimeOffset? PublishDateTime { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        [InverseProperty("Carousel")]
        public ICollection<CarouselSlide> CarouselSlide { get; set; }
    }
}
