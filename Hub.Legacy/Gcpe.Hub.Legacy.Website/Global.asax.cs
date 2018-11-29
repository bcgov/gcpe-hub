#define USE_AZURE
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.Web.DynamicData;
using System.Web.Hosting;
using System.Web.Routing;
using Gcpe.Hub.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Gcpe.Hub
{
    public class Global : System.Web.HttpApplication
    {
        private static MetaModel s_defaultModel = new MetaModel();
        public static MetaModel DefaultModel
        {
            get
            {
                return s_defaultModel;
            }
        }

#if DEBUG
        public static bool IsDebugBuild = true;
#else
        public static bool IsDebugBuild = false;
#endif
        protected void Application_Start(object sender, EventArgs e)
        {
            // IMPORTANT: DATA MODEL REGISTRATION 
            // Uncomment this line to register a LINQ to SQL model for ASP.NET Dynamic Data.
            // Set ScaffoldAllTables = true only if you are sure that you want all tables in the
            // data model to support a scaffold (i.e. templates) view. To control scaffolding for
            // individual tables, create a partial class for the table and apply the
            // [ScaffoldTable(true)] attribute to the partial class.
            // Note: Make sure that you change "YourDataContextType" to the name of the data context
            // class in your application.
            DefaultModel.RegisterContext(typeof(CorporateCalendar.Data.CorporateCalendarDataContext), new ContextConfiguration() { ScaffoldAllTables = true });
            //Additional information: The table 'Activities' does not have a column named 'CommunicationContact'.  

            DefaultModel.DynamicDataFolderVirtualPath = "~/Calendar/Admin/DynamicData";

            // The following statements support combined-page mode, where the List, Detail, Insert, and
            // Update tasks are performed by using the same page. To enable this mode, uncomment the
            // following routes and comment out the route definition in the separate-page mode section above.
            RouteTable.Routes.Add(new DynamicDataRoute("Calendar/Admin/{table}/ListDetails.aspx")
            {
                Action = PageAction.List,
                ViewName = "ListDetails",
                Model = DefaultModel
            });

            RouteTable.Routes.Add(new DynamicDataRoute("Calendar/Admin/{table}/ListDetails.aspx")
            {
                Action = PageAction.Details,
                ViewName = "ListDetails",
                Model = DefaultModel
            });

            News.RouteConfig.RegisterRoutes(RouteTable.Routes); // Must be after the DynamicDataRoutes

            HostingEnvironment.QueueBackgroundWorkItem(applicationShutdownToken =>  ReleasePublisher.NewsReleasePublishTask(applicationShutdownToken));
        }

        public static Uri ReadContainerWithSharedAccessSignature(string containerName, DateTimeOffset expiryTime)
        {
            return ContainerWithSharedAccessSignature(containerName, SharedAccessBlobPermissions.Read, expiryTime);
        }

        public static Uri ListContainerWithSharedAccessSignature(string containerName, DateTimeOffset expiryTime)
        {
            return ContainerWithSharedAccessSignature(containerName, SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List, expiryTime);
        }

        public static Uri ModifyContainerWithSharedAccessSignature(string containerName, DateTimeOffset? expiryTime = null)
        {
            return ContainerWithSharedAccessSignature(containerName, SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Delete);
        }

        public static Uri ContainerWithSharedAccessSignature(string containerName, SharedAccessBlobPermissions permissions, DateTimeOffset? expiryTime = null)
        {
            //TODO: This method should be moved out-of-process (for instance, to the Gcpe.Hub.Services project).
 
            var connectionString = App.Settings.CloudStorageConnectionString();

            var account = CloudStorageAccount.Parse(connectionString);

            var client = account.CreateCloudBlobClient();

            var container = client.GetContainerReference(containerName);

            SharedAccessBlobPolicy sharedPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = expiryTime ?? DateTimeOffset.UtcNow.AddHours(1),
                Permissions = permissions
            };

            return new Uri(container.Uri + container.GetSharedAccessSignature(sharedPolicy));
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// This is called just before the user credentials are authenticated.
        /// We can specify our own authentication logic here to provide custom authentication.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            Authentication.GlobalAuthenticateRequest(Request, Response);
        }

        /// <summary>
        /// This is called after successful completion of authentication with user’s credentials.
        /// This event is used to determine user permissions.
        /// You can use this method to give authorization rights to user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_AuthorizeRequest(object sender, EventArgs e)
        {
            Authentication.GlobalAuthorizeRequest(Request, Response);
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            //Authentication.GlobalAcquireRequestState(((HttpApplication)sender).Context, Request);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }

#if !USE_AZURE
        public static void QueueBackgroundWorkItemWithRetry(Action action)
        {
            System.Threading.Tasks.Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        action();
                        break;
                    }
                    catch (System.Exception ex)
                    {
                        try
                        {
                            LogError(ex.ToString());
                        }
                        catch{}
                    }

                    await System.Threading.Tasks.Task.Delay(60 * 1000);
                }
            });
        }
#endif
    }
}