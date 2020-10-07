extern alias legacy;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gcpe.News.ReleaseManagement;
using legacy::Gcpe.Hub.Data.Entity;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Gcpe.Hub.News.ReleaseManagement
{
    public partial class Release : Hub.News.Page
    {
        public ReleaseModel Model;
        private FlickrManager flickrManager = new FlickrManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.Master.HelpPage = "News Release Management - Work with Releases";

            try
            {
                string releaseId = (string)Page.RouteData.Values["Id"];
                if (releaseId != null)
                {
                    Model = new ReleaseModel(HubModel.DecodeGuid(releaseId));
                }
                else
                {
                    string reference = (string)Page.RouteData.Values["Reference"];
                    Model = new ReleaseModel(reference);
                }
            }
            catch (Exception)
            {
                // throw new HttpException(404, null);
                Redirect("~/News/ReleaseManagement/", true);
                return;
            }

            if (IsPostBack)
            {
                DisplayErrors(new string[0]);

                Model.ShowVerboseHistory = !showAllHistory.Visible;
                ctrlNavBar.Model = Model;
                foreach (RepeaterItem rpt in rptDocument.Items)
                {
                    Controls.Document doc = (Controls.Document)rpt.FindControl("ctrlDocument");
                    doc.Model = Model;
                }

                if (!string.IsNullOrWhiteSpace(Model.RequiredTranslations())) Model.HasTranslations = true;
            }
            else
            {
                string folder = (string)Page.RouteData.Values["Folder"];

                bool redirect = false;

                if ((Model.IsCommitted && !Model.IsPublished) ^ folder == "Scheduled")
                    redirect = true;
                else if (Model.IsPublished ^ folder == "Published")
                    redirect = true;
                else if ((!Model.IsCommitted && !Model.IsPublished) ^ folder == "Drafts")
                    redirect = true;

                if (redirect)
                {
                    Redirect("~/News/ReleaseManagement/" + (Model.IsPublished ? "Published" : (Model.IsCommitted ? "Scheduled" : "Drafts")) + "/" + Model.Reference);
                    return;
                }

                if (!Model.IsActive)
                    throw new HttpException(404, null);


                DataBind();

                PopulateMediaDistributionListPicker();


                InitTopOrFeature(homeTopSwitch, homeFeatureSwitch, valHomeTopOrFeature, lblHomeTopOrFeature.Text);

                ScriptManager.GetCurrent(Page).RegisterPostBackControl(btnDelete);

                plannedPublishDateTimePicker.Attributes["onBlur"] = "PublishDateTimePicker_Validate(this)";
                releaseDateTimePicker.Attributes["onBlur"] = "ReleaseDateTimePicker_Validate(this)";

                plannedPublishDateTimePicker.Text = Model.PublishDateTime != null ? Model.PublishDateTime.Value.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture) : "";
                releaseDateTimePicker.Text = Model.ReleaseDate != null ? Model.ReleaseDate.Value.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture) : "";

                if (!string.IsNullOrWhiteSpace(Model.RequiredTranslations())) Model.HasTranslations = true;

                //ScriptManager.GetCurrent(Page).RegisterPostBackControl(CancelApprove);
                //ScriptManager.GetCurrent(Page).RegisterPostBackControl(lbtnCancelCategories);
            }
        }

        public void PopulateMediaDistributionListPicker()
        {
            mediaDistributionListBox.DataSource = Model.MediaDistributionLists;
            mediaDistributionListBox.DataTextField = "Text";
            mediaDistributionListBox.DataValueField = "Value";
            mediaDistributionListBox.DataBind();
        }

        public string MediaDistributionListDisplay()
        {
            string result = String.Empty;
            foreach (var item in Model.MediaDistributionLists)
            {
                if (item.Selected)
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        result = "<ul>";
                    }
                    result += "<li>" + item.Text + "</li>\n";
                }
            }
            if (!string.IsNullOrEmpty(result))
            {
                result += "</ul>";
            }
            return result;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Model.HasMediaAssets)
            {
                AssetUploadBlock.Style["display"] = "none";
            }
            else
            {
                AssetUploadBlock.Style["display"] = "block";
            }

            // Make sure we apply the "selected' items. 
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

            // If a Release has been published before, we should lock down the Media Contacts.
            bool previouslyPublished = Model.History.Any(x => x.DescriptionHtml.StartsWith("Released "));

            if (previouslyPublished)
            {
                if (Model.ReleaseTypeId == ReleaseType.Release || Model.ReleaseTypeId == ReleaseType.Factsheet)
                {
                    // There's client-side JavaScript to complete the disabled state of the Kendo control that is based on this listbox.
                    mediaDistributionListBox.Enabled = false;
                    // any others? Ministry, Sectors, ...Bueller? Bueller? Bueller?
                }
            }
        }

        public string ReleaseStatusHtml
        {
            get
            {
                if (Model.PublishDateTime.HasValue)
                {
                    string format = "<span class=\"state\">{0}</span> {1} ";
                    string statusHtml;
                    if (Model.IsPublished)
                        statusHtml = string.Format(format, "Published", "on");
                    else
                        statusHtml = string.Format(format, Model.IsCommitted ? "Scheduled" : "Planned", "for");

                    return statusHtml + FormatDateTime(Model.PublishDateTime.Value);
                }
                return "No publish date";
            }
        }

        public string PublishDateHtml
        {
            get
            {
                //TODO: Replace with Display Date/Time
                if (Model.PublishDateTime.HasValue)
                {
                    return ReleaseModel.FormatDateTime(Model.PublishDateTime.Value);
                }
                return "None";
            }
        }

        public int ReleaseImageHeight
        {
            get { return 60; }
        }


        #region Populate Edit Options

        public Dictionary<Guid, string> Images
        {
            get
            {
                var releaseImages = from ri in DbContext.NewsReleaseImages
                                        //TODO: where ri.IsActive
                                    orderby ri.Name
                                    select ri;

                return releaseImages.ToDictionary(ri => ri.Id, ri => ri.Name);
            }
        }

        public IEnumerable<string> EnglishLocations
        {
            get
            {
                return Model.EnglishLocations(DbContext);
            }
        }

        #endregion

        public string TopOrFeatureLabel(CategoryItem<Guid> item)
        {
            return Model.Id == item.TopReleaseId ? "Top" : Model.Id == item.FeatureReleaseId ? "Feature" : "";
        }

        public string HomeTopOrFeatureLabel
        {
            get
            {
                string idString = Model.Id.ToString();
                return Model.HomeTopReleaseId == idString ? "Top" : Model.HomeFeatureReleaseId == idString ? "Feature" : "";
            }
        }

        protected void rptCategory_DataBound(object sender, RepeaterItemEventArgs e)
        {
            var item = (Gcpe.Hub.News.CategoryItem<Guid>)e.Item.DataItem;
            CheckBox chk = (CheckBox)e.Item.FindControl("chkCategory");
            chk.Text = item.Text;
            chk.Checked = item.Selected;

            Label topOrFeature = (Label)e.Item.FindControl("lblTopOrFeature");
            if (chk.Checked)
            {
                topOrFeature.Text = TopOrFeatureLabel(item);
                chk.Enabled = string.IsNullOrEmpty(topOrFeature.Text);
            }
            InitTopOrFeature((HyperLink)e.Item.FindControl("topSwitch"),
                             (HyperLink)e.Item.FindControl("featureSwitch"),
                             (HiddenField)e.Item.FindControl("valTopOrFeature"),
                             topOrFeature.Text, chk.Checked);
        }

        private void InitTopOrFeature(HyperLink topSwitch, HyperLink featureSwitch, HiddenField valTopOrFeature, string topOrFeatureText, bool isValid = true)
        {
            if (!Model.IsCommitted || Model.IsScheduled)
            {
                topSwitch.Visible = false;
                featureSwitch.Visible = false;
                valTopOrFeature.Visible = false;
            }
            else if (!string.IsNullOrEmpty(topOrFeatureText))
            {
                bool isTop = (topOrFeatureText == "Top");
                topSwitch.Text = "Set as " + (isTop ? "Feature" : "Top");
                featureSwitch.Style["visibility"] = "hidden";
                featureSwitch.Text = topSwitch.Text; // to restore label on cancel
            }
            else if (!isValid)
            {
                topSwitch.Style["visibility"] = "hidden";
                featureSwitch.Style["visibility"] = "hidden";
            }
        }

        protected void cklstTag_DataBound(object sender, EventArgs e)
        {
            foreach (ListItem item in ((CheckBoxList)sender).Items)
                item.Selected = Model.Tags.Single(r => r.Value == Guid.Parse(item.Value)).Selected;
        }

        protected void SaveRepeater(RepeaterItemCollection rptItems, IEnumerable<CategoryItem<Guid>> modelCategories)
        {
            foreach (RepeaterItem rpt in rptItems)
            {
                CheckBox chk = (CheckBox)rpt.FindControl("chkCategory");
                Gcpe.Hub.News.CategoryItem<Guid> item = modelCategories.Single(m => m.Text == chk.Text);
                item.Selected = chk.Checked;
                if (item.Selected)
                {
                    HiddenField valTopOrFeature = (HiddenField)rpt.FindControl("valTopOrFeature");
                    if (valTopOrFeature.Value == "Top")
                    {
                        if (item.FeatureReleaseId == Model.Id)
                            item.FeatureReleaseId = item.TopReleaseId; //Swap Top and feature
                        item.TopReleaseId = Model.Id;
                    }
                    else if (valTopOrFeature.Value == "Feature")
                    {
                        if (item.TopReleaseId == Model.Id)
                            item.TopReleaseId = item.FeatureReleaseId; //Swap Top and feature
                        item.FeatureReleaseId = Model.Id;
                    }
                }
            }
        }

        protected void btnSaveAssets_Click(object sender, EventArgs e)
        {
            // Used to compare with the Post-save Asset Uri. 
            string superAssetBeforeSave = String.Empty;
            if (Model.Asset != null)
            {
                superAssetBeforeSave = Model.Asset.ToString();
            }

            try
            {
                Uri assetUri;
                if (string.IsNullOrWhiteSpace(txtAsset.Text))
                {
                    Model.Asset = null;
                }
                else if (Uri.TryCreate(txtAsset.Text, UriKind.Absolute, out assetUri))
                {
                    if (assetUri.Host.Contains("flickr") || assetUri.Host.Contains("flic.kr"))
                    {
                        // handle short urls, get the id and construct public url to display the preview
                        if (assetUri.Host == "flic.kr" && !string.IsNullOrWhiteSpace(assetUri.Segments[2].TrimEnd('/')))
                        {
                            var key = assetUri.Segments[2].TrimEnd('/');
                            assetUri = new Uri(flickrManager.ConstructPublicAssetUrl(key));
                        }

                        string photoId = "";

                        if (assetUri.Segments.Length == 4)
                            photoId = assetUri.Segments[3].TrimEnd('/');


                        // if the asset is public, load the preview
                        if (!string.IsNullOrWhiteSpace(photoId) && flickrManager.IsAssetPublic(photoId))
                            assetUri = AssetEmbedManager.NormalizeFlickrUri(assetUri);

                        if (assetUri == null)
                            throw new HubModelException(new string[] { "Invalid Flickr URL for SuperAsset (" + txtAsset.Text + ")." });
                    }
                    else if (assetUri.Host.Contains("youtube") || assetUri.Host.Contains("youtu.be"))
                    {
                        assetUri = AssetEmbedManager.NormalizeYoutubeUri(assetUri);

                        if (assetUri == null)
                            throw new HubModelException(new string[] { "Invalid YouTube URL for SuperAsset (" + txtAsset.Text + ")." });
                    }
                    else if (assetUri.Host.Contains("facebook.com"))
                    {
                        throw new HubModelException(new string[] { "Facebook is no longer supported due to privacy concerns. Use YouTube or Flickr URLs instead." });
                    }
                    else if (assetUri.ToString() != "https://news.gov.bc.ca/live")
                    {
                        throw new HubModelException(new string[] { "Unknown type of SuperAsset (" + txtAsset.Text + ")." });
                    }

                    Model.Asset = assetUri;
                }
                else
                {
                    throw new HubModelException(new string[] { "Invalid URL format for SuperAsset (" + Model.Asset + ")." });
                }
                Model.SocialMediaHeadline = txtSocialMediaHeadline.Text;
                Model.HasMediaAssets = chkHasMediaAssets.Checked;

                if (Model.IsCommitted && Model.IsPublished)
                {
                    Model.SaveWithLog("Release Assets Updated"); // will set IsPublish to false.
                }
                else
                {
                    Model.Save();
                }

                foreach (RepeaterItem item in rptAssetList.Items)
                {
                    Label label = (Label)item.FindControl("file");
                    HiddenField valTopOrFeature = (HiddenField)item.FindControl("valDeleted");
                    if (valTopOrFeature.Value.ToLower() == "true")
                    {
                        DeleteMediaAssets(label.Text);
                    }
                }

                // As a final step...
                // If this "IsCommitted", it's published & therefore accessible by Facebook.
                // Force a Facebook cache refresh anytime we change the 
                // SuperAsset otherwise new shares will get a stale post image.

                string superAssetAfterSave = string.Empty;
                if (Model.Asset != null)
                {
                    superAssetAfterSave = Model.Asset.ToString();
                }

                bool superAssetHasChanged = (superAssetBeforeSave != superAssetAfterSave);

            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }
            catch (Exception fde)
            {
                List<string> errorMessages = new List<string>();
                errorMessages.Add(fde.Message);
                DisplayErrors(errorMessages);
            }
            DataBind();
        }

        protected void btnSaveTranslations_Click(object sender, EventArgs e)
        {
            try
            {
                Model.HasTranslations = chkHasTranslations.Checked;

                if (Model.IsCommitted && Model.IsPublished || (Model.IsCommitted && !Model.IsPublished) || Model.IsApproved)
                {
                    Model.SaveWithLog("Translations Updated"); // will set IsPublish to false.
                }
                else
                {
                    Model.Save();
                }

                var translationsUpdatedBeforePublish
                   = Model.MostRecentLogMessages.Count() > 1
                   && Model.MostRecentLogMessages?[1] != null
                   && Model.MostRecentLogMessages?[1]?.Description == "Translations Updated";

                var lastUpdateWasTranslations = Model.MostRecentLogMessages?.LastOrDefault()?.Description == "Translations Updated";

                Model.DisplayTranslationsAnchorPanel = lastUpdateWasTranslations || translationsUpdatedBeforePublish;

                foreach (RepeaterItem item in rptTranslationList.Items)
                {
                    Label label = (Label)item.FindControl("file");
                    HiddenField valTopOrFeature = (HiddenField)item.FindControl("valDeleted");
                    if (valTopOrFeature.Value.ToLower() == "true")
                    {
                        DeleteTranslations(label.Text);
                    }
                }
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }
            catch (Exception fde)
            {
                List<string> errorMessages = new List<string>();
                errorMessages.Add(fde.Message);
                DisplayErrors(errorMessages);
            }
            DataBind();
        }

        protected void btnSaveMeta_Click(object sender, EventArgs e)
        {
            try
            {
                //TOOD: Check time zone

                DateTime releaseDate;
                if (DateTime.TryParse(releaseDateTimePicker.Text, out releaseDate))
                    Model.ReleaseDate = releaseDate;
                else
                    Model.ReleaseDate = (DateTime?)null;

                Model.Location = txtLocation.Text;
                Model.Keywords = txtKeywords.Text;

                Model.Summary = txtSummary.Text;
                Model.SocialMediaSummary = txtSocialMediaSummary.Text;
                //Model.SocialMediaHeadline = txtSocialMediaHeadline.Text;
                Uri uriResult;
                if (string.IsNullOrEmpty(txtRedirect.Text))
                {
                    Model.RedirectUrl = null;
                }
                else if (Uri.TryCreate(txtRedirect.Text, UriKind.Absolute, out uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                {
                    Model.RedirectUrl = uriResult;
                }
                else
                {
                    throw new HubModelException(new string[] { "Invalid Redirect URL format for (" + txtRedirect.Text + ")." });
                }

                if (Model.IsCommitted && Model.IsPublished)
                {
                    Model.SaveWithLog("Release Meta Updated"); // will set IsPublished to false.
                }
                else
                {
                    if (Model.Key != txtKey.Text)
                    {
                        // make sure the user didn't enter invalid characters.
                        string newKey = ReleaseManagementModel.GenerateSlug(txtKey.Text);
                        if (string.IsNullOrEmpty(newKey))
                        {
                            throw new HubModelException(new string[] { "The Story URL cannot be blank" });
                        }
                        Model.Key = newKey;
                    }
                    Model.Save();
                }
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }

            DataBind();
        }

        protected void btnSaveCategories_Click(object sender, EventArgs e)
        {
            try
            {
                string Id = Model.Id.ToString();
                if (valHomeTopOrFeature.Value == "Top")
                {
                    if (Model.HomeFeatureReleaseId == Id)
                        Model.HomeFeatureReleaseId = Model.HomeTopReleaseId; //Swap Top and feature
                    Model.HomeTopReleaseId = Id;
                }
                else if (valHomeTopOrFeature.Value == "Feature")
                {
                    if (Model.HomeTopReleaseId == Id)
                        Model.HomeTopReleaseId = Model.HomeFeatureReleaseId; //Swap Top and feature
                    Model.HomeFeatureReleaseId = Id;
                }

                SaveRepeater(rptMinistry.Items, Model.Ministries);
                SaveRepeater(rptSector.Items, Model.Sectors);
                SaveRepeater(rptTheme.Items, Model.Themes);

                foreach (ListItem item in cklstTag.Items)
                    Model.Tags.Single(s => s.Value == Guid.Parse(item.Value)).Selected = item.Selected;

                if (Model.IsCommitted && Model.IsPublished)
                {
                    Model.SaveWithLog("Release Categories Updated"); // will set IsPublish to false.
                }
                else
                {
                    Model.Save();
                }
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }

            DataBind();
        }

        protected void btnSavePlannedPublishDate_Click(object sender, EventArgs e)
        {
            try
            {
                int corpCalId;
                if (int.TryParse(txtCorpCalId.Text, out corpCalId))
                {
                    Model.ActivityId = corpCalId;
                }
                else
                {
                    Model.ActivityId = null;
                }

                Model.PublishNewsOnDemand = chkNewsOnDemand.Checked;

                //TOOD: Check time zone
                DateTime publishDate;
                if (DateTime.TryParse(plannedPublishDateTimePicker.Text, out publishDate))
                    Model.PublishDateTime = publishDate;
                else
                    Model.PublishDateTime = (DateTime?)null;

                System.Diagnostics.Debug.Assert(!Model.IsCommitted || !Model.IsPublished);

                foreach (ListItem item in mediaDistributionListBox.Items)
                    Model.MediaDistributionLists.Single(s => s.Value == Guid.Parse(item.Value)).Selected = item.Selected;

                Model.Save();
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }

            DataBind();
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(rblMinistry.SelectedValue))
                    Model.MinistryId = Guid.Parse(rblMinistry.SelectedItem.Value);

                Model.Approve();
                //Redirect("~/News/ReleaseManagement/Drafts/" + HttpUtility.UrlEncode(Model.Reference));
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }

            DataBind();
        }

        protected void btnPublish_Click(object sender, EventArgs e)
        {
            //rdTime or rdSchedule 
            //chkPublishToInPublish
            try
            {
                //if (rdNow.Checked)
                //{
                //    //TODO: Drop ms from publish time.
                //    Model.PublishDateTime = UtcPage;
                //}
                //else if (rdSchedule.Checked)
                //{
                //    Model.PublishDateTime = dtpScheduleDate.SelectedDate;

                //    Model.RssPublishIndex = chkPublishToInPublish.Checked;
                //}

                Model.Publish();

                // If this IsCommitted after the SuperAsset has been set, force the refresh.
                //if (Model.IsCommitted)
                //{
                //    await RefreshFacebookCacheAsync(Model);
                //}
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }
            catch (Exception fde)
            {
                List<string> errorMessages = new List<string>();
                errorMessages.Add(fde.Message);
                DisplayErrors(errorMessages);
            }

            DataBind();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Model.Delete();
                Redirect("~/News/ReleaseManagement/", true);
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }
        }

        private void DisplayErrors(IEnumerable<string> errorMessages)
        {
            pnlErrors.Visible = errorMessages.Any();
            rptErrors.DataSource = errorMessages;
            rptErrors.DataBind();
        }

        private void DeleteMediaAssets(string fileName)
        {
#if !LOCAL_MEDIA_STORAGE
            var container = new CloudBlobContainer(Global.ModifyContainerWithSharedAccessSignature("assets"));

            var directory = container.GetDirectoryReference(Model.ReleasePathName);

            var file = directory.GetDirectoryReference(Model.Key.ToLower()).GetBlobReference(fileName.ToLower());
            if (!file.Exists())
            {
                file = directory.GetDirectoryReference(Model.Key).GetBlobReference(fileName); // for backwards compatibility
            }

            file.Delete();
#else
            string directory = Path.Combine(Settings.Default.MediaAssetsUnc, Model.ReleasePathName, Model.Key);
            if (System.IO.File.Exists(Path.Combine(directory, fileName)))
            {
                System.IO.File.Delete(Path.Combine(directory, fileName));

                Global.QueueBackgroundWorkItemWithRetry(() =>
                {
                    foreach (string folder in Properties.Settings.Default.DeployFolders)
                    {
                        string deployDirectory = Path.Combine(folder, Model.ReleasePathName, Model.Key);

                        if (System.IO.File.Exists(Path.Combine(deployDirectory, fileName)))
                        {
                            System.IO.File.Delete(Path.Combine(deployDirectory, fileName));

                            if (!System.IO.Directory.GetFiles(deployDirectory).Any())
                            {
                                System.IO.Directory.Delete(deployDirectory);
                            }
                        }
                    }
                });

                //Delete the folder if the folder is empty
                string[] files = Directory.GetFiles(directory);
                List<object> fileList = new List<object>();

                if (files.Count() == 0)
                {
                    Directory.Delete(directory);
                }
            }
#endif
        }

        private void DeleteTranslations(string fileName)
        {
#if !LOCAL_MEDIA_STORAGE
            var container = new CloudBlobContainer(Global.ModifyContainerWithSharedAccessSignature("translations"));

            var directory = container.GetDirectoryReference(Model.ReleasePathName);

            var file = directory.GetDirectoryReference(Model.Key.ToLower()).GetBlobReference(fileName.ToLower());
            if (!file.Exists())
            {
                file = directory.GetDirectoryReference(Model.Key).GetBlobReference(fileName); // for backwards compatibility
            }

            file.Delete();
#else
            string directory = Path.Combine(Settings.Default.MediaAssetsUnc, Model.ReleasePathName, Model.Key);
            if (System.IO.File.Exists(Path.Combine(directory, fileName)))
            {
                System.IO.File.Delete(Path.Combine(directory, fileName));

                Global.QueueBackgroundWorkItemWithRetry(() =>
                {
                    foreach (string folder in Properties.Settings.Default.DeployFolders)
                    {
                        string deployDirectory = Path.Combine(folder, Model.ReleasePathName, Model.Key);

                        if (System.IO.File.Exists(Path.Combine(deployDirectory, fileName)))
                        {
                            System.IO.File.Delete(Path.Combine(deployDirectory, fileName));

                            if (!System.IO.Directory.GetFiles(deployDirectory).Any())
                            {
                                System.IO.Directory.Delete(deployDirectory);
                            }
                        }
                    }
                });

                //Delete the folder if the folder is empty
                string[] files = Directory.GetFiles(directory);
                List<object> fileList = new List<object>();

                if (files.Count() == 0)
                {
                    Directory.Delete(directory);
                }
            }
#endif
        }

        protected void lbtnStopPublishSwitch_Click(object sender, EventArgs e)
        {
            try
            {
                Model.Unpublish();

                DataBind();
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }
        }

        protected void lbtnUnpublishSwitch_Click(object sender, EventArgs e)
        {
            try
            {
                Model.Unpublish();

                DataBind();
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }
        }

        public string GetNewArchiveUrl()
        {
            string archivePath;

            if (Request.Url.Host == "localhost")
                archivePath = "http://dev.www2.news.gov.bc.ca";
            else
                archivePath = "http://" + Request.Url.Host.Replace("internal.", "").Replace("hub.gcpe.gov.bc.ca", "www2.news.gov.bc.ca");

            archivePath += "/news_releases_" + Model.CollectionName + "/" + Model.Reference + ".htm";

            return archivePath;
        }

        protected void CancelApprove_Click(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void CancelSavePlannedPublishDate_Click(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void lbtnCancelCategories_Click(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void lbtnCancelAssets_Click(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void lbtnCancelTranslations_Click(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void lbtnCancelMeta_Click(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void rblMinistry_DataBound(object sender, EventArgs e)
        {
            foreach (ListItem item in ((RadioButtonList)sender).Items)
            {
                Guid ministryId = Guid.Parse(item.Value);
                item.Selected = Model.MinistryId == ministryId;
            }
        }

        protected void showAllHistory_Click(object sender, EventArgs e)
        {
            showAllHistory.Visible = false;
            Model.ShowVerboseHistory = true;

            DataBind();
        }

        //protected void refreshTimer_Tick(object sender, EventArgs e)
        //{
        //    Model.Summary = DateTime.Now.ToString() + " " + DateTime.Now.Ticks.ToString();

        //    DataBind();
        //}
    }
}
