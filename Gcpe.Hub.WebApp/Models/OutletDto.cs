using System;

namespace Gcpe.Hub.Website.Models
{
    public class OutletDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsMajor { get; set; }
    }

    public class MediaJobDto
    {
        public Guid Id { get; set; }
        public OutletDto Outlet { get; set; }
        public string Title { get; set; }
    }
}