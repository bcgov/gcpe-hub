using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManagePrintCategories : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Print Category", "Print Categories", "SortOrder", "AddEditPrintCategory.aspx", DataListAdminLib.DeleteEntity<PrintCategory>);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var printCategories = from s in ctx.PrintCategories select s;

            if (Master.SortProperty != null)
                printCategories = LinqDataMethods.OrderBy(printCategories, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.PrintCategoryName);

            if (Master.SearchText != null)
                printCategories = (IOrderedQueryable<PrintCategory>)printCategories.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.PrintCategoryName) > 0);

            var movePrintCategoryUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<PrintCategory>("PrintCategory", "PrintCategoryId", guid, true); });
            var movePrintCategoryDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<PrintCategory>("PrintCategory", "PrintCategoryId", guid, false); });

            Master.RegisterSortOrder(movePrintCategoryUp, movePrintCategoryDown, printCategories.Min(printCategory => printCategory.SortOrder), printCategories.Max(printCategory => printCategory.SortOrder));

            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "PringCategoryName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                printCategories.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(printCategory => new ManageRow()
                {
                    Guid = printCategory.Id,
                    Values = new string[] {
                        printCategory.PrintCategoryName,
                        printCategory.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        printCategory.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = printCategory.SortOrder
                }),

                printCategories.Count()
            );
        }
    }
}