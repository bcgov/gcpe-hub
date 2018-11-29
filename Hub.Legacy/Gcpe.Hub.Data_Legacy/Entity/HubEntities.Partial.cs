using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Gcpe.Hub.Data.Configuration;

namespace Gcpe.Hub.Data.Entity
{
    partial class HubEntities : IHubDbContext
    {
 
        private static string _connectionString;

        public static string GetConnectionString()
        {
            if (String.IsNullOrEmpty(_connectionString))
            {
                var connstr = System.Configuration.ConfigurationManager.ConnectionStrings["HubEntities"];
                // read the entity connection string as it is stored...
                EntityConnectionStringBuilder entityConnectionStringBuilder = new EntityConnectionStringBuilder(connstr.ConnectionString);
                // set the sql connection string that we will manipulate...
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(entityConnectionStringBuilder.ProviderConnectionString);

                //Console.WriteLine(builder.ConnectionString);

                builder.Password = App.Settings.DbPassword;
                builder.UserID = App.Settings.DbUserID;
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
        // the HubEntities() constructor from "HubEntities.Context.cs"
        public HubEntities() : base(GetConnectionString())
        {
        }

        public ApplicationSetting AppSetting(string appSetting)
        {
            return ApplicationSettings.SingleOrDefault(a => a.SettingName == appSetting);
        }

        public string GetAppSetting(string appSetting)
        {
            ApplicationSetting setting = ApplicationSettings.SingleOrDefault(a => a.SettingName == appSetting);

            return setting == null ? null : setting.SettingValue ?? "";
        }

        public ApplicationSetting SetAppSetting(string appSetting, string appValue)
        {
            ApplicationSetting setting = AppSetting(appSetting);

            if (setting == null)
            {
                setting = new ApplicationSetting { SettingName = appSetting };
                ApplicationSettings.Add(setting);
            }

            setting.SettingValue = appValue;

            return setting;
        }

        public async Task<NewsRelease> GetTopReleaseAsync()
        {
            ApplicationSetting setting = AppSetting("HomeTopReleaseId");

            if (setting == null || string.IsNullOrEmpty(setting.SettingValue))
                return null;

            return await NewsReleases.FindAsync(Guid.Parse(setting.SettingValue));
        }

        public async Task<NewsRelease> GetFeatureReleaseAsync()
        {
            ApplicationSetting setting = AppSetting("HomeFeatureReleaseId");

            if (setting == null || string.IsNullOrEmpty(setting.SettingValue))
                return null;

            return await NewsReleases.FindAsync(Guid.Parse(setting.SettingValue));
        }
    }

    public partial class Language
    {
        //en-CA language code
        public const int enCA = 4105;
        //fr-CA language code
        public const int frCA = 3084;
    }

    public interface ICategory
    {
        string DisplayName { get; set; }
        System.DateTime Timestamp { get; set; }
        string MiscHtml { get; set; }
        string MiscRightHtml { get; set; }
        string TwitterUsername { get; set; }
        string FlickrUrl { get; set; }
        string YoutubeUrl { get; set; }
        string AudioUrl { get; set; }
        string FacebookEmbedHtml { get; set; }
        string YoutubeEmbedHtml { get; set; }
        string AudioEmbedHtml { get; set; }
        Nullable<System.Guid> TopReleaseId { get; set; }
        Nullable<System.Guid> FeatureReleaseId { get; set; }
        bool IsActive { get; set; }
    }

    public partial class Ministry : ICategory
    {
    }

    public partial class Sector : ICategory
    {
    }
}
