using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManageBeats : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Beat", "Beats", "SortOrder", "AddEditBeat.aspx", DataListAdminLib.DeleteBeat);
      
        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var beats = from s in ctx.Beats select s;

            if (Master.SortProperty != null)
                beats = LinqDataMethods.OrderBy(beats, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.BeatName);

            if (Master.SearchText != null)
                beats = (IOrderedQueryable<Beat>)beats.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%",  r.BeatName) > 0);

            var moveBeatUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<Beat>("Beat", "BeatId", guid, true); });
            var moveBeatDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<Beat>("Beat", "BeatId", guid, false); });

            Master.RegisterSortOrder(moveBeatUp, moveBeatDown, beats.Min(beat => beat.SortOrder), beats.Max(beat => beat.SortOrder));

            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "BeatName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                beats.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(beat => new ManageRow()
                {
                    Guid = beat.Id,
                    Values = new string[] {
                        beat.BeatName,
                        beat.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        beat.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = beat.SortOrder
                }),
              
                beats.Count()
            );
        }
    }
}

