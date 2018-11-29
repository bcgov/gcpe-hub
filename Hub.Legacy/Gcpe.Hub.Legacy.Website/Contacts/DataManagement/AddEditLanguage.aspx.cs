using System;
using System.Collections.Generic;
using System.Linq;
using MediaRelationsLibrary;
using MediaRelationsDatabase;

public partial class AddEditLanguage : System.Web.UI.Page
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
        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            if (Guid.TryParse(Request.QueryString["guid"], out guid))
            {
                bool valid = false;
                var language = (from lang in ctx.Languages where lang.Id == guid select lang).FirstOrDefault();

                if (language != null)
                {
                    if (!IsPostBack)
                    {
                        languageNameTb.Text = language.LanguageName;
                        currentName.Text = language.LanguageName;
                        sortOrderTextBox.Text = Convert.ToString(language.SortOrder);
                    }
                    valid = true;
                }

                if (!valid)
                {
                    errorPanel.Visible = true;
                    displayPanel.Visible = false;
                    pageErrorField.Text = "Invalid page request";
                    return;
                }

                editOnlyDisplayPanel.Visible = true;

                if (canDelete) deleteButton.Visible = true;
            }
            else // add new
            {
                if (!IsPostBack)
                {
                    sortOrderTextBox.Text = DataListAdminLib.NextSortOrder<Language>(ctx.Languages).ToString();
                }
            }
        }
    }

    /// <summary>
    /// This method is used to delete a language from the database, this will affect
    /// media outlets
    /// </summary>
    public void DeleteClick(Object sender, EventArgs e)
    {
        if (canDelete)
        {
            KeyValuePair<bool, string> succ = DataListAdminLib.DeleteEntity<Language>(guid);

            if (succ.Key) Response.Redirect("ManageLanguages.aspx?message=" + Server.UrlEncode("Language has been deleted"));
            else Response.Redirect(Request.Url.PathAndQuery + "&message=" + Server.UrlEncode(succ.Value));
        }
    }

    protected void CancelClick(object sender, EventArgs e)
    {
        Response.Redirect("ManageLanguages.aspx");
    }

    protected void SaveClick(object sender, EventArgs e)
    {
        nameError.InnerText = "";
        bool isEdited = guid != Guid.Empty;
        int errors = DataListAdminLib.CreateEditLanguage(languageNameTb.Text.Trim(), sortOrderTextBox.Text, isEdited ? (Guid?)guid : null);

        if (errors > 0)
        {
            if ((errors & 1) != 0) sortOrderError.InnerText = "Invalid Sort Order specified";
            if ((errors & 2) != 0) nameError.InnerText = "Language name cannot be empty";
            else if ((errors & 8) != 0) nameError.InnerText = "Language name is already in use";
            else if ((errors & 1024) != 0) nameError.InnerText = "Language you are editing does not exist";
        }
        else if (errors == 0)
        {
            Response.Redirect("ManageLanguages.aspx?message=" + Server.UrlEncode("Language has been " + (isEdited ? "modified" : "created")));
        }
    }
}
