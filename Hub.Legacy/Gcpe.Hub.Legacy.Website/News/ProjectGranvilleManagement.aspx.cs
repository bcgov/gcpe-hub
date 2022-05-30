extern alias legacy;

using System;
using legacy::Gcpe.Hub.Data.Entity;

namespace Gcpe.Hub.News
{
    public partial class ProjectGranvilleManagement : Hub.News.Page
    {
        string appSetting = "granville";

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
            site.AddNavigationItem("Project Granville", "~/News/ProjectGranvilleManagement");
            if (!IsPostBack)
            {
                SetControlText(GetFeedState());
            }
        }

        protected void btnToggleProjectGranville(object sender, EventArgs ev)
        {
            bool newState = !GetFeedState();
            SetFeedState(newState);
            SetControlText(newState);
        }

        private void SetControlText(bool enabled)
        {
            enabled_Label.InnerHtml = enabled ? "Project Granville is Enabled" : "Project Granville is Disabled";
            save_Button.Text = enabled ? "Disable Project Granville" : "Enable Project Granville";
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