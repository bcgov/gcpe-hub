using System;
using System.Linq;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Data.Entity.SqlServer;
using MediaRelationsSiteApp.DataManagement;

public partial class ManageLanguages : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Register("Language", "Languages", "SortOrder", "AddEditLanguage.aspx", DataListAdminLib.DeleteEntity<Language>);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {

            var langs = from s in ctx.Languages select s;

            if (Master.SortProperty != null)
                langs = LinqDataMethods.OrderBy(langs, Master.SortProperty, Master.SortDescending, false).ThenBy(t => t.LanguageName);

            if (Master.SearchText != null)
                langs = (IOrderedQueryable<Language>)langs.Where(r => SqlFunctions.PatIndex("%" + Master.SearchText + "%", r.LanguageName) > 0);

            var moveLanguageUp = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<Language>("Language", "LanguageId", guid, true); });
            var moveLanguageDown = new ManageUpdateHandler((guid) => { DataListAdminLib.MoveEntity<Language>("Language", "LanguageId", guid, false); });

            Master.RegisterSortOrder(moveLanguageUp, moveLanguageDown, langs.Min(lang => lang.SortOrder), langs.Max(lang => lang.SortOrder));


            Master.RegisterData
            (
                new ManageColumn[]
                { 
                    new ManageColumn() { Text = "Name", PropertyName = "LanguageName" },
                    new ManageColumn() { Text = "Date Modified", PropertyName = "ModifiedDate" },
                    new ManageColumn() { Text = "Date Created", PropertyName = "CreationDate" }
                },

                langs.Skip(Master.PaginatorTopSkip).Take(Master.PaginatorTopPerPage).ToList().Select(language => new ManageRow()
                {
                    Guid = language.Id,
                    Values = new string[] {
                        language.LanguageName,
                        language.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@")),
                        language.CreationDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR.Replace("@", "<br/>@"))
                    },
                    SortOrder = language.SortOrder
                }),

                langs.Count()
            );
        }
    }
}