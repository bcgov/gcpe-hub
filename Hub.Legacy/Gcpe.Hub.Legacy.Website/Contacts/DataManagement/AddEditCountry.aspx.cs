using System;
using System.Collections.Generic;
using System.Linq;
using MediaRelationsLibrary;
using MediaRelationsDatabase;

public partial class AddEditCountry : System.Web.UI.Page
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
                Country country = (from c in ctx.Countries where c.Id == guid select c).FirstOrDefault();
                if (country != null)
                {
                    if (!IsPostBack)
                    {
                        countryNameTb.Text = country.CountryName;
                        countryAbbrevTb.Text = country.CountryAbbrev;
                        currentName.Text = country.CountryName;
                        sortOrderTextBox.Text = Convert.ToString(country.SortOrder);
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
    /// This method is used to delete a country from the database, this will affect
    /// media outlets
    /// </summary>
    public void DeleteClick(Object sender, EventArgs e)
    {
        if (canDelete)
        {
            KeyValuePair<bool, string> succ = DataListAdminLib.DeleteEntity<Country>(guid);

            if (succ.Key) Response.Redirect("ManageCountries.aspx?message=" + Server.UrlEncode("Country has been deleted"));
            else Response.Redirect(Request.Url.PathAndQuery + "&message=" + Server.UrlEncode(succ.Value));
        }
    }

    protected void CancelClick(object sender, EventArgs e)
    {
        Response.Redirect("ManageCountries.aspx");
    }

    protected void SaveClick(object sender, EventArgs e)
    {
        bool isEdited = guid != Guid.Empty;
        int errors = DataListAdminLib.CreateEditCountry(countryNameTb.Text.Trim(), countryAbbrevTb.Text.Trim(), sortOrderTextBox.Text, isEdited ? (Guid?)guid : null);

        if (errors > 0)
        {
            if ((errors & 1) != 0) sortOrderError.InnerText = "Invalid Sort Order specified";
            if ((errors & 2) != 0) nameError.InnerText = "Country name cannot be empty";
            else if ((errors & 4) != 0) nameError.InnerText = "Country abbreviation cannot be empty";
            else if ((errors & 8) != 0) nameError.InnerText = "Country name and/or abbreviation is already in use";
            else if ((errors & 1024) != 0) nameError.InnerText = "Country you are editing does not exist";
        }
        else if (errors == 0)
        {
            Response.Redirect("ManageCountries.aspx?message=" + Server.UrlEncode("Country has been " + (isEdited ? "modified" : "created")));
        }
    }
}