using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using CorporateCalendar.Data;

namespace CorporateCalendarAdmin
{
    public partial class UserList : System.Web.UI.Page
    {
        /// <summary>
        /// Load the list of users, sorting it by ministry name and then by user full name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            var corporateCalendarDataContext = new CorporateCalendar.Data.CorporateCalendarDataContext();

            var activeSystemUserMinistries = corporateCalendarDataContext.SystemUserMinistries.Where(um => um.IsActive && um.Ministry.IsActive && um.SystemUser.IsActive)
                .Select(c => new { c.SystemUserId, c.SystemUser.FullName, c.Ministry.Abbreviation }).OrderBy(c => c.Abbreviation).ThenBy(c=> c.FullName);

            foreach (var activeSystemUserMinistry in activeSystemUserMinistries)
            {
                var table = new HtmlTable();
                var row = new HtmlTableRow();
                var cell = new HtmlTableCell();

                CommunicationContact activeCommunicationContact =
                    corporateCalendarDataContext.CommunicationContacts.FirstOrDefault(y => y.IsActive == true && y.SystemUserId.Equals(activeSystemUserMinistry.SystemUserId));

                cell.InnerHtml = string.Format("<a href=\"User.aspx?SystemUserId={0}\" target=\"_self\">{1} ({2}) ({3})</a> ",
                  activeSystemUserMinistry.SystemUserId, 
                  activeSystemUserMinistry.FullName,
                  activeSystemUserMinistry.Abbreviation, activeCommunicationContact != null ? Convert.ToString(activeCommunicationContact.SortOrder) : string.Empty);
                row.Cells.Add(cell);
                table.Rows.Add(row);
                SystemUserTablePlaceHolder.Controls.Add(table);
            }

        }
    }
}