using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateCalendar.Data.Configuration;

namespace CorporateCalendar.Configuration
{
    public interface ICalendarConfiguration : IConnectionConfiguration
    {
        String LdapUrl { get; }
        String ActiveDirectoryDomain { get; }
        String HQAdmin { get; }
        String ApplicationOwnerOrganizations { get; }

        String SMTPServer { get; }
        String LogMailFrom { get; }
        String LogMailTo { get; }

        String DebugLdapUser { get; }
    }
}
