using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.Data.Entity.SqlServer;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using MediaRelationsSiteApp.DataManagement;

public partial class ManageElectoralDistricts : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Electoral District", "Electoral Districts", "SortOrder", "AddEditElectoralDistrict.aspx", DataListAdminLib.DeleteElectoralDistrict);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var districts = from s in ctx.ElectoralDistricts select s;

            if (Master.SortProperty != null)
                districts = LinqDataMethods.OrderBy(districts, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.DistrictName);

            if (Master.SearchText != null)
                districts = (IOrderedQueryable<ElectoralDistrict>)districts.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.DistrictName) > 0);

            var moveElectoralDistrictUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<ElectoralDistrict>("Beat", "DistrictId", guid, true); });
            var moveElectoralDistrictDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<ElectoralDistrict>("Beat", "DistrictId", guid, false); });

            Master.RegisterSortOrder(moveElectoralDistrictUp, moveElectoralDistrictDown, districts.Min(district => district.SortOrder), districts.Max(district => district.SortOrder));

            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "DistrictName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                districts.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(district => new ManageRow()
                {
                    Guid = district.Id,
                    Values = new string[] {
                        district.DistrictName,
                        district.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        district.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = district.SortOrder
                }),

                districts.Count()
            );
        }
    }
}