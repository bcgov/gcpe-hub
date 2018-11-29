using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManageCountries : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Country", "Countries", "SortOrder", "AddEditCountry.aspx", DataListAdminLib.DeleteEntity<Country>);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var countries = (from s in ctx.Countries select s);

            if (Master.SortProperty != null)
                countries = LinqDataMethods.OrderBy(countries, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.CountryName);
            if (Master.SearchText != null)
                countries = (IOrderedQueryable<Country>)countries.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.CountryName) > 0);

            var moveCountryUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<Country>("Country", "CountryId", guid, true); });
            var moveCountryDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<Country>("Country", "CountryId", guid, false); });

            Master.RegisterSortOrder(moveCountryUp, moveCountryDown, countries.Min(country =>  country.SortOrder), countries.Max(country => country.SortOrder));
            
            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "CountryName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                countries.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(country => new ManageRow()
                {
                    Guid = country.Id,
                    Values = new string[] {
                        country.CountryName,
                        country.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        country.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = country.SortOrder
                }),

                countries.Count()
            );
        }
    }
}