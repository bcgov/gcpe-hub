using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gcpe.Hub.WebApp.Models
{
    public class ReportUpdate
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public Data.Entity.EodReportWith? EodReportWith { get; set; }
    }
}
