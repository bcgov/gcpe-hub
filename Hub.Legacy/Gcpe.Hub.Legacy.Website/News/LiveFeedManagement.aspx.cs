extern alias legacy;

using System;
using legacy::Gcpe.Hub.Data.Entity;

namespace Gcpe.Hub.News
{
    public partial class LiveFeedManagement : Hub.News.Page
    {
        string appSetting = "live_webcast_enabled";

        protected void Page_Load(object sender, EventArgs e)
        {
            Hub.News.Site site = Master;
            site.MenuText = "BC Gov News";
            site.AddAppItem("BC Gov Corporate Calendar", "~/Calendar");
            site.AddAppItem("News Release Management", "~/News/ReleaseManagement/Drafts");

            site.AddNavigationItem("Files", "~/News/FileManagement");
            site.AddNavigationItem("Carousel", "~/News/Carousel");
            site.AddNavigationItem("Emergency Pin", "~/News/EmergencySlideManagement");
            site.AddNavigationItem("Live Feed", "~/News/LiveFeedManagement");
            //site.AddNavigationItem("Project Granville", "~/News/ProjectGranvilleManagement");

            if (!IsPostBack)
            {
                SetControlText(GetFeedState());
            }
        }

        protected void btnToggleLiveFeed(object sender, EventArgs ev)
        {
            bool newState = !GetFeedState();
            SetFeedState(newState);
            SetControlText(newState);
        }

        private void SetControlText(bool enabled)
        {
            enabled_Label.InnerHtml = enabled ? "Live Feed is Enabled" : "Live Feed is Disabled";
            save_Button.Text = enabled ? "Disable Live Feed" : "Enable Live Feed";
        }

        private bool GetFeedState()
        {
            HubEntities hub = new HubEntities();
            string enabled = hub.GetAppSetting(appSetting);
            return enabled.ToLower() == "true";
        }

        private void SetFeedState(bool enabled)
        {
            HubEntities hub = new HubEntities();
            hub.SetAppSetting(appSetting, enabled ? "true" : "false");
            hub.SaveChanges();
        }
    }
}