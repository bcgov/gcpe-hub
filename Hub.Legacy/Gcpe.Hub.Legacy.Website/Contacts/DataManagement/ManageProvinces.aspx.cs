using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManageProvinces : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Province", "Provinces", "SortOrder", "AddEditProvince.aspx", DataListAdminLib.DeleteEntity<ProvState>);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var provinces = from s in ctx.ProvStates select s;

            if (Master.SortProperty != null)
                provinces = LinqDataMethods.OrderBy(provinces, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.ProvStateName);

            if (Master.SearchText != null)
                provinces = (IOrderedQueryable<ProvState>)provinces.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.ProvStateName) > 0);

            var moveProvStateUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<ProvState>("ProvState", "ProvStateId", guid, true); });
            var moveProvStateDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<ProvState>("ProvState", "ProvStateId", guid, false); });

            Master.RegisterSortOrder(moveProvStateUp, moveProvStateDown, provinces.Min(province => province.SortOrder), provinces.Max(province => province.SortOrder));

            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "ProvStateName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                provinces.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(province => new ManageRow()
                {
                    Guid = province.Id,
                    Values = new string[] {
                        province.ProvStateName,
                        province.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        province.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = province.SortOrder
                }),
                provinces.Count()
            );
        }
    }
}