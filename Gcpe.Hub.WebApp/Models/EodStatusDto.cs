using Gcpe.Hub.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gcpe.Hub.WebApp.Models
{
    public class EodStatusDto
    {
        public MinistryDto Ministry { get; set; }

        public DateTimeOffset? LastActivity { get; set; }
    }
}
