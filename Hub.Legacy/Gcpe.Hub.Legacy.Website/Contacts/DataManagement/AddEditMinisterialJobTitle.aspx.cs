using System;
using System.Collections.Generic;
using System.Linq;
using MediaRelationsLibrary;
using MediaRelationsDatabase;

public partial class AddEditMinisterialJobTitle : System.Web.UI.Page
{
    Guid guid = Guid.Empty;

    bool canDelete = false;

    protected void Page_Init(object sender, EventArgs e)
    {
        CommonEventLogging logger = new CommonEventLogging();

        canDelete = ((Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists, logger) & Permissions.SiteAction.Delete) != 0);

        if (!canDelete) deleteButton.Visible = false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Guid.TryParse(Request.QueryString["guid"], out guid))
        {
            bool valid = false;
            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                MinisterialJobTitle ministerialJobTitle = (from jt in ctx.MinisterialJobTitles where jt.Id == guid select jt).FirstOrDefault();

                if (ministerialJobTitle != null)
                {
                    if (!IsPostBack)
                    {
                        ministerialJobTitleNameTb.Text = ministerialJobTitle.MinisterialJobTitleName;
                        currentName.Text = ministerialJobTitle.MinisterialJobTitleName;
                        sortOrderTextBox.Text = Convert.ToString(ministerialJobTitle.SortOrder);
                    }
                    valid = true;
                }
            }

            if (!valid)
            {
                errorPanel.Visible = true;
                displayPanel.Visible = false;
                pageErrorField.Text = "Invalid page request";
                return;
            }

            editOnlyDisplayPanel.Visible = true;
            deleteButton.Visible = true;
        }
    }

    public void DeleteClick(Object sender, EventArgs e)
    {
        if (canDelete)
        {
            KeyValuePair<bool, string> succ = DataListAdminLib.DeleteMinisterialJobTitle(guid);

            if (succ.Key) Response.Redirect("ManageMinisterialJobTitles.aspx?message=" + Server.UrlEncode("MinisterialJobTitle has been deleted"));
            else Response.Redirect(Request.Url.PathAndQuery + "&message=" + Server.UrlEncode(succ.Value));
        }
    }

    protected void CancelClick(object sender, EventArgs e)
    {
        Response.Redirect("ManageMinisterialJobTitles.aspx");
    }

    protected void SaveClick(object sender, EventArgs e)
    {
        bool isEdited = guid != Guid.Empty;
        int errors = DataListAdminLib.CreateEditMinisterialJobTitle(ministerialJobTitleNameTb.Text.Trim(), sortOrderTextBox.Text, isEdited ? (Guid?)guid : null);

        if (errors > 0)
        {
            if ((errors & 1) != 0) sortOrderError.InnerText = "Invalid Sort Order specified";
            if ((errors & 2) != 0) nameError.InnerText = "MinisterialJobTitle name cannot be empty";
            else if ((errors & 8) != 0) nameError.InnerText = "MinisterialJobTitle name is already in use";
            else if ((errors & 1024) != 0) nameError.InnerText = "MinisterialJobTitle you are editing does not exist";
        }
        else if (errors == 0)
        {
            Response.Redirect("ManageMinisterialJobTitles.aspx?message=" + Server.UrlEncode("MinisterialJobTitle has been " + (isEdited ? "modified" : "created")));
        }
    }
}