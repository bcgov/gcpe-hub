using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MediaRelationsDatabase.Configuration;

namespace MediaRelationsDatabase
{
    partial class MediaRelationsEntities : DbContext
    {
        private static string _connectionString;

        public static string GetConnectionString()
        {
            if (String.IsNullOrEmpty(_connectionString))
            {
                var connstr = System.Configuration.ConfigurationManager.ConnectionStrings["MediaRelationsEntities"];
                // read the entity connection string as it is stored...
                EntityConnectionStringBuilder entityConnectionStringBuilder = new EntityConnectionStringBuilder(connstr.ConnectionString);
                // set the sql connection string that we will manipulate...
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(entityConnectionStringBuilder.ProviderConnectionString);

                //Console.WriteLine(builder.ConnectionString);

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
                builder["Database"] = App.Settings.DbName;
                builder["Server"] = App.Settings.DbServer;

                //Console.WriteLine(builder.ConnectionString);
                string providerString = builder.ToString();

                EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();

                //Set the provider name from the original connection string
                entityBuilder.Provider = entityConnectionStringBuilder.Provider;

                // Set the Metadata location.
                entityBuilder.Metadata = entityConnectionStringBuilder.Metadata;

                // Set the provider-specific connection string with the override values
                entityBuilder.ProviderConnectionString = providerString;

                //Console.WriteLine(entityBuilder.ToString());
                _connectionString = entityBuilder.ToString();
            }

            return _connectionString;
        }

        // FYI, After re-generating the EF models you will need to delete 
        // the MediaRelations() constructor from "MediaRelations.Context.cs"
        public MediaRelationsEntities() : base(GetConnectionString())
        {
        }
    }
}
