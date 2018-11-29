using System;
using System.Collections.Generic;
using System.Linq;
using MediaRelationsLibrary;
using MediaRelationsDatabase;

public partial class AddEditRegion : System.Web.UI.Page
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
                Region region = (from reg in ctx.Regions where reg.Id == guid select reg).FirstOrDefault();

                if (region != null)
                {
                    if (!IsPostBack)
                    {
                        regionNameTb.Text = region.RegionName;
                        currentName.Text = region.RegionName;
                        sortOrderTextBox.Text = Convert.ToString(region.SortOrder);
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

    /// <summary>
    /// This method is used to delete a region from the database, this will affect
    /// media outlets
    /// </summary>
    public void DeleteClick(Object sender, EventArgs e)
    {
        if (canDelete)
        {
            KeyValuePair<bool, string> succ = DataListAdminLib.DeleteRegion(guid);

            if (succ.Key) Response.Redirect("ManageRegions.aspx?message=" + Server.UrlEncode("Region has been deleted"));
            else Response.Redirect(Request.Url.PathAndQuery + "&message=" + Server.UrlEncode(succ.Value));
        }
    }

    protected void CancelClick(object sender, EventArgs e)
    {
        Response.Redirect("ManageRegions.aspx");
    }

    protected void SaveClick(object sender, EventArgs e)
    {
        bool isEdited = guid != Guid.Empty;
        int errors = DataListAdminLib.CreateEditRegion(regionNameTb.Text.Trim(), sortOrderTextBox.Text, isEdited ? (Guid?)guid : null);

        if (errors > 0)
        {
            if ((errors & 1) != 0) sortOrderError.InnerText = "Invalid Sort Order specified";
            if ((errors & 2) != 0) nameError.InnerText = "Region name cannot be empty";
            else if ((errors & 8) != 0) nameError.InnerText = "Region name is already in use";
            else if ((errors & 1024) != 0) nameError.InnerText = "Region you are editing does not exist";
        }
        else if (errors == 0)
        {
            Response.Redirect("ManageRegions.aspx?message=" + Server.UrlEncode("Region has been " + (isEdited ? "modified" : "created")));
        }
    }
}