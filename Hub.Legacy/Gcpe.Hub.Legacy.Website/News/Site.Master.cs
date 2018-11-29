using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gcpe.Hub.News
{
    public partial class Site : System.Web.UI.MasterPage
    {
        List<NavigationItem> navigationItems = new List<NavigationItem>();
        List<NavigationItem> appItems = new List<NavigationItem>();

        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (NavigationItem item in navigationItems)
            {
                item.IsActive = Page.Request.AppRelativeCurrentExecutionFilePath == item.NavigateUrl
                             || Page.Request.AppRelativeCurrentExecutionFilePath.StartsWith(item.NavigateUrl + "/")
                             || Page.Request.AppRelativeCurrentExecutionFilePath.StartsWith(item.NavigateUrl + "?");
            }

            navbarRepeater.DataSource = navigationItems;
            //TODO: Determine if this line is necessary
            //navbarRepeater.DataBind();
            appsRepeater.DataSource = appItems;
            if (!IsPostBack)
            {
                DataBind();

                ScriptManager.GetCurrent(Page).RegisterPostBackControl(txtSearch);
            }

            //PostBackTrigger trigger = new PostBackTrigger();

            //updatePanel.Triggers.Add(trigger);
            //updatePanel.Triggers.Remove(trigger);
            favicon.Href = Gcpe.Hub.Configuration.App.Settings.FaviconImg;
        }

        public string MenuText { get; set; }

        public string MenuNavigateUrl { get; set; }

        public ICollection<NavigationItem> NavigationItems
        {
            get
            {
                return navigationItems;
            }
        }

        public void AddNavigationItem(string text, string navigateUrl)
        {
            navigationItems.Add(
                new Hub.News.NavigationItem() { Text = text, NavigateUrl = navigateUrl }
            );
        }

        public void SetActiveNavigationItem(string text)
        {
            navigationItems.ForEach(e => e.IsActive = e.Text == text);
        }

        public ICollection<NavigationItem> AppItems
        {
            get
            {
                return appItems;
            }
        }

        public void AddAppItem(string text, string navigateUrl)
        {
            appItems.Add(
                new Hub.News.NavigationItem() { Text = text, NavigateUrl = navigateUrl }
            );
        }

        public string HelpPage { get; set; }

        public string GetHelpUrl()
        {
            return String.Format(Gcpe.Hub.Configuration.App.Settings.NewsHelpUrl, HelpPage);
        }

        //protected void txtSearch_TextChanged(object sender, EventArgs e)
        //{
        //    if (txtSearch.Text.Trim() != "")
        //    {
        //        Response.Redirect(ResolveUrl("~/Search") + "?q=" + HttpUtility.UrlEncode(txtSearch.Text.Trim()));
        //    }
        //    else
        //    {   
        //        DataBind();
        //    }
        //}
    }
}