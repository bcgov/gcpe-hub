using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateCalendar.Configuration;
using MediaRelationsLibrary.Configuration;

namespace Gcpe.Hub.Configuration
{
    public interface IWebsiteConfiguration : ICalendarConfiguration, IMediaRelationsConfiguration
    {
        new String SMTPServer { get; }
        new String SubscribeBaseUri { get; }
        new String ActiveDirectoryDomain { get; }

        // Calendar
        String Version { get; }
        bool DisableEditTable { get; }
        String SharedWithExcludes { get; }
        bool StrategyIsRequired { get; }
        bool SignificanceIsRequired { get; }
        bool SchedulingIsRequired { get; }
        bool ShowHqCommentsField { get; }
        bool ShowSignificanceField { get; }
        bool ShowScheduleField { get; }
        bool ShowRecordsSection { get; }
        String HelpFileUri { get; }

        // Media Relations
        bool DisableEmail { get; }
        String FromEmailAddress { get; }
        int MaxBccEmails { get; }

        //News
        String NewsHostUri { get; }
        String NewsFileFolder { get; }
        String MediaAssetsUri { get; }
        String MediaAssetsUnc { get; }
        String DeployFolders { get;  }
        bool EnableForecastTab { get; }
        String PublishLocation { get; }
        String DeployLocations { get; }
        String CurrentNewsReleaseCollectionId { get; }
        String NewsReleaseEnglishLocations { get; }

        // Website
        String TrustedReverseProxyServers { get; }

        String FlickrApiKey { get; }
        String FlickrApiSecret { get; }
        String FlickrApiToken { get; }
        String FlickrApiTokenSecret { get; }
        String FlickrApiVerifier { get; }

        // Database Connection
        new String DbServer { get; }
        new String DbName { get; }
        new String DbUserID { get; }
        new String DbPassword { get; }

        // Cloud Storage / Azure Configuration
        String CloudEndpointsProtocol { get; }
        String CloudAccountName { get; }
        String CloudAccountKey { get; }
        String CloudEndpointSuffix { get; }

        String CloudStorageConnectionString();


        // Customizations - Images, Text, etc.
        String FaviconImg { get; }
        String ContactsHeaderImg { get; }
        String CalendarLookAheadCoverImg { get; }
        String NewsHelpUrl { get; }

    }
}
