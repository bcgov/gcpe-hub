using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateCalendar
{
    public class ActivityModel
    {
        public ICollection<Release> Releases { get; } = new List<Release>();

        public class Release
        {
            public Guid Id { get; set; }
            public string Color { get; set; }
            public string DocumentType { get; set; }
            public string PublishStatus { get; set; }
        }
    }
}
