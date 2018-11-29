using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManagePublicationDays : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Publication Day", "Publication Days", "SortOrder","AddEditPublicationDay.aspx", DataListAdminLib.DeleteEntity<PublicationDay>);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var publicationDays = from s in ctx.PublicationDays select s;

            if (Master.SortProperty != null)
                publicationDays = LinqDataMethods.OrderBy(publicationDays, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.PublicationDaysName);

            if (Master.SearchText != null)
                publicationDays = (IOrderedQueryable<PublicationDay>)publicationDays.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.PublicationDaysName) > 0);

            var movePublicationDayUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<PublicationDay>("PublicationDays", "PublicationDaysId", guid, true); });
            var movePublicationDayDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<PublicationDay>("PublicationDays", "PublicationDaysId", guid, false); });

            Master.RegisterSortOrder(movePublicationDayUp, movePublicationDayDown, publicationDays.Min(publicationDay => publicationDay.SortOrder), publicationDays.Max(publicationDay => publicationDay.SortOrder));

            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "PublicationDaysName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                publicationDays.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(publicationDay => new ManageRow()
                {
                    Guid = publicationDay.Id,
                    Values = new string[] {
                        publicationDay.PublicationDaysName,
                        publicationDay.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        publicationDay.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = publicationDay.SortOrder
                }),

                publicationDays.Count()
            );
        }
    }
}