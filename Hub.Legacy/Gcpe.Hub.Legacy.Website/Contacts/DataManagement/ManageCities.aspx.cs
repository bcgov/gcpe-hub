using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManageCities : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("City", "Cities", "SortOrder", "AddEditCity.aspx", DataListAdminLib.DeleteEntity<City>);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var cities = from s in ctx.Cities select s;

            if (Master.SortProperty != null)
                cities = LinqDataMethods.OrderBy(cities, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.CityName);

            if (Master.SearchText != null)
                cities = (IOrderedQueryable<City>)cities.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.CityName) > 0);

            
            var moveCityUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<City>("City", "CityId", guid, true); });
            var moveCityDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<City>("City", "CityId", guid, false); });

            Master.RegisterSortOrder(moveCityUp, moveCityDown, cities.Min(city => city.SortOrder), cities.Max(city => city.SortOrder));

            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "CityName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                cities.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(city => new ManageRow()
                {
                    Guid = city.Id,
                    Values = new string[] {
                        city.CityName,
                        city.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        city.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = city.SortOrder
                }),

                cities.Count()
            );
        }
    }
}