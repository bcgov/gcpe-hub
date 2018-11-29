using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManageMinisterialJobTitles : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Ministerial Job Title", "Ministerial Job Titles", "SortOrder", "AddEditMinisterialJobTitle.aspx", DataListAdminLib.DeleteMinisterialJobTitle);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var ministerialJobTitles = from s in ctx.MinisterialJobTitles select s;

            if (Master.SortProperty != null)
                ministerialJobTitles = LinqDataMethods.OrderBy(ministerialJobTitles, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.MinisterialJobTitleName);

            if (Master.SearchText != null)
                ministerialJobTitles = (IOrderedQueryable<MinisterialJobTitle>)ministerialJobTitles.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.MinisterialJobTitleName) > 0);

            var moveministerialJobTitleUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<MinisterialJobTitle>("MinisterialJobTitle", "MinisterialJobTitleId", guid, true); });
            var moveministerialJobTitleDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<MinisterialJobTitle>("MinisterialJobTitle", "MinisterialJobTitleId", guid, false); });

            Master.RegisterSortOrder(moveministerialJobTitleUp, moveministerialJobTitleDown, ministerialJobTitles.Min(ministerialJobTitle => ministerialJobTitle.SortOrder), ministerialJobTitles.Max(ministerialJobTitle => ministerialJobTitle.SortOrder));

            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "MinisterialJobTitleName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                ministerialJobTitles.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(ministerialJobTitle => new ManageRow()
                {
                    Guid = ministerialJobTitle.Id,
                    Values = new string[] {
                        ministerialJobTitle.MinisterialJobTitleName,
                        ministerialJobTitle.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        ministerialJobTitle.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = ministerialJobTitle.SortOrder
                }),

                ministerialJobTitles.Count()
            );
        }
    }
}