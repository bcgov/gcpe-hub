using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaRelationsSiteApp.DataManagement
{
    using MediaRelationsLibrary;

    public class ManageColumn
    {
        public string Text { get; set; }
        public string PropertyName { get; set; }
    }

    public class ManageRow
    {
        public Guid Guid;
        public bool? IsActive;
        public int? SortOrder;
        public IEnumerable<string> Values { get; set; }
    }

    public delegate KeyValuePair<bool, string> ManageUpdateMessageHandler(Guid guid);
    public delegate void ManageUpdateHandler(Guid guid);

    public partial class Manage : System.Web.UI.MasterPage
    {
        public string SearchText { get; private set; }
        public string SortProperty { get; private set; }
        public bool SortDescending = false;

        //Moved From Page's Global Context
        protected bool canCreate = false;
        protected bool canUpdate = false;
        protected bool canDelete = false;

        protected string DataNameSingle { get; set; }
        protected string DataNamePlural { get; set; }

        protected ManageUpdateMessageHandler Delete;
        //protected ManageUpdateHandler Activate;
        //protected ManageUpdateHandler Deactivate;
        protected ManageUpdateHandler MoveUp;
        protected ManageUpdateHandler MoveDown;

        protected string AddUrl
        {
            get
            {
                return addUrlLink.NavigateUrl;
            }
            set
            {
                addUrlLink.NavigateUrl = value;
            }
        }
        protected int? LowestSortOrder { get; set; }
        protected int? HighestSortOrder { get; set; }

        public bool IsSortOrderDistinct
        {
            get
            {
                return Rows.Select(e => e.SortOrder).Distinct().Count() == Rows.Count();
            }
        }

        public void Register(string entityName, string entityPlural, string defaultSort, string addUrl, ManageUpdateMessageHandler deleteMethod)
        {
            DataNameSingle = entityName;
            DataNamePlural = entityPlural;
            if (SortProperty == null)
                SortProperty = defaultSort;
            AddUrl = addUrl;
            Delete = deleteMethod;
        }

        /*public void RegisterActivate(ManageUpdateHandler activateMethod, ManageUpdateHandler deactivateMethod)
        {
            Activate = activateMethod;
            Deactivate = deactivateMethod;
        }*/

        public void RegisterSortOrder(ManageUpdateHandler moveUpMethod, ManageUpdateHandler moveDownMethod, int? minSortOrder, int? maxSortOrder)
        {
            MoveUp = moveUpMethod;
            MoveDown = moveDownMethod;
            LowestSortOrder = minSortOrder;
            HighestSortOrder = maxSortOrder;
        }

        public void RegisterData(IEnumerable<ManageColumn> columns, IEnumerable<ManageRow> rows, int rowCount)
        {
            Columns = columns;
            Rows = rows;
            PaginatorTopCount = rowCount;

            PaginatorTop.EntityType = rowCount == 1 ? DataNameSingle : DataNamePlural;
            PaginatorBottom.EntityType = rowCount == 1 ? DataNameSingle : DataNamePlural;
        }

        public IEnumerable<ManageColumn> Columns
        {
            get
            {
                return (IEnumerable<ManageColumn>)columnHeaderRepeater.DataSource;
            }
            set
            {
                columnHeaderRepeater.DataSource = value;
                columnHeaderRepeater.DataBind();

                columnFooterRepeater.DataSource = value;
                columnFooterRepeater.DataBind();
            }
        }

        public IEnumerable<ManageRow> Rows
        {
            get
            {
                return rowRepeater.DataSource as IEnumerable<ManageRow>;
            }
            set
            {
                rowRepeater.DataSource = value;
                rowRepeater.DataBind();
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            //Moved From Page's Page_Init Method
            PaginatorTop.BulkActionsEventHandler += BulkActionsHandlerTop;
            PaginatorBottom.BulkActionsEventHandler += BulkActionsHandlerBottom;

            CommonEventLogging logger = new CommonEventLogging();
            PermissionContainer1.Logger = logger;

            canCreate = ((Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists, logger) & Permissions.SiteAction.Create) != 0);
            canUpdate = ((Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists, logger) & Permissions.SiteAction.Update) != 0);
            canDelete = ((Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists, logger) & Permissions.SiteAction.Delete) != 0);

            if (!canCreate) addNew.Visible = false;

            List<string> actions = new List<string>();
            actions.Add("Bulk Actions");
            if (canDelete) actions.Add("Delete");

            PaginatorTop.BulkActionItems = actions;
            PaginatorBottom.BulkActionItems = actions;

            //Moved From Page's Page_Load Method
            if (Request.QueryString["sort"] != null)
                SortProperty = Request.QueryString["sort"];

            if (Request.QueryString["sortDir"] != null)
            {
                if (Request.QueryString["sortDir"].ToLower().Equals("desc"))
                    SortDescending = true;
            }

            if (Request.QueryString["search"] != null)
            {
                SearchText = Request.QueryString["search"];
                
                if (!IsPostBack)
                    SearchBox.Text = SearchText;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.InnerText = DataNamePlural;
            Label2.InnerText = DataNameSingle;
            addUrlLink.Text = " ADD NEW " + DataNameSingle;

            //Moved From Page's Page_Load Method
            Guid guid;
            if (Guid.TryParse(Request.QueryString["delete"], out guid))
            {
                string message;
                KeyValuePair<bool, string> succ = Delete(guid);

                if (!succ.Key) message = succ.Value;
                else message = "The " + DataNameSingle + " has been successfully deleted";

                Response.Redirect(Request.Url.AbsolutePath + "?message=" + Server.UrlEncode(message));
            }

            if (Guid.TryParse(Request.QueryString["up"], out guid))
            {
                MoveUp(guid);
                Dictionary<string, string> oqs = CommonMethods.GetEditableQueryString();
                oqs.Remove("up");
                Response.Redirect(Request.Url.AbsolutePath + "?" + CommonMethods.GetQueryString(oqs));
            }

            if (Guid.TryParse(Request.QueryString["down"], out guid))
            {
                MoveDown(guid);
                Dictionary<string, string> oqs = CommonMethods.GetEditableQueryString();
                oqs.Remove("down");
                Response.Redirect(Request.Url.AbsolutePath + "?" + CommonMethods.GetQueryString(oqs));
            }

            System.Diagnostics.Debug.Assert(DataNameSingle != null);
            System.Diagnostics.Debug.Assert(DataNamePlural != null);
            System.Diagnostics.Debug.Assert(canUpdate ? AddUrl != null : true);
            System.Diagnostics.Debug.Assert(canDelete ? Delete != null : true);
            System.Diagnostics.Debug.Assert((MoveUp != null) == (MoveDown != null));
            System.Diagnostics.Debug.Assert(MoveUp != null ? LowestSortOrder.HasValue && HighestSortOrder.HasValue : true);
            System.Diagnostics.Debug.Assert(Rows.Any(row => row.SortOrder.HasValue) == (LowestSortOrder.HasValue && HighestSortOrder.HasValue));
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> queryString = CommonMethods.GetEditableQueryString();

            queryString.Remove("message");
            queryString.Remove("page");
            queryString.Remove("delete");

            if (!queryString.ContainsKey("search")) queryString.Add("search", SearchBox.Text);
            else queryString["search"] = SearchBox.Text;

            Response.Redirect(Request.Path + "?" + CommonMethods.GetQueryString(queryString));
        }

        private int PaginatorTopCount
        {
            get { return PaginatorTop.Count; }
            set
            {
                PaginatorTop.Count = value;
                PaginatorBottom.Count = value;
            }
        }

        public int PaginatorTopSkip
        {
            get { return PaginatorTop.Skip; }
        }

        public int PaginatorTopPerPage
        {
            get { return PaginatorTop.PerPage; }
        }

        protected void BulkActionsHandlerTop(object sender, EventArgs e)
        {
            string selectedAction = PaginatorTop.SelectedBulkAction;

            PerformBulkAction(selectedAction);
        }

        protected void BulkActionsHandlerBottom(object sender, EventArgs e)
        {
            string selectedAction = PaginatorBottom.SelectedBulkAction;

            PerformBulkAction(selectedAction);
        }

        private void PerformBulkAction(string selectedAction)
        {
            string message = "";

            if (Request.Form.GetValues("categoryAction") != null)
            {
                string[] values = Request.Form.GetValues("categoryAction");

                List<string> errorMessages = new List<string>();

                foreach (string val in values)
                {
                    if (selectedAction.Equals("Delete"))
                    {
                        if (Delete == null)
                            throw new NotImplementedException();

                        KeyValuePair<bool, string> error = Delete(Guid.Parse(val));
                        if (!error.Key) errorMessages.Add(error.Value);
                    }
                }

                if (errorMessages.Count() > 0)
                {
                    message = "There were errors in the request : " + string.Join(",", errorMessages.ToArray());
                }
                else
                {
                    message = "Actions have been successfully performed";
                }
            }

            Response.Redirect(Request.Path + (!string.IsNullOrEmpty(message) ? "?message=" + Server.UrlEncode(message) : ""));
        }

        public string GetUpDownControls(ManageRow row)
        {
            string orderHref = "";
            bool doShowUpArrow = true;
            bool doShowDownArrow = true;

            if (row.SortOrder <= LowestSortOrder) doShowUpArrow = false;
            if (row.SortOrder >= HighestSortOrder) doShowDownArrow = false;

            Dictionary<string, string> qs = CommonMethods.GetEditableQueryString();
            qs.Remove("message");
            qs.Remove("up");
            qs.Remove("down");

            qs.Add("up", row.Guid.ToString());

            orderHref += "<a class='gradient gradientBorder' href='" + Request.Url.AbsolutePath + "?" + CommonMethods.GetQueryString(qs) + "' " +
                (!doShowUpArrow ? "style='visibility:hidden'" : "") + "><img src='" + ResolveUrl("~/Contacts/") + "images/Up.png' border='0'/></a>";

            qs.Remove("up");
            qs.Remove("down");
            qs.Add("down", row.Guid.ToString());

            orderHref += " <a class='gradient gradientBorder' href='" + Request.Url.AbsolutePath + "?" + CommonMethods.GetQueryString(qs) + "' " +
                (!doShowDownArrow ? "style='visibility:hidden'" : "") + "><img src='" + ResolveUrl("~/Contacts/") + "images/down.png' border='0'/></a>\n";

            return orderHref;
        }
    }
}