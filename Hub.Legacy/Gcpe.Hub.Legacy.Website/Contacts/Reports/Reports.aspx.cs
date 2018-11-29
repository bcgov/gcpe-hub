using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using MediaRelationsDatabase;
using MediaRelationsLibrary;

public partial class Reports_Reports : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ReportsLib lib = new ReportsLib();
        ReportsLib.PerformPageChecks();

        if (!lib.CanCreatePublicReports)
        {
            orderByBottom.Visible = false;
            orderByTop.Visible = false;
            cbBottomTh.Visible = false;
            cbTopTh.Visible = false;

            topPaginator.BulkActions = false;
            bottomPaginator.BulkActions = false;
        }
        else
        {
            topPaginator.BulkActions = true;
            bottomPaginator.BulkActions = true;
            cbBottomTh.Visible = true;
            cbTopTh.Visible = true;

            topPaginator.BulkActionsEventHandler += TopPaginatorHandler;
            bottomPaginator.BulkActionsEventHandler += BottomPaginatorHandler;

            List<string> bulkActions = new List<string>();
            bulkActions.Add("Bulk Actions");
            if (lib.CanDeletePublicReports) bulkActions.Add("Delete");
            topPaginator.BulkActionItems = bottomPaginator.BulkActionItems = bulkActions;
        }

        StringBuilder sb = new StringBuilder();

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var results = (from c in ctx.Reports where c.IsPublic == true orderby c.SortOrder ascending select c);

            topPaginator.Count = bottomPaginator.Count = results.Count();

            int highestSortOrder = 0;
            int lowestSortOrder = 0;

            int count = results.Count();
            if (count > 0)
            {
                lowestSortOrder = results.Min(x => x.SortOrder);
                highestSortOrder = results.Max(x => x.SortOrder);
            }

            List<Report> reports = results.Skip(topPaginator.Skip).Take(topPaginator.PerPage).ToList();

            count = 0;
            foreach (Report report in reports)
            {
                sb.Append("<tr class='" + (count % 2 == 0 ? "even" : "odd") + "'>\n");

                if (lib.CanDeletePublicReports) sb.Append("<td><input type='checkbox' name='categoryAction' value='" + report.Id + "'/></td>\n");
                sb.Append("<td>" + report.ReportName + "</td>\n");
                sb.Append("<td>" + report.ReportOwner + "</td>\n");
                sb.Append("<td>" + report.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")) + "</td>\n");

                string searchUrl = ResolveUrl("~/Contacts/") + "Search.aspx?" + report.ReportQueryString;

                string orderHref = "";
                bool doShowUpArrow = true;
                bool doShowDownArrow = true;

                if (report.SortOrder <= lowestSortOrder) doShowUpArrow = false;
                if (report.SortOrder >= highestSortOrder) doShowDownArrow = false;

                Dictionary<string, string> qs = CommonMethods.GetEditableQueryString();
                qs.Remove("message");
                qs.Add("up", report.Id.ToString());

                orderHref += "<a class='gradient gradientBorder' href='" + Request.Url.AbsolutePath + "?" + CommonMethods.GetQueryString(qs) + "' " +
                    (!doShowUpArrow ? "style='visibility:hidden'" : "") + "><img src='" + ResolveUrl("~/Contacts/") + "images/Up.png' border='0'/></a>";


                qs.Remove("up");
                qs.Add("down", report.Id.ToString());

                orderHref += " <a class='gradient gradientBorder' href='" + Request.Url.AbsolutePath + "?" + CommonMethods.GetQueryString(qs) + "' " +
                    (!doShowDownArrow ? "style='visibility:hidden'" : "") + "><img src='" + ResolveUrl("~/Contacts/") + "images/down.png' border='0'/></a>\n";

                string actionString = "<a href='" + searchUrl + "' onclick='ShowSearchLoadingModule()'>View</a>";

                if (lib.CanDeletePublicReports)
                {
                    actionString += " | <a href='" + Request.Url.AbsolutePath + "?delete=" + report.Id + "' onclick='return confirm(deleteButtonText);'>Delete</a>";
                }

                sb.Append("<td>" + actionString + "</td>\n");

                if (lib.CanCreatePublicReports)
                {
                    sb.Append("<td class='updownarrows'>" + orderHref + "</td>\n");
                }

                sb.Append("</tr>\n");

                count++;
            }

            if (reports.Count == 0)
            {
                int colspan = 6;
                if (!lib.CanDeletePublicReports) colspan = 5;
                sb.Append("<tr><td colspan='" + colspan + "'>No items to display</td></tr>\n");
            }
        }

        tableContentLit.Text = sb.ToString();

    }

    private void PerformBulkActions(string selectedAction)
    {
        ReportsLib lib = new ReportsLib();

        string[] selectedItems = Request.Form.GetValues("categoryAction");
        if (selectedItems != null)
        {
            foreach (string guidStr in selectedItems)
            {
                if (selectedAction.Equals("Delete"))
                {
                    Guid guid;
                    if (Guid.TryParse(guidStr, out guid))
                    {
                        lib.DeleteReport(guid);
                    }
                }
            }
        }

        Response.Redirect(Request.Url.AbsolutePath + "?message=" + Server.UrlEncode("Bulk actions have been performed"));
    }

    protected void TopPaginatorHandler(object sender, EventArgs e)
    {
        PerformBulkActions(topPaginator.SelectedBulkAction);
    }

    protected void BottomPaginatorHandler(object sender, EventArgs e)
    {
        PerformBulkActions(bottomPaginator.SelectedBulkAction);
    }
}