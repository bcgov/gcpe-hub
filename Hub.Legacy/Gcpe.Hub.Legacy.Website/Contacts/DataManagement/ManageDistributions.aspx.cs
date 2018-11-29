using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManageDistribution : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Distribution", "Distributions", "SortOrder", "AddEditDistribution.aspx", DataListAdminLib.DeleteEntity<Distribution>);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var distributions = from s in ctx.Distributions select s;

            if (Master.SortProperty != null)
                distributions = LinqDataMethods.OrderBy(distributions, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.DistributionName);

            if (Master.SearchText != null)
                distributions = (IOrderedQueryable<Distribution>)distributions.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.DistributionName) > 0);

            var moveDistributionUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<Beat>("Distribution", "DistributionId", guid, true); });
            var moveDistributionDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<Beat>("Distribution", "DistributionId", guid, false); });

            Master.RegisterSortOrder(moveDistributionUp, moveDistributionDown, distributions.Min(distribution => distribution.SortOrder), distributions.Max(distribution => distribution.SortOrder));

            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "DistributionName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                distributions.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(distribution => new ManageRow()
                {
                    Guid = distribution.Id,
                    Values = new string[] {
                        distribution.DistributionName,
                        distribution.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        distribution.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = distribution.SortOrder
                }),

                distributions.Count()
            );
        }
    }
}
