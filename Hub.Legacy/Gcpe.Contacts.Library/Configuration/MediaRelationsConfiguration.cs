using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MediaRelationsLibrary.Configuration
{
    public static class Extensions
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
        readonly static IMediaRelationsConfiguration instance = new MediaRelationsConfiguration(new RES.Configuration.Configuration());
        public static IMediaRelationsConfiguration Settings => instance;
    }

    public class MediaRelationsConfiguration : IMediaRelationsConfiguration
    {
        readonly RES.Configuration.Configuration configuration;

        public MediaRelationsConfiguration(RES.Configuration.Configuration configuration)
        {
            Contract.Requires(configuration != null);
            this.configuration = configuration;

            Validate();
        }

        void Validate()
        {
            using (var validator = configuration.CreateValidator)
            {
                validator.Check(() => DoExceptionLogging);
                validator.Check(() => SMTPServer);
                validator.Check(() => AdminGroups);
                validator.Check(() => ContributorGroups);
                validator.Check(() => ActiveDirectoryDomain);

                validator.Check(() => TypedownItemLimit);
                validator.Check(() => PurgeTaskFrequencyHours);
                validator.Check(() => CompanyPurgeDays);
                validator.Check(() => ContactPurgeDays);
                validator.Check(() => LookbackDays);

                validator.Check(() => SubscribeBaseUri);

                validator.Check(() => DbServer);
                validator.Check(() => DbName);
                validator.Check(() => DbUserID);
                validator.Check(() => DbPassword);
            }
        }

        public bool DoExceptionLogging => configuration.GetBool(MethodBase.GetCurrentMethod());
        public String SMTPServer => configuration.GetString(MethodBase.GetCurrentMethod());
        public String AdminGroups => configuration.GetString(MethodBase.GetCurrentMethod());
        public String ContributorGroups => configuration.GetString(MethodBase.GetCurrentMethod());
        public String ActiveDirectoryDomain => configuration.GetString(MethodBase.GetCurrentMethod());

        public int TypedownItemLimit => configuration.GetIntWithDefault(MethodBase.GetCurrentMethod() ,20);
        public int PurgeTaskFrequencyHours => configuration.GetIntWithDefault(MethodBase.GetCurrentMethod(), 24);
        public int CompanyPurgeDays => configuration.GetIntWithDefault(MethodBase.GetCurrentMethod(), 90);
        public int ContactPurgeDays => configuration.GetIntWithDefault(MethodBase.GetCurrentMethod(), 90);
        public int LookbackDays => configuration.GetIntWithDefault(MethodBase.GetCurrentMethod(), 10);

        public String SubscribeBaseUri => configuration.GetString(MethodBase.GetCurrentMethod());

        public string DbServer => configuration.GetString(MethodBase.GetCurrentMethod());
        public string DbName => configuration.GetString(MethodBase.GetCurrentMethod());
        public string DbUserID => configuration.GetString(MethodBase.GetCurrentMethod());
        public string DbPassword => configuration.GetString(MethodBase.GetCurrentMethod());
    }
}
