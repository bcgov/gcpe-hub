using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Web;
using CorporateCalendar.Configuration;

namespace Gcpe.Hub.Configuration
{
    public static class Utils
    {
        public static System.Uri ToUri(this string value)
        {
            return new System.Uri(value);
        }

        public static string[] ToStringArray(this string value, char delimiter = ',')
        {
           return value.Split(delimiter);
        }

        public static ICollection<string> ToCollection(this string value, char delimiter = ',')
        {
            return value.Split(delimiter).ToList();

        }
    }

    public static class App
    {
        readonly static IWebsiteConfiguration instance = new WebsiteConfiguration(new RES.Configuration.Configuration());
        public static IWebsiteConfiguration Settings => instance;

    }
    public class WebsiteConfiguration : IWebsiteConfiguration
    {
        readonly RES.Configuration.Configuration configuration;

        public WebsiteConfiguration(RES.Configuration.Configuration configuration)
        {
            Contract.Requires(configuration != null);
            this.configuration = configuration;

            Validate();
        }

        void Validate()
        {
            using (var validator = configuration.CreateValidator)
            {
                // ICalendarConfiguration
                validator.Check(() => LdapUrl);
                validator.Check(() => ActiveDirectoryDomain);
                validator.Check(() => HQAdmin);
                validator.Check(() => ApplicationOwnerOrganizations);
                validator.Check(() => SMTPServer);
                validator.Check(() => LogMailFrom);
                validator.Check(() => LogMailTo);
                validator.Check(() => DebugLdapUser);

                // IMediaRelationsConfiguration
                validator.Check(() => DoExceptionLogging);
                //validator.Check(() => SMTPServer);  // checked above
                validator.Check(() => AdminGroups);
                validator.Check(() => ContributorGroups);
                validator.Check(() => ActiveDirectoryDomain);
                validator.Check(() => TypedownItemLimit);
                validator.Check(() => PurgeTaskFrequencyHours);
                validator.Check(() => CompanyPurgeDays);
                validator.Check(() => ContactPurgeDays);
                validator.Check(() => LookbackDays);
                validator.Check(() => SubscribeBaseUri);

                // IWebsiteConfiguration (Calendar)
                validator.Check(() => Version);
                validator.Check(() => DisableEditTable);
                validator.Check(() => SharedWithExcludes);
                validator.Check(() => StrategyIsRequired);
                validator.Check(() => SignificanceIsRequired);
                validator.Check(() => SchedulingIsRequired);
                validator.Check(() => ShowHqCommentsField);
                validator.Check(() => ShowSignificanceField);
                validator.Check(() => ShowScheduleField);
                validator.Check(() => HelpFileUri);

                // IWebsiteConfiguration (Contacts/Media Relations)
                validator.Check(() => DisableEmail);
                validator.Check(() => FromEmailAddress);
                validator.Check(() => MaxBccEmails);

                // IWebsiteConfiguration (News)
                //
                validator.Check(() => NewsHostUri);
                validator.Check(() => NewsFileFolder);
                validator.Check(() => MediaAssetsUri);
                validator.Check(() => MediaAssetsUnc);
                validator.Check(() => DeployFolders);
                validator.Check(() => EnableForecastTab);
                validator.Check(() => PublishLocation);
                validator.Check(() => DeployLocations);
                validator.Check(() => CurrentNewsReleaseCollectionId);
                validator.Check(() => NewsReleaseEnglishLocations);

               // Website
                validator.Check(() => TrustedReverseProxyServers);
                validator.Check(() => ContextDomainName);
                validator.Check(() => FlickrApiKey);
                validator.Check(() => FlickrApiSecret);
                validator.Check(() => FlickrApiToken);
                validator.Check(() => FlickrApiTokenSecret);
                validator.Check(() => FlickrApiVerifier);

                // Database Connection
                validator.Check(() => DbServer);
                validator.Check(() => DbName);
                validator.Check(() => DbUserID);
                validator.Check(() => DbPassword);

                validator.Check(() => CloudEndpointsProtocol);
                validator.Check(() => CloudAccountName);
                validator.Check(() => CloudAccountKey);
                validator.Check(() => CloudEndpointSuffix);

                // Customizations - Images, Text, etc.
               /* validator.Check(() => FaviconImg);
                validator.Check(() => ContactsHeaderImg);
                validator.Check(() => CalendarLookAheadCoverImg);
                validator.Check(() => ContactsTitle);
                validator.Check(() => CalendarTitle);
                validator.Check(() => NewsReleaseManagementTitle);
                validator.Check(() => NewsFileManagementTitle);
                validator.Check(() => NewsHelpUrl);

                validator.Check(() => GovShortName);
                validator.Check(() => GovLongName);
                validator.Check(() => ICalProductID);*/
            }
        }
        // Shared across other interfaces
        public String SMTPServer => configuration.GetString(MethodBase.GetCurrentMethod());
        public String ActiveDirectoryDomain => configuration.GetString(MethodBase.GetCurrentMethod());
        public String SubscribeBaseUri => configuration.GetString(MethodBase.GetCurrentMethod());

        // ICalendarConfiguration
        public String LdapUrl => configuration.GetString(MethodBase.GetCurrentMethod());
        public String HQAdmin => configuration.GetString(MethodBase.GetCurrentMethod());
        public String ApplicationOwnerOrganizations => configuration.GetString(MethodBase.GetCurrentMethod());
        public String LogMailFrom => configuration.GetString(MethodBase.GetCurrentMethod());
        public String LogMailTo => configuration.GetString(MethodBase.GetCurrentMethod());
        public String DebugLdapUser => configuration.GetString(MethodBase.GetCurrentMethod());

        // IMediaRelationsConfiguration
        public bool DoExceptionLogging => configuration.GetBool(MethodBase.GetCurrentMethod());
        public String AdminGroups => configuration.GetString(MethodBase.GetCurrentMethod());
        public String ContributorGroups => configuration.GetString(MethodBase.GetCurrentMethod());
        public int TypedownItemLimit => configuration.GetIntWithDefault(MethodBase.GetCurrentMethod(), 20);
        public int PurgeTaskFrequencyHours => configuration.GetIntWithDefault(MethodBase.GetCurrentMethod(), 24);
        public int CompanyPurgeDays => configuration.GetIntWithDefault(MethodBase.GetCurrentMethod(), 90);
        public int ContactPurgeDays => configuration.GetIntWithDefault(MethodBase.GetCurrentMethod(), 90);
        public int LookbackDays => configuration.GetIntWithDefault(MethodBase.GetCurrentMethod(), 10);
 
        // IWebsiteConfiguration (Calendar)
        public String Version => configuration.GetString(MethodBase.GetCurrentMethod());
        public bool DisableEditTable => configuration.GetBool(MethodBase.GetCurrentMethod());
        public String SharedWithExcludes => configuration.GetString(MethodBase.GetCurrentMethod());

        public bool StrategyIsRequired => configuration.GetBool(MethodBase.GetCurrentMethod());
        public bool SignificanceIsRequired => configuration.GetBool(MethodBase.GetCurrentMethod());
        public bool SchedulingIsRequired => configuration.GetBool(MethodBase.GetCurrentMethod());
        public bool ShowHqCommentsField => configuration.GetBool(MethodBase.GetCurrentMethod());
        public bool ShowSignificanceField => configuration.GetBool(MethodBase.GetCurrentMethod());
        public bool ShowScheduleField => configuration.GetBool(MethodBase.GetCurrentMethod());
        public bool ShowRecordsSection => configuration.GetBool(MethodBase.GetCurrentMethod());
        public String HelpFileUri => configuration.GetString(MethodBase.GetCurrentMethod());

        // IWebsiteConfiguration (Contacts/Media Relations)
        public bool DisableEmail => configuration.GetBoolWithDefault(MethodBase.GetCurrentMethod(), false);
        public String FromEmailAddress => configuration.GetString(MethodBase.GetCurrentMethod());
        public int MaxBccEmails => configuration.GetIntWithDefault(MethodBase.GetCurrentMethod(), 50);

        // IWebsiteConfiguration (News)
        public String NewsHostUri => configuration.GetString(MethodBase.GetCurrentMethod());
        public String NewsFileFolder => configuration.GetString(MethodBase.GetCurrentMethod());
        public String MediaAssetsUri => configuration.GetString(MethodBase.GetCurrentMethod());
        public String MediaAssetsUnc => configuration.GetString(MethodBase.GetCurrentMethod());
        public String DeployFolders => configuration.GetString(MethodBase.GetCurrentMethod());
        public bool EnableForecastTab => configuration.GetBool(MethodBase.GetCurrentMethod());
        public String PublishLocation => configuration.GetString(MethodBase.GetCurrentMethod());
        public String DeployLocations => configuration.GetString(MethodBase.GetCurrentMethod());
        public String CurrentNewsReleaseCollectionId => configuration.GetString(MethodBase.GetCurrentMethod());
        public String NewsReleaseEnglishLocations => configuration.GetString(MethodBase.GetCurrentMethod());

        // Website
        public String TrustedReverseProxyServers => configuration.GetString(MethodBase.GetCurrentMethod());
        public String ContextDomainName => configuration.GetString(MethodBase.GetCurrentMethod());

        public String FlickrApiKey => configuration.GetString(MethodBase.GetCurrentMethod());
        public String FlickrApiSecret => configuration.GetString(MethodBase.GetCurrentMethod());
        public String FlickrApiToken => configuration.GetString(MethodBase.GetCurrentMethod());
        public String FlickrApiTokenSecret => configuration.GetString(MethodBase.GetCurrentMethod());
        public String FlickrApiVerifier => configuration.GetString(MethodBase.GetCurrentMethod());

        // Database Connection
        public string DbServer => configuration.GetString(MethodBase.GetCurrentMethod());
        public string DbName => configuration.GetString(MethodBase.GetCurrentMethod());
        public string DbUserID => configuration.GetString(MethodBase.GetCurrentMethod());
        public string DbPassword => configuration.GetString(MethodBase.GetCurrentMethod());

        // Cloud Storage / Azure Configuration
        public string CloudEndpointsProtocol => configuration.GetString(MethodBase.GetCurrentMethod());
        public string CloudAccountName => configuration.GetString(MethodBase.GetCurrentMethod());
        public string CloudAccountKey => configuration.GetString(MethodBase.GetCurrentMethod());
        public string CloudEndpointSuffix => configuration.GetString(MethodBase.GetCurrentMethod());

        public string CloudStorageConnectionString()
        {
            return String.Format("DefaultEndpointsProtocol={0};AccountName={1};AccountKey={2};EndpointSuffix={3}", new string[]{CloudEndpointsProtocol, CloudAccountName, CloudAccountKey, CloudEndpointSuffix});
        }

        public string FaviconImg => configuration.GetString(MethodBase.GetCurrentMethod());
        public string ContactsHeaderImg => configuration.GetString(MethodBase.GetCurrentMethod());
        public string CalendarLookAheadCoverImg => configuration.GetString(MethodBase.GetCurrentMethod());
        public string NewsHelpUrl => configuration.GetString(MethodBase.GetCurrentMethod());
    }
}