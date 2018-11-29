using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManageSpecialtyPublications : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Specialty Publication", "Specialty Publications", "SortOrder", "AddEditSpecialtyPublication.aspx", DataListAdminLib.DeleteEntity<SpecialtyPublication>);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var specialtyPublications = from s in ctx.SpecialtyPublications select s;

            if (Master.SortProperty != null)
                specialtyPublications = LinqDataMethods.OrderBy(specialtyPublications, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.SpecialtyPublicationName);

            if (Master.SearchText != null)
                specialtyPublications = (IOrderedQueryable<SpecialtyPublication>)specialtyPublications.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.SpecialtyPublicationName) > 0);

            var moveSpecialtyPublicationUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<Beat>("Beat", "BeatId", guid, true); });
            var moveSpecialtyPublicationDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<Beat>("Beat", "BeatId", guid, false); });

            Master.RegisterSortOrder(moveSpecialtyPublicationUp, moveSpecialtyPublicationDown, specialtyPublications.Min(specialtyPublication => specialtyPublication.SortOrder), specialtyPublications.Max(specialtyPublication => specialtyPublication.SortOrder));
            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "MediaDeskName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                specialtyPublications.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(specialtyPublication => new ManageRow()
                {
                    Guid = specialtyPublication.Id,
                    Values = new string[] {
                        specialtyPublication.SpecialtyPublicationName,
                        specialtyPublication.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        specialtyPublication.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = specialtyPublication.SortOrder
                }),

                specialtyPublications.Count()
            );
        }
    }
}