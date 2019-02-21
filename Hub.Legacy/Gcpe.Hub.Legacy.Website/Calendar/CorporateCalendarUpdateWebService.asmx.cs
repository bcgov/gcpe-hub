using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using CorporateCalendar.Data;
using HtmlAgilityPack;

/// <summary>
/// Summary description for CorporateCalendarUpdateWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, we need the following line. 
[System.Web.Script.Services.ScriptService]
public class CorporateCalendarUpdateWebService : System.Web.Services.WebService
{

    public enum NewsFeedType
    {
        Today,
        LastestFive,
        BetweenDates
    }

    public IEnumerable<NewsFeed> GetCustomDateRangeNewsFeedItems(CorporateCalendarDataContext ctx, DateTime? fromDate, DateTime? toDate, string updateType, string keyword)
    {
        var customPrincipal = new CorporateCalendar.Security.CustomPrincipal(System.Web.HttpContext.Current.User.Identity);

        var items = ctx.GetCorpCalendarUpdatesBetweenDates(customPrincipal.Id,
            customPrincipal.IsInApplicationOwnerOrganizations, null,
            fromDate, toDate, updateType);

        List<NewsFeed> items_limited = null;
        if (keyword != string.Empty)
        {
            // Search Id, Title, Details, City
            items_limited = items.ToList().FindAll(
                delegate(CorporateCalendar.Data.NewsFeed p)
                {
                    return (p.Text.ToLower()).Contains(keyword);
                }
                );

        }

        return items_limited == null ? items.OrderByDescending(n => n.Id).Take(1000)
                                     : items_limited.OrderByDescending(n => n.Id).Take(1000);
    }

    public IOrderedEnumerable<NewsFeed> GetActivityNewsFeedItems(CorporateCalendarDataContext ctx, int activityId)
    {
        var customPrincipal = new CorporateCalendar.Security.CustomPrincipal(System.Web.HttpContext.Current.User.Identity);

        List<NewsFeed> items = null;
        items = ctx.GetCorpCalendarUpdatesBetweenDates(customPrincipal.Id,
                customPrincipal.IsInApplicationOwnerOrganizations,
                activityId, null, null, null).
                OrderByDescending(n => n.Id).ToList();

        return items.OrderByDescending(n => n.Id);
    }

    public IOrderedEnumerable<NewsFeed> GetNewsFeedItems(CorporateCalendarDataContext ctx, NewsFeedType type)
    {
        var customPrincipal = new CorporateCalendar.Security.CustomPrincipal(System.Web.HttpContext.Current.User.Identity);

        IOrderedEnumerable<NewsFeed> newsFeedItems = null;

        switch (type)
        {
            case NewsFeedType.Today:
                newsFeedItems = ctx.GetCorpCalendarUpdatesToday(customPrincipal.Id,
                    customPrincipal.IsInApplicationOwnerOrganizations).
                    OrderByDescending(n => n.Id);
                break;
            case NewsFeedType.LastestFive:
            default:
                newsFeedItems =
                    ctx.GetCorpCalendarUpdates(customPrincipal.Id,
                    customPrincipal.IsInApplicationOwnerOrganizations).
                    OrderByDescending(n => n.Id);
                break;
        }


        return newsFeedItems;
    }

    private static IEnumerable<NewsFeed> ModifyNewsFeedItems(IEnumerable<NewsFeed> newsFeedItems)
    {
        var modifiedNewsFeedItems = new List<CorporateCalendar.Data.NewsFeed>();

        // Use HTML Agility Pack to parse html snippet stored in NewsFeed.Text
        var htmlDocument = new HtmlDocument();

        foreach (var newsFeedItem in newsFeedItems)
        {
            string modifiedNewsFeedItemText = newsFeedItem.Text;
            int posOpenSpan = modifiedNewsFeedItemText.IndexOf("<span");
            if (posOpenSpan != -1)
            {
                const string token = "</span>&nbsp;";
                int posCloseSpan = modifiedNewsFeedItemText.IndexOf(token, posOpenSpan);
                modifiedNewsFeedItemText = modifiedNewsFeedItemText.Substring(0, posOpenSpan) + modifiedNewsFeedItemText.Substring(posCloseSpan + token.Length);
            }

            int posAt = modifiedNewsFeedItemText.LastIndexOf("</a> at");
            int posClosing = posAt != -1 ? modifiedNewsFeedItemText.LastIndexOf("\">", posAt) : -1;
            if (posClosing != -1)
            {
                // Change the onhover activity Title to be in the main body and add the activity Detail on hover
                modifiedNewsFeedItemText = modifiedNewsFeedItemText.Substring(0, posAt) + ")" + modifiedNewsFeedItemText.Substring(posAt + 4);
                modifiedNewsFeedItemText = modifiedNewsFeedItemText.Substring(0, posClosing) + " - Start date: " + newsFeedItem.Activity.StartDateTime.Value.ToShortDateString() + " End date: " + newsFeedItem.Activity.EndDateTime.Value.ToShortDateString() + "</a> (" + modifiedNewsFeedItemText.Substring(posClosing + 2);
                modifiedNewsFeedItemText = modifiedNewsFeedItemText.Replace("style=\"color: Blue\" title=\"", "style='color: Blue' title='" + newsFeedItem.Activity.Details + "'>");
            }
            newsFeedItem.Text = modifiedNewsFeedItemText;
            modifiedNewsFeedItems.Add(newsFeedItem);
        }
        return modifiedNewsFeedItems;
    }

    private string GetHtml(IEnumerable<NewsFeed> newsFeedItems, out string count)
    {
        var modifiedNewsFeedItems = ModifyNewsFeedItems(newsFeedItems);
        var repeater = new System.Web.UI.WebControls.Repeater
        {
            ItemTemplate = new CorporateCalendarUpdateTemplate(ListItemType.Item),
            DataSource = modifiedNewsFeedItems
        };

        repeater.DataBind();
        count = "<span id='totalCount'>" + repeater.Items.Count + "</span>";
        var stringBuilder = new StringBuilder();
        var stringWriter = new StringWriter(stringBuilder);
        var htmlTextWriter = new HtmlTextWriter(stringWriter);

        repeater.RenderControl(htmlTextWriter);

        return stringBuilder.ToString();
    }


    [WebMethod]
    public string GetLatestUpdates()
    {
        string count;
        using (var ctx = new CorporateCalendarDataContext())
        {
            IOrderedEnumerable<NewsFeed> newsFeedItems = GetNewsFeedItems(ctx, NewsFeedType.LastestFive);
            string html = GetHtml(newsFeedItems, out count);

            return "<h1>Latest 5 updates:</h1><p>Total " + count + " items</p>"
                + string.Format("<ul id=\"CorporateCalendarUpdatesUnorderedList\" style=\"margin-top: -8px; padding-left: 5px; margin: 0px;\">{0}</ul>", html);
        }
    }

    [WebMethod]
    public string GetTodaysUpdates()
    {
        string count;
        using (var ctx = new CorporateCalendarDataContext())
        {
            IOrderedEnumerable<NewsFeed> newsFeedItems = GetNewsFeedItems(ctx, NewsFeedType.Today);
            string html = GetHtml(newsFeedItems, out count);

            return "<h1>Today's updates:</h1><p>Total " + count + " items</p>"
                + string.Format("<ul id=\"CorporateCalendarUpdatesUnorderedList\" style=\"margin-top: -8px; padding-left: 5px; margin: 0px;\">{0}</ul>", html);
        }
    }

    //[WebMethod, ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [WebMethod]
    public string GetUpdatesBetweenDates(string fromDate, string toDate, string updateType, string keyword)
    {
        string title = "updated";
        if (string.IsNullOrEmpty(updateType))
        {
            updateType = null;
        }
        else
        {
            title = updateType;
            int suffixLength = updateType == "added" || updateType == "reviewed" ? 2 : 1;
            updateType = updateType.Substring(0, updateType.Length - suffixLength);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            int activityId;
            string[] activityIdString = keyword.Split('-');
            if (int.TryParse(activityIdString[activityIdString.Length - 1], out activityId) && activityId > 10000)
            {
                return GetUpdatesForActivity(activityId);
            }
        }
        string count;
        bool hasFromDate = !string.IsNullOrEmpty(fromDate);
        bool hasToDate = !string.IsNullOrEmpty(toDate);

        using (var ctx = new CorporateCalendarDataContext())
        {
            IEnumerable<NewsFeed> newsFeedItems = GetCustomDateRangeNewsFeedItems(ctx, hasFromDate ? (DateTime?)Convert.ToDateTime(fromDate) : null,
                                                                              hasToDate ? (DateTime?)Convert.ToDateTime(toDate) : null, updateType, keyword.ToLower().Trim());
            string html = GetHtml(newsFeedItems, out count);
            html = string.Format(":</h1><p>Total {3} items</p><ul id=\"CorporateCalendarUpdatesUnorderedList\" style=\"margin-top: -8px; padding-left: 5px; margin: 0px;\">{2}</ul>",
                fromDate, toDate, html, count);

            if (hasFromDate)
            {
                title += " from " + fromDate;
            }
            if (hasToDate)
            {
                title += " to " + toDate;
            }

            return "<h1>Activities " + title + html;
        }
    }

    //[WebMethod, ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [WebMethod]
    public string GetUpdatesForActivity(int activityId)
    {
        string count;
        using (var ctx = new CorporateCalendarDataContext())
        {
            IOrderedEnumerable<NewsFeed> newsFeedItems = GetActivityNewsFeedItems(ctx, activityId);
            string html = GetHtml(newsFeedItems, out count);
            html = string.Format("<h1>Updates for Activity {0}:</h1><p>Total {2} items</p><ul id=\"CorporateCalendarUpdatesUnorderedList\" style=\"margin-top: -8px; padding-left: 5px; margin: 0px;\">{1}</ul>",
                activityId, html, count);

            return html;
        }
    }

}

public class CorporateCalendarUpdateTemplate : System.Web.UI.ITemplate
{
    System.Web.UI.WebControls.ListItemType templateType;
    public CorporateCalendarUpdateTemplate(System.Web.UI.WebControls.ListItemType type)
    {
        templateType = type;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        var literal = new System.Web.UI.WebControls.Literal();

        switch (templateType)
        {

            default:
                literal.DataBinding += new EventHandler(Item_DataBinding);
                break;

        }
        container.Controls.Add(literal);
    }

    static void Item_DataBinding(object sender, System.EventArgs e)
    {
        var literal = (System.Web.UI.WebControls.Literal)sender;
        RepeaterItem ri = (RepeaterItem)literal.NamingContainer;
        string item1Value = (string)DataBinder.Eval(ri.DataItem, "Text");
        literal.Text = "<li id=\"NewsFeedItem\"  style=\"list-style: none\">" + item1Value + "</li>";
    }
}
