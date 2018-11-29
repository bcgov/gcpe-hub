using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaRelationsDatabase.Configuration;

namespace MediaRelationsLibrary.Configuration
{
    public interface IMediaRelationsConfiguration: IConnectionConfiguration
    {
        bool DoExceptionLogging { get; }
        String SMTPServer { get; }
        String AdminGroups { get; }
        String ContributorGroups { get; }
        String ActiveDirectoryDomain { get; }
        int TypedownItemLimit { get; }
        int PurgeTaskFrequencyHours { get; }
        int CompanyPurgeDays { get; }
        int ContactPurgeDays { get; }
        int LookbackDays { get; }

        String SubscribeBaseUri { get; }
    }
}
