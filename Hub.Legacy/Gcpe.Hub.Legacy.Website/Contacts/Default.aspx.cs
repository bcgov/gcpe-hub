using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using MediaRelationsDatabase;
using MediaRelationsLibrary;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        searchFieldTb.Attributes.Add("placeholder", "Enter name");

        myReportsHomeContainer.ClientIDMode = ClientIDMode.Static;
        reportsHomeContainer.ClientIDMode = ClientIDMode.Static;

        ReportsLib lib = new ReportsLib();
        ReportsLib.PerformPageChecks();
        
        StringBuilder sb = new StringBuilder();
        using (MediaRelationsEntities ctx = new MediaRelationsEntities()) {

            List<Report> myReports = (from c in ctx.Reports 
                                      where c.IsPublic == false 
                                      where c.ReportOwner == lib.User 
                                      orderby c.SortOrder ascending select c).ToList();

            int count = 0;
            foreach (Report report in myReports) {
                sb.Append("<a href='" + ResolveUrl("~/Contacts/") + "Search.aspx?" + report.ReportQueryString + "' class='" + (count % 2 == 0 ? "even" : "odd") + "' onclick='ShowSearchLoadingModule()'>" + report.ReportName + 
                    " <img src='" + ResolveUrl("~/Contacts/") + "images/BigX@2x.png' border='0' onclick='return DeleteReport(\""+report.Id+"\")'/></a>\n");
                count++;
            }

            if (myReports.Count == 0) sb.Append("<div class='no-reports'>No Reports</div>\n");

            myReportsLit.Text = sb.ToString();

            sb.Clear();

            List<Report> reports = (from c in ctx.Reports
                                    where c.IsPublic == true
                                    orderby c.SortOrder ascending
                                    select c).ToList();

            count = 0;
            foreach (Report report in reports) {
                if (lib.CanDeletePublicReports) {
                    sb.Append("<a class='" + (count % 2 == 0 ? "even" : "odd") + "' href='" + ResolveUrl("~/Contacts/") + "Search.aspx?" + report.ReportQueryString + "' onclick='ShowSearchLoadingModule()'>" + report.ReportName +
                        " <img src='" + ResolveUrl("~/Contacts/") + "images/BigX@2x.png' border='0' onclick='return DeleteReport(\"" + report.Id + "\")'/></a>\n");
                } else {
                    sb.Append("<a class='" + (count % 2 == 0 ? "even" : "odd") + "' href='" + ResolveUrl("~/Contacts/") + "Search.aspx?" + report.ReportQueryString + "' onclick='ShowSearchLoadingModule()'>" + report.ReportName + "</a>\n");
                }
                count++;
            }

            if (reports.Count == 0) sb.Append("<div class='no-reports'>No Reports</div>\n");

            reportsLit.Text = sb.ToString();

        }
    }

    protected void SearchButtonClick(object sender, EventArgs e) {

        string url = "~/Contacts/Search.aspx?" + SearchLib.CriteriaType.Name + "=" + Server.UrlEncode(searchFieldTb.Text.Trim());
        Response.Redirect(url);

    }
}