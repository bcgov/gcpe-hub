extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Gcpe.News.ReleaseManagement;
using HtmlAgilityPack;

namespace Gcpe.Hub.News.ReleaseManagement.Controls
{
    using legacy::Gcpe.Hub.Data.Entity;
    using Microsoft.AspNet.FriendlyUrls;

    public partial class Document : System.Web.UI.UserControl
    {
        public ReleaseModel Model;
        public NewsReleaseDocumentLanguage documentLanguage;

        public const legacy::Gcpe.Hub.Data.Entity.PageLayout FormalPageLayout = legacy::Gcpe.Hub.Data.Entity.PageLayout.Formal;
        public const legacy::Gcpe.Hub.Data.Entity.PageLayout InformalPageLayout = legacy::Gcpe.Hub.Data.Entity.PageLayout.Informal;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnSaveDocument.UseSubmitBehavior = false;
                //ScriptManager.GetCurrent(Page).RegisterPostBackControl(lbtnCancelSave);

                //ScriptManager.GetCurrent(Page).RegisterPostBackControl(btnDelete);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            contentCKEditor.InitNewsReleaseEditor(BodyHtml);
        }

        public bool DocumentIsNew
        {
            get { return DocumentLanguage == null || DocumentLanguage.Headline == null; }
        }

        public string DocumentPath
        {
            get
            {
                return documentPath.Value;
            }
            set
            {
                documentPath.Value = value;
            }
        }

        public NewsReleaseDocumentLanguage DocumentLanguage
        {
            get
            {
                if (documentLanguage == null)
                {
                    documentLanguage = Model.DocumentLanguage(SortIndex, LanguageId); // this will create the documentLanguage if it doesn't exist
                }
                return documentLanguage;
            }
        }

        public NewsReleaseLanguage ReleaseLanguage
        {
            get { return DocumentLanguage.Document.Release.Languages.SingleOrDefault(e => e.Language.Id == LanguageId); }
        }

        public int SortIndex
        {
            get
            {
                return Int16.Parse(documentPath.Value.Split('/')[0]) - 1;
            }
        }

        public int LanguageId
        {
            get
            {
                string[] values = documentPath.Value.Split('/');
                if (values.Length > 1)
                {
                    if (values[1] == "fr-CA")
                        return Language.frCA;
                    else if (values[1] != "en-CA")
                        throw new NotImplementedException();
                }
                return Language.enCA;
            }
        }

        public Dictionary<PageLayout, string> PageLayouts
        {
            get
            {
                IEnumerable<PageLayout> pageLayouts = Enum.GetValues(typeof(PageLayout)).Cast<PageLayout>();
                return pageLayouts.ToDictionary(pl => pl, pl => pl.ToString());
            }
        }

        public string PageTitle
        {
            get { return DocumentLanguage.PageTitle; }
            set { DocumentLanguage.PageTitle = value; }
        }

        public bool IsFirst
        {
            get
            {
                return SortIndex == 0;
            }
        }

        public string LanguageName
        {
            get { return DocumentLanguage.Language.Name; }
        }

        public string BodyHtml
        {
            get { return DocumentLanguage.BodyHtml; }
            set { DocumentLanguage.BodyHtml = value; }
        }

        public IEnumerable<int> AddLanguages
        {
            get
            {
                return Model.AddLanguages(DocumentLanguage.Document.Languages);
            }
        }

        public string PreviewPageTitleHtml
        {
            get
            {
                return "<b>" + (HttpUtility.HtmlEncode(PageTitle) ?? "").ToUpper() + "</b>";
            }
        }

        //TODO Remove duplicate code 
        public string PreviewTopLeftHeaderHtml
        {
            get
            {
                if (PageLayout == legacy::Gcpe.Hub.Data.Entity.PageLayout.Formal)
                {
                    if (LanguageId == Language.enCA)
                    {
                        string text = "For Immediate Release";

                        if (string.IsNullOrEmpty(Model.Reference) || Model.LeadOrganization == null)
                            text += (text == "" ? "" : "<br />") + "Not Approved";
                        else if (Model.ReleaseTypeId != ReleaseType.Advisory)
                            text += (text == "" ? "" : "<br />") + (Model.ReleaseTypeId == ReleaseType.Release ? Model.Key : Model.Reference);

                        //TODO: Display Release Date
                        text += (text == "" ? "" : "<br />") + Gcpe.News.ReleaseManagement.Templates.Release.CPDateFormat(Model.ReleaseDate ?? Model.PublishDateTime ?? DateTime.Today);

                        return text;
                    }
                    else if (LanguageId == Language.frCA)
                    {
                        string text = "Pour diffusion immédiate";

                        if (string.IsNullOrEmpty(Model.Reference) || Model.LeadOrganization == null)
                            text += (text == "" ? "" : "<br />") + "Not Approved";
                        else if (Model.ReleaseTypeId != ReleaseType.Advisory)
                            text += (text == "" ? "" : "<br />") + (Model.ReleaseTypeId == ReleaseType.Release ? Model.Key : Model.Reference);

                        text += (text == "" ? "" : "<br />") + (Model.ReleaseDate ?? Model.PublishDateTime ?? DateTime.Today).ToString("d MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo(LanguageId));
                        return text;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public string PreviewTopRightHeaderHtml
        {
            get
            {
                if (PageLayout == legacy::Gcpe.Hub.Data.Entity.PageLayout.Formal)
                {
                    return HttpUtility.HtmlEncode(Organizations).Replace("\r", "").Replace("\n", "<br />");
                }
                else
                {
                    return null;
                }
            }
        }

        public string PreviewMiddleHeaderHtml
        {
            get
            {
                string html = "<b>" + HttpUtility.HtmlEncode(Headline) + "</b>" + (string.IsNullOrEmpty(Subheadline) ? "" : "<br />" + HttpUtility.HtmlEncode(Subheadline));

                if (LanguageId == Language.enCA && HasTranslation)
                    html += "<br />(disponible en français en bas de page)";

                return html;
            }
        }

        public string PreviewBottomHeaderHtml
        {
            get
            {
                if (PageLayout == legacy::Gcpe.Hub.Data.Entity.PageLayout.Formal)
                {
                    return null;
                }
                else
                {
                    string text = (string.IsNullOrEmpty(Byline) ? "" : "<b>" + HttpUtility.HtmlEncode(Byline).Replace("\r\n", "\n").Replace("\n", "<br />") + "</b>");

                    DateTime? releaseDate;

                    if (Model.ReleaseDate.HasValue)
                        releaseDate = Model.ReleaseDate.Value;
                    else if (Model.PublishDateTime.HasValue)
                        releaseDate = Model.PublishDateTime.Value.DateTime;
                    else
                        releaseDate = null;

                    text += (text == "" ? "" : "<br /><br />");

                    if (LanguageId == Language.enCA)
                        text += (releaseDate.HasValue ? Gcpe.News.ReleaseManagement.Templates.Release.CPDateFormat(releaseDate.Value) : "No Release Date");
                    else if (LanguageId == Language.frCA)
                        text += (releaseDate.HasValue ? releaseDate.Value.ToString("d MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo(LanguageId)) : "");

                    return text;
                }
            }
        }

        public string PreviewBodyHtml
        {
            get
            {
                string location;

                if (IsFirst)
                {
                    string releaseLocation = ReleaseLanguage.Location;
                    location = string.IsNullOrEmpty(releaseLocation) ? Model.Location : releaseLocation;
                }
                else
                    location = "";

                string bodyHtml = Gcpe.News.ReleaseManagement.Templates.Release.MergeLocationBody(location, BodyHtml);
                bodyHtml = ShowLinks(bodyHtml);
                return AssetEmbedManager.RenderAssetsInHtml(bodyHtml);
            }
        }

        public IEnumerable<string> PreviewContactsHtml
        {
            get
            {
                return Contacts.Select(e => HttpUtility.HtmlEncode(e).Replace("\r", "").Replace("\n", "<br />"));
            }
        }

        public bool HasTranslation
        {
            get
            {
                return DocumentLanguage.Document.Languages.Count() > 1;
            }
        }
        public bool CanDeleteDocument
        {
            get
            {
                return !DocumentIsNew && (!IsFirst || LanguageId != Language.enCA);
            }
        }

        public Guid? PageImageId
        {
            get
            {
                NewsReleaseImage pageImage = DocumentLanguage.PageImage;
                return pageImage == null ? (Guid?)null : pageImage.Id;
            }
            set
            {
                var image = value.HasValue ? Model.FindImage(value.Value) : (NewsReleaseImage)null;

                if (image != DocumentLanguage.PageImage)
                    DocumentLanguage.PageImage = image;
            }
        }

        public PageLayout PageLayout
        {
            get { return DocumentLanguage.Document.PageLayout; }
            set { DocumentLanguage.Document.PageLayout = value; }
        }

        public string Organizations
        {
            get { return DocumentLanguage.Organizations; }
            set { DocumentLanguage.Organizations = value; }
        }

        public string Headline
        {
            get { return DocumentLanguage.Headline; }
            set { DocumentLanguage.Headline = value; }
        }

        public string Subheadline
        {
            get { return DocumentLanguage.Subheadline; }
            set { DocumentLanguage.Subheadline = value; }
        }

        public string Byline
        {
            get { return DocumentLanguage.Byline; }
            set { DocumentLanguage.Byline = value; }
        }

        public IEnumerable<string> Contacts
        {
            get
            {
                return DocumentLanguage.Contacts.OrderBy(e => e.SortIndex).Select(e => e.Information);
            }
        }

        protected void btnSaveDocument_Click(object sender, EventArgs e)
        {
            PageTitle = txtPageTitle.Text.Trim();

            if (rblPageLayout.SelectedItem != null)
                PageLayout = (PageLayout)Enum.Parse(typeof(PageLayout), rblPageLayout.SelectedItem.Value);


            string oldHeadLine = Headline;
            Headline = txtHeadline.Text.Trim();

            Subheadline = txtSubheadline.Text.Trim();

            PageLayout pageLayout = PageLayout;
            Organizations = pageLayout == PageLayout.Formal ? txtOrganization.Text.Trim() : "";
            Byline = pageLayout == PageLayout.Informal ? txtByline.Text.Trim() : "";


            PageImageId = pageImagePicker.Value;

            DocumentLanguage.Contacts.Clear();
            string paramKey = "parameters" + DocumentPath + ":";
            int sortIndex = 0;
            foreach (string key in Request.Form.AllKeys)
            {
                if (key != null && key.StartsWith(paramKey))
                {
                    string contact = Request.Form[key].Trim();
                    if (!string.IsNullOrEmpty(contact))
                    {
                        DocumentLanguage.Contacts.Add(new NewsReleaseDocumentContact() { SortIndex = sortIndex++, Information = contact });
                    }
                }
            }

            try
            {
                UpdateSummary();

                if (IsFirst && LanguageId == Language.enCA)
                {
                    if (Model.IsKeyEditable && oldHeadLine != DocumentLanguage.Headline)
                        Model.Key = ReleaseManagementModel.GenerateSlug(DocumentLanguage.Headline);
                }
                NewsReleaseType newReleaseType = null;
                if (hidAddReleaseType.Value == "true")
                    Model.CreateNewsReleaseTypeIfNeeded(PageTitle, DocumentLanguage.Language);

                if (newReleaseType != null)
                {
                    //TODO: Change ReleaseType PageLayout to Enum
                    newReleaseType.PageLayout = (int)PageLayout;

                    if (PageImageId.HasValue)
                        newReleaseType.PageImage = Model.FindImage(PageImageId.Value);
                }

                if (DocumentIsNew)
                    Model.SaveWithLog("Added " + PageTitle + " Document"); /* NewsReleaseLog.Description */
                else
                    Model.SaveWithLog("Updated " + PageTitle + " Document"); /* NewsReleaseLog.Description */

                // Not happening in page load so forcing layout to be correct
                DocumentPreview.Attributes.Remove("class");
                DetailsEdit.Attributes.Add("class", "edit");
                Page.DataBind();// update view section
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);

                // Stay in edit mode when error
                DocumentPreview.Attributes.Add("class", "hidden");
                DetailsEdit.Attributes.Add("class", "edit-visible");
            }
        }

        private void UpdateSummary()
        {
            if (!ReleaseManagementModel.ValidateUrls(contentCKEditor.Text))
                throw new HubModelException(new string[] { "The body content contains links in invalid format. Please use http:// or https:// valid format." });

            string bodyHtml = AssetEmbedManager.NormalizeAssetTags(contentCKEditor.Text);
            string previousBodyText = NewModel.TidyAndTruncateDocumentBodyText(
                Gcpe.News.ReleaseManagement.Templates.Convert.HtmlToText(TidyDocumentBodyHtml(DocumentLanguage.BodyHtml)));
            if (DocumentLanguage.BodyHtml != bodyHtml)
            {
                DocumentLanguage.BodyHtml = bodyHtml;
                bodyHtml = TidyDocumentBodyHtml(bodyHtml);

                //Only recreate the Summary field if BodyHtml has been changed.
                //and the content of the Summary hasn't diverged from the lede  
                try
                {
                    if (IsFirst && LanguageId == Language.enCA)
                    {
                        //TODO: Find "lead paragraph to populate RSS Summary field

                        string bodyText =
                            NewModel.TidyAndTruncateDocumentBodyText(
                                Gcpe.News.ReleaseManagement.Templates.Convert.HtmlToText(bodyHtml));


                        //TODO: Notify users that the Summary has been updated.

                        // if summary has not been customized to be something other than the lede
                        if (Gcpe.News.ReleaseManagement.Utils.TrimSummary(previousBodyText, 500) ==
                            ReleaseLanguage.Summary)
                        {
                            ReleaseLanguage.Summary =
                                Utils.TrimSummary(bodyText, 500);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new HubModelException(new string[]
                    {"The body content could not be processed as HTML (" + ex.Message + ")."});
                }
            }
        }

        private static string TidyDocumentBodyHtml(string bodyHtml)
        {
            bodyHtml = bodyHtml.Replace("<p>&nbsp;</p>", "");
            while (bodyHtml.Contains("\r\n\r\n"))
                bodyHtml = bodyHtml.Replace("\r\n\r\n", "\r\n");
            return bodyHtml;
        }

        private static string ShowLinks(string bodyHtml)
        {
            if (string.IsNullOrWhiteSpace(bodyHtml))
                return bodyHtml;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(bodyHtml);
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]") ?? Enumerable.Empty<HtmlNode>())
            {
                string href = link.GetAttributeValue("href", string.Empty);

                Uri uri;
                if (Uri.TryCreate(href, UriKind.Absolute, out uri))
                {
                    string host = uri.Host;

                    if (host.StartsWith("www."))
                        host = host.Substring("www.".Length);

                    if (!host.Contains("gov.bc.ca") && !link.InnerText.ToLower().Contains(host.ToLower()))
                    {
                        string linktext = link.OuterHtml.Replace(">" + link.InnerHtml + "</a>", " aria-label=" + "'external link to " + host + "'>" + link.InnerHtml + "</a>");
                        bodyHtml = bodyHtml.Replace(link.OuterHtml, linktext + "<span class='subscript'>&nbsp;(" + host + ") </span>");
                    }
                }
            }
            return bodyHtml;
        }



        public string GetPageImageUrl()
        {
            //TODO: Calculate
            const int previewReleaseImageHeight = 68; // 83;

            return PageImageId.HasValue ? Gcpe.Hub.News.ReleaseManagement.Controls.ReleaseImagePicker.GetReleaseImageClientUrl(Page, previewReleaseImageHeight, PageImageId) : null;
        }

        private void DisplayErrors(IEnumerable<string> errorMessages)
        {
            pnlErrors.Visible = true;

            rptErrors.DataSource = errorMessages;
            rptErrors.DataBind();

        }
        public string DeleteMessage()
        {
            if (LanguageId == Language.enCA)
            {
                if (HasTranslation)
                    return "'This will delete the current document and all translations.'";
                else
                    return "'This will delete the current document.'";
            }
            else
            {
                return "'This will delete the " + DocumentLanguage.Language.Name + " translation of this document.'";
            }
        }

        protected void btnAddLanguage_Click(object sender, EventArgs e)
        {
            // This will not be saved to the db yet. Just a place holder until the user fills it up and click Save
            Model.AddDocumentLanguagePlaceHolder(SortIndex, Language.frCA);
            Page.DataBind();
            // Scroll to the newly added document. Timeout is required for this to work correctly.
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "ScrollToNewDocument", "setTimeout(function() { ScrollToId('#Documents-" + Model.DocumentCount + "-fr-CA');}, 1000);", true);
        }


        protected void lbtnCancelSave_Click(object sender, EventArgs e)
        {
            Page.DataBind();
        }

        protected void lbtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Model.DeleteDocumentLanguage(DocumentLanguage);
                Page.DataBind();
            }
            catch (HubModelException mEx)
            {
                DisplayErrors(mEx.Errors);
            }
            //Page.DataBind();
        }
    }
}