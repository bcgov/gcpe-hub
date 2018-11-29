using System;
using System.Data.SqlClient;
using CorporateCalendar.Data.Configuration;

namespace CorporateCalendar.Data
{
    partial class SystemUser
    {
    }

    partial class CorporateCalendarDataContext
    {
        private static string _connectionString;

        public static string GetConnectionString()
        {
            if (String.IsNullOrEmpty(_connectionString))
            {
                var connstr = System.Configuration.ConfigurationManager.ConnectionStrings["Gcpe_CalendarConnectionString"];
                // read the connection string as it is stored, we will manipulate and override values.
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connstr.ConnectionString);

               Console.WriteLine(builder.ConnectionString);

                builder.Password = App.Settings.DbPassword;
                builder.UserID = App.Settings.DbUserID;
                builder["Database"] = App.Settings.DbName;
                builder["Server"] = App.Settings.DbServer;

                _connectionString = builder.ToString();

                Console.WriteLine(_connectionString);
            }
            return _connectionString;
        }

        public CorporateCalendarDataContext() :
            base(GetConnectionString(), mappingSource)
        {
        }
    }
}
