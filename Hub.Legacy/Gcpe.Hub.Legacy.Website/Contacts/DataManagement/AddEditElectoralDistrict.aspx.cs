using System;
using System.Collections.Generic;
using System.Linq;
using MediaRelationsLibrary;
using MediaRelationsDatabase;

public partial class AddEditElectoralDistrict : System.Web.UI.Page
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
                ElectoralDistrict district = (from ed in ctx.ElectoralDistricts where ed.Id == guid select ed).FirstOrDefault();

                if (district != null)
                {
                    if (!IsPostBack)
                    {
                        electoralDistrictNameTb.Text = district.DistrictName;
                        currentName.Text = district.DistrictName;
                        sortOrderTextBox.Text = Convert.ToString(district.SortOrder);
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
            KeyValuePair<bool, string> succ = DataListAdminLib.DeleteElectoralDistrict(guid);

            if (succ.Key) Response.Redirect("ManageElectoralDistricts.aspx?message=" + Server.UrlEncode("Electoral District has been deleted"));
            else Response.Redirect(Request.Url.PathAndQuery + "&message=" + Server.UrlEncode(succ.Value));
        }
    }

    protected void CancelClick(object sender, EventArgs e)
    {
        Response.Redirect("ManageElectoralDistricts.aspx");
    }

    protected void SaveClick(object sender, EventArgs e)
    {
        bool isEdited = guid != Guid.Empty;
        int errors = DataListAdminLib.CreateEditElectoralDistrict(electoralDistrictNameTb.Text.Trim(), sortOrderTextBox.Text, isEdited ? (Guid?)guid : null);

        if (errors > 0)
        {
            if ((errors & 1) != 0) sortOrderError.InnerText = "Invalid Sort Order specified";
            if ((errors & 2) != 0) nameError.InnerText = "Electoral District name cannot be empty";
            else if ((errors & 8) != 0) nameError.InnerText = "Electoral District name is already in use";
            else if ((errors & 1024) != 0) nameError.InnerText = "Electoral District you are editing does not exist";
        }
        else if (errors == 0)
        {
            Response.Redirect("ManageElectoralDistricts.aspx?message=" + Server.UrlEncode("Electoral District has been " + (isEdited ? "modified" : "created")));
        }
    }
}