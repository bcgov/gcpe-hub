using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManageEthnicities : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Ethnicity", "Ethnicities", "SortOrder", "AddEditEthnicity.aspx", DataListAdminLib.DeleteEntity<Ethnicity>);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
     {
            var eths = from s in ctx.Ethnicities select s;
            if (Master.SortProperty != null)
                eths = LinqDataMethods.OrderBy(eths, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.EthnicityName);

            if (Master.SearchText != null)
                eths = (IOrderedQueryable<Ethnicity>)eths.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.EthnicityName) > 0);

            var moveEthnicityUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<Ethnicity>("Ethnicity", "EthnicityId", guid, true); });
            var moveEthnicityDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<Ethnicity>("Ethnicity", "EthnicityId", guid, false); });

            Master.RegisterSortOrder(moveEthnicityUp, moveEthnicityDown, eths.Min(ethnicity => ethnicity.SortOrder), eths.Max(ethnicity => ethnicity.SortOrder));

            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "EthnicityName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                eths.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(ethnicity => new ManageRow()
                {
                    Guid = ethnicity.Id,
                    Values = new string[] {
                        ethnicity.EthnicityName,
                        ethnicity.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        ethnicity.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = ethnicity.SortOrder
                }),
                eths.Count()
            );
        }
    }
}
