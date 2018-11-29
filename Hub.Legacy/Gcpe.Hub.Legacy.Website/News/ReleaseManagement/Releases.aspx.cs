extern alias legacy;

using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Gcpe.Hub.News.ReleaseManagement
{
    using legacy::Gcpe.Hub.Data.Entity;

    public partial class Releases : Hub.News.Page
    {
        public ReleasesModel Model;

        public string Folder { get; set; }
        public string Type { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            Folder = (string)Page.RouteData.Values["Folder"];
            Type = (string)Page.RouteData.Values["Type"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            Model = new ReleasesModel();
            listViewDataSource.ObjectCreated += listViewDataSource_ObjectCreated;
            CalendarlistViewDataSource.ObjectCreated += listViewDataSource_ObjectCreated;
            DateTimeOffset tomorrow = DateTimeOffset.Now.Date.AddDays(1d);


            if (Folder == "Drafts")
            {
                if (string.IsNullOrWhiteSpace(Type) || Type == "All")
                {
                    Model.ResultSetName = "Drafts";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && !nr.IsCommitted && !nr.IsPublished orderby nr.PublishDateTime < tomorrow ? 0 : (nr.PublishDateTime.HasValue ? 2 : 1), nr.PublishDateTime select nr;
                }
                else if (Type == "Story")
                {
                    Model.ResultSetName = "Story Drafts";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && !nr.IsCommitted && !nr.IsPublished && nr.ReleaseType == ReleaseType.Story orderby nr.PublishDateTime < tomorrow ? 0 : (nr.PublishDateTime.HasValue ? 2 : 1), nr.PublishDateTime select nr;
                }
                else if (Type == "Release")
                {
                    Model.ResultSetName = "Release Drafts";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && !nr.IsCommitted && !nr.IsPublished && nr.ReleaseType == ReleaseType.Release orderby nr.PublishDateTime < tomorrow ? 0 : (nr.PublishDateTime.HasValue ? 2 : 1), nr.PublishDateTime select nr;
                }
                else if (Type == "Factsheet")
                {
                    Model.ResultSetName = "Factsheet Drafts";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && !nr.IsCommitted && !nr.IsPublished && nr.ReleaseType == ReleaseType.Factsheet orderby nr.PublishDateTime < tomorrow ? 0 : (nr.PublishDateTime.HasValue ? 2 : 1), nr.PublishDateTime select nr;
                }
                else if (Type == "Update")
                {
                    Model.ResultSetName = "Update Drafts";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && !nr.IsCommitted && !nr.IsPublished && nr.ReleaseType == ReleaseType.Update orderby nr.PublishDateTime < tomorrow ? 0 : (nr.PublishDateTime.HasValue ? 2 : 1), nr.PublishDateTime select nr;
                }
                else if (Type == "Advisory")
                {
                    Model.ResultSetName = "Advisory Drafts";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && !nr.IsCommitted && !nr.IsPublished && nr.ReleaseType == ReleaseType.Advisory orderby nr.PublishDateTime < tomorrow ? 0 : (nr.PublishDateTime.HasValue ? 2 : 1), nr.PublishDateTime select nr;
                }
            }
            else if (Folder == "Scheduled")
            {
                if (string.IsNullOrWhiteSpace(Type) || Type == "All")
                {
                    Model.ResultSetName = "Scheduled";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && nr.IsCommitted && !nr.IsPublished orderby nr.PublishDateTime select nr;
                }
                else if (Type == "Story")
                {
                    Model.ResultSetName = "Scheduled Stories";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && nr.IsCommitted && !nr.IsPublished && nr.ReleaseType == ReleaseType.Story orderby nr.PublishDateTime select nr;
                }
                else if (Type == "Release")
                {
                    Model.ResultSetName = "Scheduled Releases";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && nr.IsCommitted && !nr.IsPublished && nr.ReleaseType == ReleaseType.Release orderby nr.PublishDateTime select nr;
                }
                else if (Type == "Factsheet")
                {
                    Model.ResultSetName = "Scheduled Factsheets";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && nr.IsCommitted && !nr.IsPublished && nr.ReleaseType == ReleaseType.Factsheet orderby nr.PublishDateTime select nr;
                }
                else if (Type == "Update")
                {
                    Model.ResultSetName = "Scheduled Updates";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && nr.IsCommitted && !nr.IsPublished && nr.ReleaseType == ReleaseType.Update orderby nr.PublishDateTime select nr;
                }
                else if (Type == "Advisory")
                {
                    Model.ResultSetName = "Scheduled Advisories";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && nr.IsCommitted && !nr.IsPublished && nr.ReleaseType == ReleaseType.Advisory orderby nr.PublishDateTime select nr;
                }
            }
            else if (Folder == "Published")
            {
                if (string.IsNullOrWhiteSpace(Type) || Type == "All")
                {
                    Model.ResultSetName = "Published";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && nr.IsPublished orderby nr.PublishDateTime descending select nr;
                }
                else if (Type == "Story")
                {
                    Model.ResultSetName = "Published Stories";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && nr.IsPublished && nr.ReleaseType == ReleaseType.Story orderby nr.PublishDateTime descending select nr;
                }
                else if (Type == "Release")
                {
                    Model.ResultSetName = "Published Releases";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && nr.IsPublished && nr.ReleaseType == ReleaseType.Release orderby nr.PublishDateTime descending select nr;
                }
                else if (Type == "Factsheet")
                {
                    Model.ResultSetName = "Published Factsheets";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && nr.IsPublished && nr.ReleaseType == ReleaseType.Factsheet orderby nr.PublishDateTime descending select nr;
                }
                else if (Type == "Update")
                {
                    Model.ResultSetName = "Published Updates";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && nr.IsPublished && nr.ReleaseType == ReleaseType.Update orderby nr.PublishDateTime descending select nr;
                }
                else if (Type == "Advisory")
                {
                    Model.ResultSetName = "Published Advisories";
                    Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && nr.IsPublished && nr.ReleaseType == ReleaseType.Advisory orderby nr.PublishDateTime descending select nr;
                }
            }
            else if (Folder == "Forecast")
            {
                Model.ResultSetName = "Forecast";
                Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && nr.PublishDateTime >= DateTimeOffset.Now
                                  orderby nr.PublishDateTime descending select nr;

                //TODO: Discuss code change below with Shane
                //var todayTimeWithProperTimeStamp = DateTime.Now.ToLocalTime();
                //Model.ResultSet = from nr in DbContext.NewsReleases where nr.IsActive && nr.PublishDateTime != null && DateTimeOffset.Compare(nr.PublishDateTime.Value, todayTimeWithProperTimeStamp) > 0 orderby nr.PublishDateTime descending select nr;
            }
            else
            {
                throw new HttpException(404, null);
            }

            Page.Title = Model.ResultSetName;

            if (!IsPostBack)
            {
                DataBind();

                //if (Folder == "Drafts" || Folder == "Scheduled")
                //{
                //    if (listView.FindControl("dataPager") != null)
                //        ((DataPager)listView.FindControl("dataPager")).PageSize = Int32.MaxValue;
                //}
            }
        }

        protected void DataPager_Init(object sender, EventArgs e)
        {
            if (Folder == "Drafts" || Folder == "Scheduled")
                ((DataPager)listView.FindControl("dataPager")).PageSize = Int32.MaxValue;
        }


        void listViewDataSource_ObjectCreated(object sender, ObjectDataSourceEventArgs e)
        {
            ((ReleasesDataSource)e.ObjectInstance).Model = Model;
        }

        public string GetWebCalUrl()
        {
            string url = Request.Url.ToString().Replace("http", "webcal");
            int posLastSlash = url.LastIndexOf('/');
            return url.Substring(0, posLastSlash + 1) + "Forecast.ashx";
        }

        public string GetCalActivityUrl(int? activityId)
        {
            return "~/Calendar/Activity.aspx?ActivityId=" + (activityId ?? 0);
        }

        public string PublishStatusDate(Gcpe.Hub.News.ReleaseManagement.ReleasesModel.Result item)
        {
            if (item.PublishDateTime.HasValue)
            {
                if (!item.IsCalActivity)
                {
                    return ReleaseModel.FormatPublishStatusDate(item.PublishDateTime.Value, item.IsPublished, item.IsCommitted);
                }
                return (item.IsConfirmed == true ? "Planned for " : "Forecast for ") + ReleaseModel.FormatDateTime(item.PublishDateTime.Value);
            }
            else
            {
                return ""; // "No publish date";
            }

            //Item.PublishedDate.HasValue ? (Item.PublishedDate.Value.ToString("MMMM d, yyyy h:mm ") + Item.PublishedDate.Value.ToString("tt").ToLower()) : "No publish date "
        }
    }
    public class DataPlaceHolder : PlaceHolder
    {
        protected override void DataBindChildren()
        {
            if (Visible)
            {
                base.DataBindChildren();
            }
        }
    }

}