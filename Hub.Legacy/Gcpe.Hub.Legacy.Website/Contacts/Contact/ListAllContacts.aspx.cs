using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MediaRelationsDatabase;
using MediaRelationsLibrary;
using System.Text;

public partial class Contact_ListAllContacts : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        using (var ctx = new MediaRelationsEntities())
        {
            var _contacts = (from s in ctx.Contacts orderby (s.FirstName + s.LastName) select s);
            var cnt = _contacts.Count();
            PaginatorTop.Count = cnt;
            PaginatorBottom.Count = cnt;
            var contacts = _contacts.Skip(PaginatorTop.Skip).Take(PaginatorTop.PerPage).ToList();
            StringBuilder sb = new StringBuilder();

            if (cnt == 0)
            {
                sb.Append("<tr><td colspan='2'>No items to display</td></tr>\n");
            }
            else
            {
                foreach (var con in contacts)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + con.FirstName + " " + con.LastName + "</td>");
                    sb.Append("<td><a href=\"" + ResolveUrl("~/Contacts/") + "Contact/ViewContact.aspx?guid=" + con.Id + "\">view</a></td>");
                    sb.Append("</tr>\n");
                }
            }
            ContactLit.Text = sb.ToString();
        }
    }
}