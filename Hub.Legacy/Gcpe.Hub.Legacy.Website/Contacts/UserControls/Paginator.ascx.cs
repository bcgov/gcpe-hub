using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Text;
using MediaRelationsLibrary;

public partial class UserControls_Paginator : System.Web.UI.UserControl
{
    int count;
    public int Count
    {
        get { return count; }
        set
        {
            count = value;
            actionsPanel.Visible = (Count > 10 && PerPage > 10) || mode == "Bottom";
        }
    }

    public string PaginatorID { get; set; }

    public List<string> BulkActionItems { get; set; }

    public string EntityType { get; set; }

    int perpage = 10;

    public int PerPage
    {
        get { return perpage; }
        set
        {
            perpage = value;
            actionsPanel.Visible = (Count > 10 && PerPage > 10) || mode == "Bottom";
        }
    }

    int page = 1;
    public int ResultsPage
    {
        get { return page; }
        set { page = value; }
    }

    public int Skip
    {
        get { return (page - 1) * perpage; }
    }

    string mode = "top";
    public string Mode
    {
        set
        {
            mode = value;
            actionsPanel.Visible = (Count > 10 && PerPage > 10) || mode == "Bottom";
        }
    }

    public string SelectedBulkAction
    {
        get { return bulkActionsDropDown.SelectedValue; }
    }

    private bool bulkActions = false;
    public bool BulkActions
    {
        get
        {
            return bulkActions;
        }
        set
        {
            bulkActions = value;
            bulkActionsPanel.Visible = bulkActions;
        }
    }

    public bool PageVisible
    {
        get { return pagePanel.Visible; }
        set { pagePanel.Visible = value; }
    }

    public EventHandler BulkActionsEventHandler { get; set; }

    string page_param = "page";
    string perpage_param = "perpage";
    string perpage_session_param = "mru_items_per_page";

    protected void Page_Init()
    {
        if (PaginatorID != null)
        {
            page_param = PaginatorID + "_page";
            perpage_param = PaginatorID + "_perpage";
        }

        if (Request.QueryString[page_param] != null)
            Int32.TryParse(Request.QueryString[page_param], out page);

        if (Request.QueryString[perpage_param] == null)
        {
            if (Session[perpage_session_param] != null)
                perpage = (int)Session[perpage_session_param];
        }
        else
        {
            int.TryParse(Request.QueryString[perpage_param], out perpage);
            Session[perpage_session_param] = perpage;
        }

        if (!mode.Equals("top"))
        {
            pageItemCountPanel.Visible = false;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (BulkActions)
        {
            bulkActionsPanel.Visible = true;
        }
        bulkActionsBtn.OnClientClick += "return ConfirmBulkActions('" + bulkActionsDropDown.ClientID + "');";

        if (BulkActions && BulkActionItems != null)
        {
            if (!IsPostBack)
            {
                foreach (string action in BulkActionItems)
                {
                    bulkActionsDropDown.Items.Add(new ListItem(action, action));
                }
            }
        }

        List<KeyValuePair<String, String>> qs = CommonMethods.GetEditableQueryStringMultiple();


        CommonMethods.RemoveItemFromQueryString(qs, "message");
        CommonMethods.RemoveItemFromQueryString(qs, "delete");
        CommonMethods.RemoveItemFromQueryString(qs, perpage_param);
        CommonMethods.RemoveItemFromQueryString(qs, page_param);


        // per page 

        if (!IsPostBack)
        {
            perPageDD.Items.Add(new ListItem("5", "5"));
            perPageDD.Items.Add(new ListItem("10", "10"));
            perPageDD.Items.Add(new ListItem("25", "25"));
            perPageDD.Items.Add(new ListItem("50", "50"));

            perPageDD.SelectedValue = perpage.ToString();
        }

        perPageDD.SelectedIndexChanged += ChangeSelectedPerPage;
        perPageDD.AutoPostBack = true;


        // items count        
        qs.Add(new KeyValuePair<string, string>(perpage_param, perpage.ToString()));

        int pages = (int)Math.Ceiling((double)Count / perpage);

        int startPage = page - 2;
        if (startPage < 1) startPage = 1;

        int endPage = startPage + 4;
        if (endPage > pages) endPage = pages;

        int startRow = ((page - 1) * perpage) + 1;
        int endRow = page * perpage;
        if (endRow > Count) endRow = Count;

        if (Count == 0)
        {
            startRow = 0;
            endRow = 0;
        }

        rowNumbersLit.Text = startRow + " to " + endRow + " of <span class='paginator-total-count'>" + Count + "</span>";
        itemCountLit.Text = Count.ToString();

        // page display
        StringBuilder sb = new StringBuilder();

        string prevUrl = Request.Url.AbsolutePath + "?" + CommonMethods.GetQueryStringMultiple(qs) + (qs.Count > 0 ? "&" : "?") + page_param + "=" + (page - 1);
        if (page != 1) sb.Append("<a href='" + prevUrl + "' class='prev gradient item arrow'><img src='" + ResolveUrl("~/Contacts/") + "images/prev.png' border='0'/></a>");

        for (int i = startPage; i <= endPage; i++)
        {

            if (i == page) sb.Append("<span class='gradient item'>" + i + "</span>");
            else
            {
                string thisUrl = Request.Url.AbsolutePath + "?" + CommonMethods.GetQueryStringMultiple(qs) + (qs.Count > 0 ? "&" : "?") + page_param + "=" + i;
                sb.Append("<a href='" + thisUrl + "' class='gradient item'>" + i + "</a>");
            }

        }

        string nextUrl = Request.Url.AbsolutePath + "?" + CommonMethods.GetQueryStringMultiple(qs) + (qs.Count > 0 ? "&" : "?") + page_param + "=" + (page + 1);
        if (pages != 0 && page != pages) sb.Append("<a href='" + nextUrl + "' class='next gradient item arrow'><img src='" + ResolveUrl("~/Contacts/") + "images/Next.png' border='0'/></a>");



        pageLinksLit.Text = sb.ToString();
    }

    protected void ChangeSelectedPerPage(object sender, EventArgs e)
    {
        //perPageDD.Attributes.Add("onchange", "window.location='" + Request.Path + "?" + CommonMethods.GetQueryString(qs) + (qs.Count > 0 ? "&" : "") + page_param + "=1&" + perpage_param + "='+this.options[this.selectedIndex].value\">");*/

        /*Dictionary<String, String> qs = CommonMethods.GetEditableQueryString();

        if (qs.ContainsKey("message")) qs.Remove("message");
        if (qs.ContainsKey("delete")) qs.Remove("delete");
        if (qs.ContainsKey(perpage_param)) qs.Remove(perpage_param);
        if (qs.ContainsKey(page_param)) qs.Remove(page_param);

        Response.Redirect(Request.Url.AbsolutePath + "?" + CommonMethods.GetQueryString(qs) + (qs.Count > 0 ? "&" : "") + perpage_param + "=" + perPageDD.SelectedValue);*/

        List<KeyValuePair<string, string>> qs = CommonMethods.GetEditableQueryStringMultiple();
        CommonMethods.RemoveItemFromQueryString(qs, "message");
        CommonMethods.RemoveItemFromQueryString(qs, "delete");
        CommonMethods.RemoveItemFromQueryString(qs, perpage_param);
        CommonMethods.RemoveItemFromQueryString(qs, page_param);

        Response.Redirect(Request.Url.AbsolutePath + "?" + CommonMethods.GetQueryStringMultiple(qs) + (qs.Count > 0 ? "&" : "") + perpage_param + "=" + perPageDD.SelectedValue);

    }

    protected void BulkActionsClick(object sender, EventArgs e)
    {
        if (BulkActionsEventHandler != null) BulkActionsEventHandler(sender, e);
    }
}