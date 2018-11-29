using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManageMediaDesks : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Media Desk", "Media Desks", "SortOrder", "AddEditMediaDesk.aspx", DataListAdminLib.DeleteEntity<MediaDesk>);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var mediaDesks = from s in ctx.MediaDesks select s;

            if (Master.SortProperty != null)
                mediaDesks = LinqDataMethods.OrderBy(mediaDesks, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.MediaDeskName);

            if (Master.SearchText != null)
                mediaDesks = (IOrderedQueryable<MediaDesk>)mediaDesks.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.MediaDeskName) > 0);

            var moveMediaDeskUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<MediaDesk>("MediaDesk", "MediaDeskId", guid, true); });
            var moveMediaDeskDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<MediaDesk>("MediaDesk", "MediaDeskId", guid, false); });

            Master.RegisterSortOrder(moveMediaDeskUp, moveMediaDeskDown, mediaDesks.Min(mediaDesk => mediaDesk.SortOrder), mediaDesks.Max(mediaDesk => mediaDesk.SortOrder));

            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "MediaDeskName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                mediaDesks.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(mediaDesk => new ManageRow()
                {
                    Guid = mediaDesk.Id,
                    Values = new string[] {
                        mediaDesk.MediaDeskName,
                        mediaDesk.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        mediaDesk.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = mediaDesk.SortOrder
                }),

                mediaDesks.Count()
            );
        }
    }
}