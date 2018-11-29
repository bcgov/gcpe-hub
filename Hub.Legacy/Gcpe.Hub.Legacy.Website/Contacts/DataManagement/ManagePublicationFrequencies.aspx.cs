using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManagePublicationFrequencies : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Publication Frequency", "Publication Frequencies", "SortOrder", "AddEditPublicationFrequency.aspx", DataListAdminLib.DeletePublicationFrequency);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var publictionFrequencies = from s in ctx.PublicationFrequencies select s;

            if (Master.SortProperty != null)
                publictionFrequencies = LinqDataMethods.OrderBy(publictionFrequencies, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.PublicationFrequencyName);

            if (Master.SearchText != null)
                publictionFrequencies = (IOrderedQueryable<PublicationFrequency>)publictionFrequencies.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.PublicationFrequencyName) > 0);

            var movePublicationFrequencyUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<PublicationFrequency>("PublicationFrequency", "PublicationFrequencyId", guid, true); });
            var movePublicationFrequencyDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<PublicationFrequency>("PublicationFrequency", "PublicationFrequencyId", guid, false); });

            Master.RegisterSortOrder(movePublicationFrequencyUp, movePublicationFrequencyDown, publictionFrequencies.Min(publictionFrequency => publictionFrequency.SortOrder), publictionFrequencies.Max(publictionFrequency => publictionFrequency.SortOrder));


            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "PublicationFrequencyName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                publictionFrequencies.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(publicationFrequency => new ManageRow()
                {
                    Guid = publicationFrequency.Id,
                    Values = new string[] {
                        publicationFrequency.PublicationFrequencyName,
                        publicationFrequency.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        publicationFrequency.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = publicationFrequency.SortOrder
                }),

                publictionFrequencies.Count()
            );
        }
    }
}
