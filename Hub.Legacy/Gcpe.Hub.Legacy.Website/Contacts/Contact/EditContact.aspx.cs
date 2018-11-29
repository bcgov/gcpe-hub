using System;
using MediaRelationsLibrary;

public partial class EditContact : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ContactAdminLib lib = new ContactAdminLib();
        Guid guid;
        Guid.TryParse(Request.QueryString["guid"], out guid);

        if (guid == Guid.Empty)
        {
            ErrorLit.Text = "You have not specified a contact to edit";
            AddEditContactControl.Visible = false;
        }
        else if (!lib.ContactExists(guid))
        {
            ErrorLit.Text = "The specified contact does not exist";
            AddEditContactControl.Visible = false;
        }
    }
}