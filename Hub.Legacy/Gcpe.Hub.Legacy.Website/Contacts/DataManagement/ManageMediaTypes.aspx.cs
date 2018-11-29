using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManageMediaTypes : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Media Type", "Media Types", "SortOrder", "AddEditMediaType.aspx", DataListAdminLib.DeleteEntity<MediaType>);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            var types = (from s in ctx.MediaTypes select s);

            if (Master.SortProperty != null)
                types = LinqDataMethods.OrderBy(types, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.MediaTypeName);

            if (Master.SearchText != null)
                types = (IOrderedQueryable<MediaType>)types.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.MediaTypeName) > 0);

            var moveMediaTypeUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<MediaType>("MediaType", "MediaTypeId", guid, true); });
            var moveMediaTypeDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<MediaType>("MediaType", "MediaTypeId", guid, false); });

            Master.RegisterSortOrder(moveMediaTypeUp, moveMediaTypeDown, types.Min(type => type.SortOrder), types.Max(type => type.SortOrder));
            
            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "MediaTypeName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                types.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(type => new ManageRow()
                {
                    Guid = type.Id,
                    Values = new string[] {
                        type.MediaTypeName,
                        type.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        type.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = type.SortOrder
                }),

                types.Count()
            );
        }
    }
}