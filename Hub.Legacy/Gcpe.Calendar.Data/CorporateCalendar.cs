using System;
using System.Data.SqlClient;

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

                string GcpeHubDbUserID = Environment.GetEnvironmentVariable("GcpeHubDbUserID");
                string GcpeHubDbPassword = Environment.GetEnvironmentVariable("GcpeHubDbPassword");
                if (!string.IsNullOrEmpty(GcpeHubDbUserID) && !string.IsNullOrEmpty(GcpeHubDbPassword))
                {
                    // override the connection string.
                    // turn off integrated security and add these credentials
                    builder.Password = GcpeHubDbPassword;
                    builder.UserID = GcpeHubDbUserID;
                    builder.IntegratedSecurity = false;
                }

                builder["Server"] = Environment.GetEnvironmentVariable("DbServer");

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
