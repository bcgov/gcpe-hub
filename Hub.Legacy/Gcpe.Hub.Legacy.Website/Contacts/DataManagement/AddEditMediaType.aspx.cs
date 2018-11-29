using System;
using System.Collections.Generic;
using System.Linq;
using MediaRelationsLibrary;
using MediaRelationsDatabase;

public partial class AddEditMediaType : System.Web.UI.Page
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
                var mediaType = (from type in ctx.MediaTypes where type.Id == guid select type).FirstOrDefault();
                if (mediaType != null)
                {
                    if (!IsPostBack)
                    {
                        mediaTypeTb.Text = mediaType.MediaTypeName;
                        currentName.Text = mediaType.MediaTypeName;
                        sortOrderTextBox.Text = Convert.ToString(mediaType.SortOrder);
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
            // This method is used to delete a media type from the database, this will affect media outlets
            KeyValuePair<bool, string> succ = DataListAdminLib.DeleteEntity<MediaType>(guid);

            if (succ.Key) Response.Redirect("ManageMediaTypes.aspx?message=" + Server.UrlEncode("Media type has been deleted"));
            else Response.Redirect(Request.Url.PathAndQuery + "&message=" + Server.UrlEncode(succ.Value));
        }
    }

    protected void CancelClick(object sender, EventArgs e)
    {
        Response.Redirect("ManageMediaTypes.aspx");
    }

    protected void SaveClick(object sender, EventArgs e)
    {
        bool isEdited = guid != Guid.Empty;
        int errors = DataListAdminLib.CreateEditMediaType(mediaTypeTb.Text.Trim(), sortOrderTextBox.Text, isEdited ? (Guid?)guid : null);

        if (errors > 0)
        {
            if ((errors & 1) != 0) sortOrderError.InnerText = "Invalid Sort Order specified";
            if ((errors & 2) != 0) nameError.InnerText = "Media type name cannot be empty";
            else if ((errors & 4) != 0) nameError.InnerText = "Media type abbreviation cannot be empty";
            else if ((errors & 8) != 0) nameError.InnerText = "Media type name and/or abbreviation is already in use";
            else if ((errors & 1024) != 0) nameError.InnerText = "Media type you are editing does not exist";
        }
        else if (errors == 0)
        {
            Response.Redirect("ManageMediaTypes.aspx?message=" + Server.UrlEncode("Media type has been " + (isEdited ? "modified" : "created")));
        }
    }
}