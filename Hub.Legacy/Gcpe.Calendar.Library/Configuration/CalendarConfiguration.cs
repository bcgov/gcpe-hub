using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace CorporateCalendar.Configuration
{
    public static class Extensions
    {

        public static string[] ToStringArray(this string value, char delimiter = ',')
        {
           return value.Split(delimiter);
        }
    }

    public static class App
    {
        readonly static ICalendarConfiguration instance = new CalendarConfiguration(new RES.Configuration.Configuration());
        public static ICalendarConfiguration Settings => instance;
    }

    public class CalendarConfiguration : ICalendarConfiguration
    {
        readonly RES.Configuration.Configuration configuration;

        public CalendarConfiguration(RES.Configuration.Configuration configuration)
        {
            Contract.Requires(configuration != null);
            this.configuration = configuration;

            Validate();
        }

        void Validate()
        {
            using (var validator = configuration.CreateValidator)
            {
                validator.Check(() => LdapUrl);
                validator.Check(() => HQAdmin);
                validator.Check(() => ApplicationOwnerOrganizations);

                validator.Check(() => SMTPServer);
                validator.Check(() => LogMailFrom);
                validator.Check(() => LogMailTo);

                validator.Check(() => DbServer);
                validator.Check(() => DbName);
                validator.Check(() => DbUserID);
                validator.Check(() => DbPassword);
            }
        }

        public String LdapUrl => configuration.GetString(MethodBase.GetCurrentMethod());
        public String HQAdmin => configuration.GetString(MethodBase.GetCurrentMethod());
        public String ApplicationOwnerOrganizations => configuration.GetString(MethodBase.GetCurrentMethod());
        public String SMTPServer => configuration.GetString(MethodBase.GetCurrentMethod());
        public String LogMailFrom => configuration.GetString(MethodBase.GetCurrentMethod());
        public String LogMailTo => configuration.GetString(MethodBase.GetCurrentMethod());

        public string DbServer => configuration.GetString(MethodBase.GetCurrentMethod());
        public string DbName => configuration.GetString(MethodBase.GetCurrentMethod());
        public string DbUserID => configuration.GetString(MethodBase.GetCurrentMethod());
        public string DbPassword => configuration.GetString(MethodBase.GetCurrentMethod());
    }
}
