using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using MediaRelationsDatabase;
using MediaRelationsLibrary;

public partial class MasterPage_MediaRelationsResponsive : System.Web.UI.MasterPage
{

    private List<Report> reports = null;
    private List<Report> myReports = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        ReportsLib lib = new ReportsLib();
        using (MediaRelationsEntities ctx = new MediaRelationsEntities()) {
            myReports = (from c in ctx.Reports where c.ReportOwner == lib.User where c.IsPublic == false orderby c.SortOrder ascending select c).ToList();
            reports = (from c in ctx.Reports where c.IsPublic == true orderby c.SortOrder ascending select c).ToList();            
        }
    
        // if (!IsPostBack) SetupPageNavigation(); // NTK - In case we want mobile navigation again and not to go to home page
    }

    private void SetupPageNavigation() {
        CommonEventLogging logger = new CommonEventLogging();


        AdminNavigationLib anl = new AdminNavigationLib();
        // mobile nav               
        int selectedIndex = -1;
        List<MobileNavItem> mobileItems = anl.MobileNavigationItems(out selectedIndex, logger);

        StringBuilder sb = new StringBuilder();
        int count = 0;
        foreach (MobileNavItem item in mobileItems) {
            string className = "";
            if (count == selectedIndex) className = "selected";            

            if (item.Section != null && item.Section == Permissions.SiteSection.MediaRelationsUserReports) {
                sb.Append("<a href='javascript:void(0);' class='" + className + "' onclick='toggleMobileNavItem(this, \"my-reports-mobile\")'>" + item.Name + " <div></div></a>\n");
                sb.Append("<div id='my-reports-mobile' class='mobile-subnav-container'>\n");
                foreach (Report report in myReports) {
                    sb.Append("<a href='" + ResolveUrl("~/Contacts/") + "Search.aspx?" + report.ReportQueryString+"' class='report-item'>"+report.ReportName+"</a>\n");
                }

                if (myReports.Count == 0) {
                    sb.Append("<a href='#' class='report-item'>No Reports</a>\n");
                }

                sb.Append("</div>\n");
            } else if (item.Section != null && item.Section == Permissions.SiteSection.MediaRelationsCommonReports) {                
                sb.Append("<a href='javascript:void(0);' class='" + className + "' onclick='toggleMobileNavItem(this, \"reports-mobile\")'>" + item.Name + " <div></div></a>\n");
                sb.Append("<div id='reports-mobile' class='mobile-subnav-container'>\n");
                foreach (Report report in reports) {
                    sb.Append("<a href='" + ResolveUrl("~/Contacts/") + "Search.aspx?" + report.ReportQueryString + "' class='report-item'>" + report.ReportName + "</a>\n");
                }

                if (reports.Count == 0) {
                    sb.Append("<a href='#' class='report-item'>No Reports</a>\n");
                }

                sb.Append("</div>\n");
            } else {
                sb.Append("<a href='" + item.Url + "' class='" + className + "'>" + item.Name + "</a>\n");
            }

            count++;
        }

        mobileNavigationLit.Text = sb.ToString();       

    }

}
