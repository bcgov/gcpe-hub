using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Gcpe.Hub.News
{
    public partial class Search : Hub.News.Page
    {
        public SearchModel Model;

        protected void Page_Load(object sender, EventArgs e)
        {
            //txtKeyword.Text = "Test";
            Hub.News.Site site = Master;

            site.MenuText = "Hub";
            site.MenuNavigateUrl = "~/News/Search";

            site.AddNavigationItem("Search", "~/News/Search");
            site.AddNavigationItem("News Release Management", "~/News/ReleaseManagement/Drafts");

            site.HelpPage = "Hub - Search";


            if (!Page.IsPostBack && Request.QueryString["q"] != null)
            {
                txtKeyword.Text = Request.QueryString["q"];
            }
            Model = new SearchModel(txtKeyword.Text);

            if (!Page.IsPostBack)
                DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataBind();
        }

        private void DisplayErrors(IEnumerable<string> errorMessages)
        {
            pnlErrors.Visible = true;
            rptErrors.DataSource = errorMessages;
            rptErrors.DataBind();
        }


        #region Filter Item Added/Clicked

        protected void chklstMinistries_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GetFilters();
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }

            DataBind();
        }

        protected void chklstSectors_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GetFilters();
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }

            DataBind();
        }

        protected void chklstStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GetFilters();
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }

            DataBind();
        }

        protected void chklstDatePresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GetFilters();
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }

            DataBind();
        }

        #endregion


        private void GetFilters()
        {
            int i = Model.FilterByMinistries.Count();


            foreach (RepeaterItem item in rptMinistryFilters.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var id = ((Button)item.FindControl("btnRemove")).CommandArgument;
                    var name = ((Literal)item.FindControl("ltrName")).Text;
                    Model.FilterByMinistries.Add(new KeyValuePair<Guid, string>(Guid.Parse(id), name));
                }
            }

            foreach (RepeaterItem item in rptSectorFilters.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var id = ((Button)item.FindControl("btnRemove")).CommandArgument;
                    var name = ((Literal)item.FindControl("ltrName")).Text;
                    Model.FilterBySectors.Add(new KeyValuePair<Guid, string>(Guid.Parse(id), name));
                }
            }

            foreach (ListItem item in chklstMinistries.Items)
                if (item.Selected)
                    Model.FilterByMinistries.Add(new KeyValuePair<Guid, string>(Guid.Parse(item.Value), item.Text));

            foreach (ListItem item in chklstSectors.Items)
                if (item.Selected)
                    Model.FilterBySectors.Add(new KeyValuePair<Guid, string>(Guid.Parse(item.Value), item.Text));


            //foreach (ListItem item in chklstDatePresets.Items)
            //    if (item.Selected)
            //        Model.FilterByDateRange = (DateRangeOptions)Enum.Parse(typeof(DateRangeOptions), item.Value);

            foreach (ListItem item in chklstStatus.Items)
                if (item.Selected)
                    Model.FilterByStatus = (StatusOptions) Enum.Parse(typeof(StatusOptions), item.Value);

            //Model.FilterByStartDate = startDatePicker.SelectedDate;
            //Model.FilterByEndDate = endDatePicker.SelectedDate;

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            GetFilters();
            DataBind();
        }

        #region Remove a filter

        protected void rptMinistryFilters_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            GetFilters();

            Guid selected = new Guid(e.CommandArgument.ToString());
            Model.FilterByMinistries.Remove(Model.FilterByMinistries.Where(m => m.Key == selected).Select(m => m).FirstOrDefault());

            DataBind();
        }

        protected void rptSectorFilters_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            GetFilters();

            Guid selected = new Guid(e.CommandArgument.ToString());
            Model.FilterBySectors.Remove(Model.FilterBySectors.Where(m => m.Key == selected).Select(m => m).FirstOrDefault()); 
            DataBind();
        }

        protected void rptStatuses_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Model.FilterByStatus = null; 

            GetFilters();
            DataBind();
        }

        protected void btnRemoveDateFilter_Click(object sender, EventArgs e)
        {
            Model.FilterByStartDate = null;
            Model.FilterByEndDate = null;
            //startDatePicker.SelectedDate = null;
            //endDatePicker.SelectedDate = null;
            GetFilters();
            DataBind();
        }

        #endregion

        protected string GetDateRangeText(DateRangeOptions? option)
        {
            if (!option.HasValue)
                return string.Empty;

            return option.ToString().Replace("_", " ");
        }

        protected string GetStatusText(StatusOptions? option)
        {
            if (!option.HasValue)
                return string.Empty;

            return option.Value.ToString();
        }

        protected void btnRemoveDateRangeFilter_Click(object sender, EventArgs e)
        {
            Model.FilterByDateRange = null;

            GetFilters();
            DataBind();
        }

        protected void btnRemoveStatus_Click(object sender, EventArgs e)
        {
            Model.FilterByStatus = null;
            GetFilters();
            DataBind();
        }

        protected void listViewDataSource_ObjectCreated(object sender, ObjectDataSourceEventArgs e)
        {
            ((SearchDataSource)e.ObjectInstance).Model = Model;
        }
    }
}