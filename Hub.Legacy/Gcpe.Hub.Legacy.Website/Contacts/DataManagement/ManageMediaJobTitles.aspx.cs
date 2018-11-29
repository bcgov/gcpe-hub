using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManageMediaJobTitles : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Job Title", "Job Titles", "SortOrder", "AddEditMediaJobTitle.aspx", DataListAdminLib.DeleteMediaJobTitle);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var mediaJobTitles = (from s in ctx.MediaJobTitles select s);
        
            if (Master.SortProperty != null)
                mediaJobTitles = LinqDataMethods.OrderBy(mediaJobTitles, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.MediaJobTitleName);

            if (Master.SearchText != null)
                mediaJobTitles = (IOrderedQueryable<MediaJobTitle>)mediaJobTitles.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.MediaJobTitleName) > 0);

            var moveMediaJobTitleUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<MediaJobTitle>("MediaJobTitle", "MediaJobTitleId", guid, true); });
            var moveMediaJobTitleDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<MediaJobTitle>("MediaJobTitle", "MediaJobTitleId", guid, false); });

            Master.RegisterSortOrder(moveMediaJobTitleUp, moveMediaJobTitleDown, mediaJobTitles.Min(mediaJobTitle => mediaJobTitle.SortOrder), mediaJobTitles.Max(mediaJobTitle => mediaJobTitle.SortOrder));
            Master.RegisterData
            (
                new ManageColumn[] { 
                    new ManageColumn() { Text = "Name", PropertyName = "MediaJobTitleName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                mediaJobTitles.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(mediaJobTitle => new ManageRow()
                {
                    Guid = mediaJobTitle.Id,
                    Values = new string[] {
                        mediaJobTitle.MediaJobTitleName,
                        mediaJobTitle.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        mediaJobTitle.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = mediaJobTitle.SortOrder
                }),

                mediaJobTitles.Count()
            );
        }
    }
}