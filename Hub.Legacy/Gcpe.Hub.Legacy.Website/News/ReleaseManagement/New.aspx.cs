extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gcpe.Hub.News.ReleaseManagement
{
    using legacy::Gcpe.Hub.Data.Entity;
    using Gcpe.News.ReleaseManagement.Controls;
    using Templates = Gcpe.News.ReleaseManagement.Templates;

    public partial class New : Hub.News.Page
    {
        public NewModel Model;

        protected void Page_Load(object sender, EventArgs e)
        {
            ((Hub.News.Site)Master.Master).HelpPage = "News Release Management - Create a Release";
            Model = new NewModel();

            if (!IsPostBack)
            {
                PopulateMediaDistributionListPicker();
                // btnSave.UseSubmitBehavior = true;

                contentCKEditor.InitNewsReleaseEditor();
                publishDateTimePicker.Attributes["onBlur"] = "DateTimePicker_Validate(this)";
            }
        }

        public void PopulateMediaDistributionListPicker() {

            mediaDistributionListBox.DataSource = Model.MediaDistributionLists;
            mediaDistributionListBox.DataTextField = "Text";
            mediaDistributionListBox.DataValueField = "Value";
            mediaDistributionListBox.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            // Make sure we apply any "selected' items. 
            // ...I think I'm gonna be sick.
            if (!Page.IsPostBack)
            {
                foreach (var item in Model.MediaDistributionLists)
                {
                    if (item.Selected)
                    {
                        for (int i = 0; i < mediaDistributionListBox.Items.Count; i++)
                        {
                            if (item.Value.ToString() == mediaDistributionListBox.Items[i].Value)
                            {
                                mediaDistributionListBox.Items[i].Selected = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
        #region Datasources for Dropdownlist Radiobuttonlist and Checkboxlist

        public Dictionary<PageLayout, string> PageLayouts
        {
            get
            {
                IEnumerable<PageLayout> pageLayouts = Enum.GetValues(typeof(PageLayout)).Cast<PageLayout>();
                return pageLayouts.ToDictionary(pl => pl, pl => pl.ToString());
            }
        }

        public Dictionary<string, int> ReleaseDocumentTypes
        {
            get
            {
                var releaseTypes =
                from nrt in DbContext.NewsReleaseTypes
                join nrdl in DbContext.NewsReleaseDocumentLanguages on nrt.PageTitle equals nrdl.PageTitle into g
                from nrdlg in g.DefaultIfEmpty()
                where nrt.LanguageId == 4105
                group nrt.ReleaseType by nrt.PageTitle into r
                orderby r.Count() descending
                select new { PageTitle = r.Key, ReleaseType = r.Distinct().FirstOrDefault() };

                return new Dictionary<string, int>(releaseTypes.ToDictionary(rt => rt.PageTitle, rt => rt.ReleaseType));
            }
        }

        public IEnumerable<string> EnglishLocations
        {
            get
            {
                var locations = from nrl in DbContext.NewsReleaseLanguages
                                where nrl.LanguageId == 4105 && nrl.Release.IsActive && nrl.Location != ""
                                group nrl by nrl.Location into g
                                select new { Location = g.Key, Count = g.Count() };
                return locations.OrderByDescending(e => e.Count).Select(e => e.Location);
            }
        }
        #endregion

        #region Default Value Lookups

        public Dictionary<string, Guid> DefaultPageImages
        {
            get
            {
                Dictionary<string, Guid> defaultLayouts = new Dictionary<string, Guid>();

                var types = from nrt in DbContext.NewsReleaseTypes
                            where nrt.LanguageId == 4105 && nrt.PageImageId.HasValue
                            select nrt;

                return types.ToDictionary(e => e.PageTitle, e => e.PageImageId.Value);
            }
        }

        public Dictionary<Guid, IEnumerable<Guid>> DefaultMinistrySectors
        {
            get
            {
                return DbContext.Ministries.ToDictionary(m => m.Id, m => m.Sectors.Select(s => s.Id));
            }
        }

        #endregion

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (txtCorpCalId.Text != "")
                Model.ActivityId = int.Parse(txtCorpCalId.Text);

            Model.ReleaseType = (ReleaseType)int.Parse(rblNewsType.SelectedValue);

            Model.PageTitle = txtPageTitle.Text.Trim();

            if (rblPageLayout.SelectedItem != null)
                Model.PageLayout = (PageLayout)Enum.Parse(typeof(PageLayout), rblPageLayout.SelectedItem.Value);

            // Don't allow choosing an image on Advisories
            bool allowImagePickerSelection = Model.ReleaseType != ReleaseType.Advisory;
            if (allowImagePickerSelection)
            {
                Model.PageImageId = pageImagePicker.Value;
            }

            foreach (ListItem item in chklstMinistries.Items)
                Model.Ministries.Single(m => m.Value == Guid.Parse(item.Value)).Selected = item.Selected;

            bool allowSectorsThemesServices = Model.ReleaseType != ReleaseType.Advisory;
            if (allowSectorsThemesServices)
            {
                foreach (ListItem item in cklstSector.Items)
                    Model.Sectors.Single(s => s.Value == Guid.Parse(item.Value)).Selected = item.Selected;

                foreach (ListItem item in cklstTheme.Items)
                    Model.Themes.Single(s => s.Value == Guid.Parse(item.Value)).Selected = item.Selected;

                foreach (ListItem item in cklstTag.Items)
                    Model.Tags.Single(s => s.Value == Guid.Parse(item.Value)).Selected = item.Selected;
            }

            // Allow Media Distribution of everything BUT  Stories & Updates
            bool allowMediaDistribution = !(Model.ReleaseType == ReleaseType.Story || Model.ReleaseType == ReleaseType.Update);
            if (allowMediaDistribution)
            {
                foreach (ListItem item in mediaDistributionListBox.Items)
                    Model.MediaDistributionLists.Single(s => s.Value == Guid.Parse(item.Value)).Selected = item.Selected;
            }


            Model.Organizations = Model.PageLayout == PageLayout.Formal ? txtOrganization.Text.Trim() : "";

            Model.Location = txtLocation.Text.Trim();

            //TODO: Set Provincewide
            //Model.IsProvinceWide = false;

            Model.Headline = txtHeadline.Text.Trim();

            Model.Subheadline = txtSubheadline.Text.Trim();

            Model.Byline = Model.PageLayout == PageLayout.Informal ? txtByline.Text.Trim() : "";

            //Model.BodyHtml = HtmlTagCleaner.StripDisallowedTags(contentCKEditor.Text);
            //Model.BodyHtml = contentCKEditor.Text;

            List<string> contacts = new List<string>();
            foreach (string key in Request.Form.AllKeys)
            {
                if(key != null)
                    if (key.StartsWith("parameters") && !string.IsNullOrWhiteSpace(Request.Form[key]))
                        contacts.Add(Request.Form[key].Trim());
            }

            Model.Contacts = contacts;

            //TODO: Convert from Pacific Time to UTC
            DateTime publishDate;
            if (DateTime.TryParse(publishDateTimePicker.Text, out publishDate))
                Model.PublishDateTime = publishDate;
            else
                Model.PublishDateTime = (DateTime?)null;
      
            try
            {
                if(!ReleaseManagementModel.ValidateUrls(contentCKEditor.Text))
                    throw new HubModelException(new string[] { "The body content contains links in invalid format. Please use http:// or https:// valid format." });

                Model.BodyHtml = AssetEmbedManager.NormalizeAssetTags(contentCKEditor.Text);
                
                try
                {
                    string text = Gcpe.News.ReleaseManagement.Templates.Convert.HtmlToText(Model.BodyHtml);
                }
                catch (Exception ex)
                {
                    throw new HubModelException(new string[] { "The body content could not be processed as HTML (" + ex.Message + ")." });
                }

                //IEnumerable<string> error = Enumerable.Empty<string>();
                //throw new HubModelException(error);

                Guid id = Model.Save(hidAddReleaseType.Value == "true");

                Redirect("~/News/ReleaseManagement/Drafts/" + HubModel.EncodeGuid(id));
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }
        }

        public Dictionary<Guid, string> Images
        {
            get
            {
                using (var db = new legacy::Gcpe.Hub.Data.Entity.HubEntities())
                {
                    var releaseImages = from ri in db.NewsReleaseImages
                                        //TODO: where ri.IsActive
                                        orderby ri.Name
                                        select ri;

                    return releaseImages.ToDictionary(ri => ri.Id, ri => ri.Name);
                }
            }
        }

        private void DisplayErrors(IEnumerable<string> errorMessages)
        {
            pnlErrors.Visible = true;
            rptErrors.DataSource = errorMessages;
            rptErrors.DataBind();
        }

        protected void chklstMinistries_DataBound(object sender, EventArgs e)
        {
            foreach (ListItem item in ((CheckBoxList)sender).Items)
            {
                Guid ministryId = Guid.Parse(item.Value);
                item.Selected = Model.Ministries.Single(m => m.Value == ministryId).Selected;
            }
        }

        protected void cklstSector_DataBound(object sender, EventArgs e)
        {
            foreach (ListItem item in ((CheckBoxList)sender).Items)
                item.Selected = Model.Sectors.Single(s => s.Value == Guid.Parse(item.Value)).Selected;
        }

        protected void cklstTheme_DataBound(object sender, EventArgs e)
        {
            foreach (ListItem item in ((CheckBoxList)sender).Items)
                item.Selected = Model.Themes.Single(s => s.Value == Guid.Parse(item.Value)).Selected;
        }

        protected void cklstTag_DataBound(object sender, EventArgs e)
        {
            foreach (ListItem item in ((CheckBoxList)sender).Items)
                item.Selected = Model.Tags.Single(s => s.Value == Guid.Parse(item.Value)).Selected;
        }
    }
}
