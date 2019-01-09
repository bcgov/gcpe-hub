using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gcpe.Hub.News.ReleaseManagement
{
    public partial class ReleaseManagement : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Hub.News.Site site = Master;

            site.MenuText = "News Release Management";
            site.MenuNavigateUrl = "~/News/ReleaseManagement/Drafts";
            site.AddAppItem("BC Gov Corporate Calendar", "~/Calendar");
            site.AddAppItem("BC Gov News", "~/News/FileManagement");

            site.AddNavigationItem("New", "~/News/ReleaseManagement/New");
            if (Gcpe.Hub.Properties.Settings.Default.EnableForecastTab)
            {
                site.AddNavigationItem("Forecast", "~/News/ReleaseManagement/Forecast");
            }
            site.AddNavigationItem("Drafts", "~/News/ReleaseManagement/Drafts");
            site.AddNavigationItem("Scheduled", "~/News/ReleaseManagement/Scheduled");
            site.AddNavigationItem("Published", "~/News/ReleaseManagement/Published");
        }
    }
}