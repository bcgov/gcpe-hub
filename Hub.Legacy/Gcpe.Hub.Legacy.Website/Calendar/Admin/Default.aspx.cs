using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.DynamicData;

namespace CorporateCalendarAdmin
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var visibleTables = Gcpe.Hub.Global.DefaultModel.VisibleTables;

            if (visibleTables.Count == 0)
            {
                throw new InvalidOperationException("There are no accessible tables. Make sure that at least one data model is registered in Global.asax and scaffolding is enabled or implement custom pages.");
            }

            Menu1.DataSource = visibleTables.Where(t => t.Scaffold);
            Menu1.DataBind();
        }
    }
}