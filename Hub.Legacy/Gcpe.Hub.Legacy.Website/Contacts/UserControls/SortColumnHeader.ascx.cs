using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using MediaRelationsLibrary;

public partial class UserControls_SortColumnHeader : System.Web.UI.UserControl
{
    public string Text { get; set; }
    public bool IsDefault { get; set; }

    public string OverrideSort { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        string currentSort = Request.QueryString["sort"];
        string currentSortDir = Request.QueryString["sortDir"];

        List<KeyValuePair<String, String>> qs = CommonMethods.GetEditableQueryStringMultiple();
        CommonMethods.RemoveItemFromQueryString(qs, "message");
        CommonMethods.RemoveItemFromQueryString(qs, "delete");
        CommonMethods.RemoveItemFromQueryString(qs, "sort");
        CommonMethods.RemoveItemFromQueryString(qs, "sortDir");

        string thisText = (!string.IsNullOrWhiteSpace(OverrideSort) ? OverrideSort : (ColumnName ?? Text));

        qs.Add(new KeyValuePair<string, string>("sort", thisText));

        string sd = "asc";
        if (currentSort == null && IsDefault || currentSort != null && thisText != null && currentSort.Equals(thisText))
        {
            if (currentSortDir == null || currentSortDir.Equals("asc"))
                sd = "desc";
        }
        qs.Add(new KeyValuePair<string, string>("sortDir", sd));

        sortLink.InnerHtml = Text + " <img src='" + ResolveUrl("~/Contacts/") + "images/Sort.png' border='0'/>";
        sortLink.HRef = Request.Path + "?" + CommonMethods.GetQueryStringMultiple(qs);
        if (currentSort == null && IsDefault || currentSort != null && thisText != null && currentSort.Equals(thisText))
        {
            if (currentSortDir == null || currentSortDir.Equals("asc"))
            {
                sortLink.Attributes["class"] = "sort-asc";
            }
            else
            {
                sortLink.Attributes["class"] = "sort-desc";
            }
        }
    }

    public string ColumnName { get; set; }
}