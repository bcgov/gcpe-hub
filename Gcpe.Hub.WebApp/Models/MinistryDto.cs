using System;
using System.Collections.Generic;

namespace Gcpe.Hub.Website.Models
{
    public class MinistryDto
    {
        public Guid Id { get; set; }

        public string DisplayAs { get; set; }

        public IEnumerable<UserDto> Users { get; set; }
        
        public string Abbreviation { get; set; }

        public DateTimeOffset? EodFinalizedDateTime { get; set; }

        public UserDto EodLastRunUser { get; set; }

        public DateTimeOffset? EodLastRunDateTime { get; set; }

        public UserDto PrimaryContact { get; set; }

        public UserDto SecondaryContact { get; set; }

        public string AfterHoursPhone { get; set; }

        public string AfterHoursPhoneExtension { get; set; }
    }
}