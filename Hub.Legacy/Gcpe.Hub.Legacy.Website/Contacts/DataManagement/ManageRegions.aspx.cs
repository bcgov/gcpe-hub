using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManageRegions : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Region", "Regions", "SortOrder", "AddEditRegion.aspx", DataListAdminLib.DeleteRegion);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var regions = from s in ctx.Regions select s;

            if (Master.SortProperty != null)
                regions = LinqDataMethods.OrderBy(regions, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.RegionName);

            if (Master.SearchText != null)
                regions = (IOrderedQueryable<Region>)regions.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.RegionName) > 0);

            var moveRegionUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<Region>("Region", "RegionId", guid, true); });
            var moveRegionDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<Region>("Region", "RegionId", guid, false); });

            Master.RegisterSortOrder(moveRegionUp, moveRegionDown, regions.Min(region => region.SortOrder), regions.Max(region => region.SortOrder));


            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "RegionName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                regions.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(region => new ManageRow()
                {
                    Guid = region.Id,
                    Values = new string[] {
                        region.RegionName,
                        region.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        region.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = region.SortOrder

                }),

                regions.Count()
            );
        }
    }
}