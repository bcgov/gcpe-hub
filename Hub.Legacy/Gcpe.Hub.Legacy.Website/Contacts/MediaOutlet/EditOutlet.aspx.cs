using System;

public partial class EditOutlet : System.Web.UI.Page
{

    protected void Page_Init(object sender, EventArgs e)
    {
        Guid guid;
        Guid.TryParse(Request.QueryString["guid"], out guid);
        if (guid == Guid.Empty)
        {
            editOutletControl.Visible = false;
            ErrorLit.Text = "You have not specified an outlet to edit";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}