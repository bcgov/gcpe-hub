extern alias legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using legacy::Gcpe.Hub.Data.Entity;

namespace Gcpe.Hub.News.ReleaseManagement
{
    public class ReleaseManagementModel : HubModel
    {
        protected NewsRelease Release;

        public override void Save()
        {
            base.Save();
            if (Release.IsCommitted)
            {
                ReleasePublisher.UpdateNextNewsReleasePublishDate(Release); // always after saving to avoid race conditions
            }
        }

        public virtual void SaveWithLog(string description)
        {
            bool needsRepublishing = Release.IsCommitted && Release.IsPublished;
            if (needsRepublishing)
                Release.IsPublished = false;

            NewsReleaseLog log = new NewsReleaseLog();

            log.DateTime = DateTimeOffset.Now;
            log.User = User;
            log.Description = description;
            log.Release = Release;

            Release.Logs.Add(log);

            db.NewsReleaseLogs.Add(log);

            try
            {
                base.Save();
            }
            catch
            {
                Release.Logs.Remove(log);
                throw;
            }
            if (needsRepublishing)
            {
                ReleasePublisher.UpdateNextNewsReleasePublishDate(Release); // always after saving to avoid race conditions
            }
        }
        public NewsReleaseDocumentLanguage DocumentLanguage(int sortIndex, int languageId)
        {
            NewsReleaseDocumentLanguage documentLanguage;
            NewsReleaseDocument document = Release.Documents.FirstOrDefault(e => e.SortIndex == sortIndex);
            if (document == null)
            {
                System.Diagnostics.Debug.Assert(languageId == db.English().Id);

                //Create New Document
                document = new NewsReleaseDocument();
                document.Id = Guid.NewGuid();
                document.SortIndex = Release.Documents.Count();
                Release.Documents.Add(document);

                documentLanguage = new NewsReleaseDocumentLanguage();
                documentLanguage.LanguageId = languageId;
                documentLanguage.Language = db.Languages.Find(languageId);
                documentLanguage.Document = document;
                documentLanguage.BodyHtml = "";
                document.Languages.Add(documentLanguage);

                var firstDocument = Release.Documents.OrderBy(e => e.SortIndex).First();

                document.PageLayout = firstDocument.PageLayout;

                var firstDocumentLanguage = firstDocument.Languages.SingleOrDefault(e => e.Language.Id == languageId);

                if (document.PageLayout == legacy::Gcpe.Hub.Data.Entity.PageLayout.Formal)
                    documentLanguage.Organizations = firstDocumentLanguage.Organizations;

                documentLanguage.PageImage = firstDocumentLanguage.PageImage;

                foreach (var firstContact in firstDocumentLanguage.Contacts)
                {
                    NewsReleaseDocumentContact contact = new NewsReleaseDocumentContact();
                    contact.SortIndex = firstContact.SortIndex;
                    contact.Information = firstContact.Information;
                    documentLanguage.Contacts.Add(contact);
                }
            }
            else
            {
                documentLanguage = document.Languages.SingleOrDefault(e => e.Language.Id == languageId);
                if (documentLanguage == null)
                {
                    documentLanguage = new NewsReleaseDocumentLanguage();
                    documentLanguage.LanguageId = languageId;
                    documentLanguage.Language = db.Languages.Find(languageId);
                    documentLanguage.Document = document;

                    documentLanguage.BodyHtml = "";
                    documentLanguage.Organizations = "";
                    documentLanguage.PageTitle = "";

                    if (document.English().PageImage != null && document.English().PageImage.Languages.Select(e => e.Language).Contains(documentLanguage.Language))
                        documentLanguage.PageImage = document.English().PageImage;

                    document.Languages.Add(documentLanguage);
                }
            }

            var releaseLanguage = documentLanguage.Document.Release.Languages.SingleOrDefault(e => e.Language.Id == documentLanguage.Language.Id);

            if (releaseLanguage == null)
            {
                //TODO: Add Summary to all Document.aspx views

                releaseLanguage = new NewsReleaseLanguage();

                releaseLanguage.Language = db.Languages.Find(languageId);
                releaseLanguage.Summary = "";
                releaseLanguage.Location = "";

                Release.Languages.Add(releaseLanguage);
            }
            return documentLanguage;
        }

        public Dictionary<string, PageLayout> DefaultPageLayouts
        {
            get
            {
                var types = from nrt in db.NewsReleaseTypes
                            where nrt.LanguageId == Language.enCA
                            select new { PageTitle = nrt.PageTitle, PageLayout = (PageLayout)nrt.PageLayout };

                return types.ToDictionary(e => e.PageTitle, e => e.PageLayout);
            }
        }

        public NewsReleaseDocumentLanguage FirstEnglishDocument
        {
            get { return Release.Documents.OrderBy(e => e.SortIndex).First().English(); }
        }

        public string Key
        {
            get { return Release.Key; }
            set
            {
                //TODO: Consider moving this code to the Save method instead of executing it in this setter.
                if (Release.Key != value)
                {
                    Release.Key = value;

                    int index = 0;
                    while (db.NewsReleases.Any(e => e.Key == Release.Key))
                    {
                        index++;
                        Release.Key = value + "-" + index.ToString();
                    }
                }
            }
        }

        public static string GenerateSlug(string phrase)
        {
            string str = phrase;
            if (phrase != null)
            {
                str = RemoveAccent(str).ToLower();
                //hyphen
                str = str.Replace("&#8211;", "-");
                //two different kinds of apostrophes
                str = str.Replace("&#8216;", "");
                str = str.Replace("&#8217;", "");
                // two different kinds of double quotes
                str = str.Replace("&#8220;", "");
                str = str.Replace("&#8221;", "");
                // e with acute accent
                str = str.Replace("&#201;", "e");
                str = str.Replace(" - ", " ");

                // invalid chars
                str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
                // convert multiple spaces into one space
                str = Regex.Replace(str, @"\s+", " ").Trim();
                // cut and trim 
                str = str.Substring(0, str.Length <= 100 ? str.Length : 100).Trim();
                str = Regex.Replace(str, @"\s", "-"); // hyphens
            }
            return str;
        }

        public static string RemoveAccent(string txt)
        {
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }


        public byte[] GetTextDocument()
        {
            Gcpe.News.ReleaseManagement.Templates.Release template = Gcpe.News.ReleaseManagement.Templates.Release.FromEntity(Release);
            return template.ToTextDocument();
        }

        private List<System.Web.UI.WebControls.ListItem> attachments;
        public List<System.Web.UI.WebControls.ListItem> Attachments
        {
            get
            {
                if (attachments == null)
                {
                    attachments = new List<System.Web.UI.WebControls.ListItem>();

                    var wordDocument = Release.History.OrderByDescending(e => e.PublishDateTime).FirstOrDefault(e => e.MimeType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                    if (wordDocument != null)
                    {
                        if (!Release.Logs.Any(e => (e.Description.StartsWith("Published ") || e.Description.StartsWith("Republished ")) && e.DateTime >= wordDocument.PublishDateTime))
                        {
                            string url = System.Web.VirtualPathUtility.ToAbsolute(String.Format("~/News/ReleaseManagement/ReleaseHistoryFile.ashx?BlobId={0}&MimeType={1}", HubModel.EncodeGuid(wordDocument.BlobId), HttpUtility.UrlEncode(wordDocument.MimeType)));

                            attachments.Add(new System.Web.UI.WebControls.ListItem()
                            {
                                Text = Release.Reference + ".docx",
                                Value = url
                            });
                        }
                    }
                }

                return attachments;
            }
        }

        public static bool ValidateUrls(string bodyHtml)
        {
            if (string.IsNullOrWhiteSpace(bodyHtml))
                return true;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(bodyHtml);
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]") ?? Enumerable.Empty<HtmlNode>())
            {
                string href = link.GetAttributeValue("href", string.Empty);
                Uri uri;
                var schemes = new string[] { "http", "https", "mailto", "ftp" };
                if (!Uri.TryCreate(href, UriKind.RelativeOrAbsolute, out uri) || 
                   (uri.IsAbsoluteUri && !schemes.Contains(uri.Scheme)) ||
                   (!uri.IsAbsoluteUri && !uri.OriginalString.StartsWith("#")))
                    return false;
            }
            return true;
        }

        public IEnumerable<string> SelectedMediaDistributionLists()
        {
            return SelectedMediaDistributionLists(Release);
        }

        public static IEnumerable<string> SelectedMediaDistributionLists(NewsRelease post)
        {
            return post.MediaDistributionLists.Select(e => "media-distribution-lists/" + e.Key);
        }

        public IList<string> SelectedNodDistributionLists()
        {
            return SelectedNodDistributionLists(Release);
        }

        public static List<string> SelectedNodDistributionLists(NewsRelease post)
        {
            List<string> distributionLists = new List<string>();
            if (post.ReleaseType != ReleaseType.Update && post.ReleaseType != ReleaseType.Advisory)
            {
                distributionLists.AddRange(post.Ministries.Select(e => "ministries/" + e.Key));
                distributionLists.AddRange(post.Sectors.Select(e => "sectors/" + e.Key));
                distributionLists.AddRange(post.Tags.Select(e => "tags/" + e.Key));
            }
            return distributionLists;
        }

        public IEnumerable<string> EnglishLocations (HubEntities dbContext)
        {
            DateTime cutOff = DateTime.Today.AddYears(-5);
            var locations = from nrl in dbContext.NewsReleaseLanguages
                            where nrl.LanguageId == 4105 && nrl.Release.IsActive &&
                            nrl.Release.PublishDateTime > cutOff && nrl.Location != ""
                            group nrl by nrl.Location into g
                            select new { Location = g.Key, Count = g.Count() };
            return locations.OrderByDescending(e => e.Count).ThenBy(e => e.Location).Select(e => e.Location);
        }
    }
}