using System;
using System.Collections.Generic;
using System.Linq;
using MediaRelationsLibrary;
using MediaRelationsDatabase;

public partial class AddEditProvince : System.Web.UI.Page
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
                ProvState province = (from ps in ctx.ProvStates where ps.Id == guid select ps).FirstOrDefault();

                if (province != null)
                {
                    if (!IsPostBack)
                    {
                        provinceNameTb.Text = province.ProvStateName;
                        provinceAbbrevTb.Text = province.ProvStateAbbrev;
                        currentName.Text = province.ProvStateName;
                        sortOrderTextBox.Text = Convert.ToString(province.SortOrder);
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
            KeyValuePair<bool, string> succ = DataListAdminLib.DeleteEntity<ProvState>(guid);

            if (succ.Key) Response.Redirect("ManageProvinces.aspx?message=" + Server.UrlEncode("Province/State has been deleted"));
            else Response.Redirect(Request.Url.PathAndQuery + "&message=" + Server.UrlEncode(succ.Value));
        }
    }

    protected void CancelClick(object sender, EventArgs e)
    {
        Response.Redirect("ManageProvinces.aspx");
    }

    protected void SaveClick(object sender, EventArgs e)
    {
        bool isEdited = guid != Guid.Empty;
        int errors = DataListAdminLib.CreateEditProvState(provinceNameTb.Text.Trim(), provinceAbbrevTb.Text.Trim(), sortOrderTextBox.Text, isEdited ? (Guid?)guid : null);

        if (errors > 0)
        {
            if ((errors & 1) != 0) sortOrderError.InnerText = "Invalid Sort Order specified";
            if ((errors & 2) != 0) nameError.InnerText = "Province/State name cannot be empty";
            else if ((errors & 4) != 0) nameError.InnerText = "Province/State abbreviation cannot be empty";
            else if ((errors & 8) != 0) nameError.InnerText = "Province/State name and/or abbreviation is already in use";
            else if ((errors & 1024) != 0) nameError.InnerText = "Province/State you are editing does not exist";
        }
        else if (errors == 0)
        {
            Response.Redirect("ManageProvinces.aspx?message=" + Server.UrlEncode("Province/State has been " + (isEdited ? "modified" : "created")));
        }
    }
}