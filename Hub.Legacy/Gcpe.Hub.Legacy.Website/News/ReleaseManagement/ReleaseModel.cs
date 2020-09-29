extern alias legacy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Gcpe.Hub.Properties;
using legacy::Gcpe.Hub.Data.Entity;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Gcpe.Hub.News.ReleaseManagement
{
    public class ReleaseModel : ReleaseManagementModel
    {
        public bool IsNew
        {
            get { return false; }
        }

        public string CollectionName
        {
            get { return Release.Collection.Name; }
        }

        public string ReleaseTypeName
        {
            get { return Enum.GetName(typeof(ReleaseType), Release.ReleaseType); }
        }

        public ReleaseType ReleaseTypeId
        {
            get { return Release.ReleaseType; }
        }

        public bool AllowPublishToNewsOnDemand
        {
            get
            {
                bool result = true;
                // Updates & Advisories *cannot* go out to News On Demand.
                if (Release.ReleaseType == ReleaseType.Update || Release.ReleaseType == ReleaseType.Advisory)
                {
                    result = false;
                }
                return result;
            }
        }

        public bool AllowPublishToMediaAdvisory
        {
            get
            {
                // TODO: Sort out which ones *can* go out to the Media Distribution Lists
                return (Release.ReleaseType == ReleaseType.Advisory); // Etc...
            }
        }

        public Guid Id
        {
            get { return Release.Id; }
        }

        public DateTimeOffset? PublishDateTime
        {
            get { return Release.PublishDateTime; }
            set { Release.PublishDateTime = value; }
        }

        public string FirstHeadline
        {
            get { return FirstEnglishDocument.Headline; }
        }

        public bool IsKeyEditable
        {
            get { return !IsPublished && !IsCommitted && (Release.ReleaseType == ReleaseType.Story || Release.ReleaseType == ReleaseType.Factsheet); }
        }

        public Uri ReleaseUri
        {
            get { return GetReleaseUri(Release); }
        }
        private static Uri GetReleaseUri(NewsRelease post)
        {
            //TODO: get it from NewRooms
            return new Uri(Settings.Default.NewsHostUri, post.ReleasePathName() + "/" + post.Key);
        }

        public string PermanentUri
        {
            get { return GetPermanentUri(Release); }
        }

        public static string GetPermanentUri(NewsRelease post)
        {
            if (post.ReleaseType == ReleaseType.Advisory)
                return "advisories/" + post.Key;
            if (post.Reference.StartsWith("NEWS-"))
                return Settings.Default.NewsHostUri + post.Reference.Substring("NEWS-".Length);
            return GetReleaseUri(post).ToString();
        }

        public string ReleasePathName
        {
            get { return Release.ReleasePathName(); }
        }

        public string Reference
        {
            get
            {
                //TODO: Get Reference (used in ReleaseModel and DocumentModel)
                return Release.Reference ?? DateTime.Now.Year.ToString() + "DRAFT";
            }
        }
        public string FindLanguageName(int languageId)
        {
            return db.Languages.Find(languageId).Name;
        }

        public NewsReleaseImage FindImage(Guid id)
        {

            return db.NewsReleaseImages.Find(id);
        }

        public IEnumerable<int> AddLanguages(ICollection<NewsReleaseDocumentLanguage> languagesPresent)
        {
            List<int> languages = new List<int>();

            foreach (var language in db.Languages.OrderBy(e => e.SortOrder).ThenBy(e => e.Name))
            {
                if (!languagesPresent.Any(e => e.Language.Id == language.Id))
                    languages.Add(language.Id);
            }

            return languages.AsReadOnly();
        }

        public Dictionary<string, int> DefaultPageTitles(int languageId)
        {
            var releaseTypes =
                from nrt in db.NewsReleaseTypes
                join nrdl in db.NewsReleaseDocumentLanguages on nrt.PageTitle equals nrdl.PageTitle into g
                from nrdlg in g.DefaultIfEmpty()
                where nrt.LanguageId == languageId
                && nrt.PageTitle != "Media Advisory - EVENT REMINDER" // (task 3576) this table doesn't have an IsActive flag in the database, so we're removing it this way
                group nrt.ReleaseType by nrt.PageTitle into r
                orderby r.Count() descending
                select new { PageTitle = r.Key, ReleaseType = r.Distinct().FirstOrDefault() };

            return new Dictionary<string, int>(releaseTypes.ToDictionary(rt => rt.PageTitle, rt => rt.ReleaseType));
        }

        public NewsReleaseType CreateNewsReleaseTypeIfNeeded(string pageTitle, Language language)
        {
            var newReleaseType = db.NewsReleaseTypes.SingleOrDefault(e => e.PageTitle == pageTitle && e.LanguageId == language.Id);
            if (newReleaseType != null)
            {
                //already exists, no need to create it
                return null;
            }
            newReleaseType = new NewsReleaseType();
            newReleaseType.ReleaseType = (int)Release.ReleaseType;
            newReleaseType.PageTitle = pageTitle;
            newReleaseType.Language = language;
            newReleaseType.SortOrder = 0;

            db.NewsReleaseTypes.Add(newReleaseType);
            return newReleaseType;
        }


        public void AddDocumentLanguagePlaceHolder(int sortIndex, int languageId)
        {
            DocumentLanguage(sortIndex, languageId);
        }

        public void DeleteDocumentLanguage(NewsReleaseDocumentLanguage documentLanguage)
        {
            if (documentLanguage.Language.Id == Language.enCA)
            {
                documentLanguage.Document.SortIndex = Int32.MaxValue;
                db.NewsReleaseDocuments.Remove(documentLanguage.Document);

                int sortIndex = 0;
                foreach (var releaseDocument in Release.Documents)
                    releaseDocument.SortIndex = sortIndex++;
            }
            else
            {
                db.NewsReleaseDocumentLanguages.Remove(documentLanguage);
            }

            base.Save();
        }


        public string Location
        {
            get { return Release.English().Location; }
            set { Release.English().Location = value; }
        }

        public Guid? MinistryId
        {
            get { return Release.MinistryId; }
            set { Release.Ministry = db.Ministries.Find(value.Value); }
        }

        public string HomeTopReleaseId
        {
            get { return db.GetAppSetting("HomeTopReleaseId"); }
            set { db.SetAppSetting("HomeTopReleaseId", value); }
        }
        public string HomeFeatureReleaseId
        {
            get { return db.GetAppSetting("HomeFeatureReleaseId"); }
            set { db.SetAppSetting("HomeFeatureReleaseId", value); }
        }

        public bool PublishNewsOnDemand
        {
            get
            {
                return Release.IsPublishNewsOnDemand();
            }
            set
            {
                if (value)
                {
                    Release.PublishOptions |= PublishOptions.PublishNewsOnDemand;
                }
                else
                {
                    Release.PublishOptions &= ~PublishOptions.PublishNewsOnDemand;
                }
            }
        }

        public bool PublishNewsArchives
        {
            get
            {
                return Release.ReleaseType != ReleaseType.Advisory; // for backwards compatibility reasons (Ref <= NEWS-09145)
            }
            set
            {
                if (value)
                {
                    Release.PublishOptions |= PublishOptions.PublishNewsArchives;
                }
                else
                {
                    Release.PublishOptions &= ~PublishOptions.PublishNewsArchives;
                }
            }
        }

        public bool PublishMediaContacts
        {
            get
            {
                return Release.IsPublishMediaContacts();
            }
            set
            {
                if (value)
                {
                    Release.PublishOptions |= PublishOptions.PublishMediaContacts;
                }
                else
                {
                    Release.PublishOptions &= ~PublishOptions.PublishMediaContacts;
                }
            }
        }

        public string NodSubscriberCount
        {
            get
            {
                return Release.NodSubscribers.HasValue ? "(" + Release.NodSubscribers + ")" : string.Empty;
            }
        }

        public string MediaSubscriberCount
        {
            get
            {
                return Release.MediaSubscribers.HasValue ? "(" + Release.MediaSubscribers + ")" : string.Empty;
            }
        }


        //public bool RssPublishFeeds
        //{
        //    get
        //    {
        //        RssOptions publishFeeds = RssOptions.PublishMinistries | RssOptions.PublishSectors;

        //        return newsRelease.RssOptions.HasFlag(publishFeeds);
        //    }
        //    set
        //    {
        //        RssOptions publishFeeds = RssOptions.PublishMinistries | RssOptions.PublishSectors;

        //        if (value)
        //        {
        //            newsRelease.RssOptions |= publishFeeds;
        //        }
        //        else
        //        {
        //            newsRelease.RssOptions &= ~publishFeeds;
        //        }
        //    }
        //}

        private List<CategoryItem<Guid>> ministries;
        public IEnumerable<CategoryItem<Guid>> Ministries
        {
            get
            {
                if (ministries == null)
                {
                    ministries = new List<CategoryItem<Guid>>();

                    var englishMinistries = from rl in db.MinistryLanguages where rl.LanguageId == 4105 orderby rl.Ministry.SortOrder, rl.Name select rl;

                    foreach (var englishMinistry in englishMinistries)
                    {
                        bool isSelected = Release.Ministries.Contains(englishMinistry.Ministry);
                        if (englishMinistry.Ministry.IsActive || isSelected)
                        {
                            ministries.Add(new CategoryItem<Guid>()
                            {
                                Text = englishMinistry.Name,
                                Value = englishMinistry.MinistryId,
                                Selected = isSelected,
                                TopReleaseId = englishMinistry.Ministry.TopReleaseId,
                                FeatureReleaseId = englishMinistry.Ministry.FeatureReleaseId
                            });
                        }
                    }
                }

                return ministries.AsReadOnly();
            }
        }

        private List<CategoryItem<Guid>> sectors;
        public IEnumerable<CategoryItem<Guid>> Sectors
        {
            get
            {
                if (sectors == null)
                {
                    sectors = new List<CategoryItem<Guid>>();

                    var englishSectors = from rl in db.SectorLanguages where rl.LanguageId == 4105 orderby rl.Sector.SortOrder, rl.Name select rl;
                    foreach (var englishSector in englishSectors)
                    {
                        bool isSelected = Release.Sectors.Contains(englishSector.Sector);
                        if (englishSector.Sector.IsActive || isSelected)
                        {
                            sectors.Add(new CategoryItem<Guid>()
                            {
                                Text = englishSector.Name,
                                Value = englishSector.SectorId,
                                Selected = isSelected,
                                TopReleaseId = englishSector.Sector.TopReleaseId,
                                FeatureReleaseId = englishSector.Sector.FeatureReleaseId
                            });
                        }
                    }
                }

                return sectors.AsReadOnly();
            }
        }

        private List<CategoryItem<Guid>> themes;
        public IEnumerable<CategoryItem<Guid>> Themes
        {
            get
            {
                if (themes == null)
                {
                    themes = new List<CategoryItem<Guid>>();

                    var dbThemes = from t in db.Themes orderby t.SortOrder, t.DisplayName select t;
                    foreach (var dbTheme in dbThemes)
                    {
                        bool isSelected = Release.Themes.Contains(dbTheme);
                        if (dbTheme.IsActive || isSelected)
                        {
                            themes.Add(new CategoryItem<Guid>()
                            {
                                Text = dbTheme.DisplayName,
                                Value = dbTheme.Id,
                                Selected = isSelected,
                                TopReleaseId = dbTheme.TopReleaseId,
                                FeatureReleaseId = dbTheme.FeatureReleaseId
                            });
                        }
                    }
                }

                return themes.AsReadOnly();
            }
        }

        private List<ListItem<Guid>> mediaDistributionLists;
        public IEnumerable<ListItem<Guid>> MediaDistributionLists
        {
            get
            {
                if (mediaDistributionLists == null)
                {
                    mediaDistributionLists = new List<ListItem<Guid>>();

                    var dbMediaLists = from mdl in db.MediaDistributionLists
                                       orderby mdl.SortOrder
                                       select mdl;
                    foreach (var ml in dbMediaLists)
                    {
                        bool isSelected = Release.MediaDistributionLists.Contains(ml);
                        if (ml.IsActive || isSelected)
                        {
                            mediaDistributionLists.Add(new ListItem<Guid>() { Text = ml.DisplayName, Value = ml.Id, Selected = isSelected });
                        }
                    }
                }
                return mediaDistributionLists.AsReadOnly();
            }
        }

        private List<ListItem<Guid>> tags;
        public IEnumerable<ListItem<Guid>> Tags
        {
            get
            {
                if (tags == null)
                {
                    tags = new List<ListItem<Guid>>();

                    var dbTags = from t in db.Tags orderby t.SortOrder, t.DisplayName select t;
                    foreach (var tag in dbTags)
                    {
                        bool isSelected = Release.Tags.Contains(tag);
                        if (tag.IsActive || isSelected)
                            tags.Add(new ListItem<Guid>() { Text = tag.DisplayName, Value = tag.Id, Selected = isSelected });
                    }
                }

                return tags.AsReadOnly();
            }
        }

        public string Keywords
        {
            get { return Release.Keywords; }
            set { Release.Keywords = value; }
        }

        public int? ActivityId
        {
            get { return Release.ActivityId; }
            set { Release.ActivityId = value; }
        }

        public bool HasMediaAssets
        {
            get { return Release.HasMediaAssets; }
            set { Release.HasMediaAssets = value; }
        }

        private List<string> mediaAssets;
        public IEnumerable<string> MediaAssets
        {
            get
            {
                if (mediaAssets == null)
                {
                    mediaAssets = new List<string>();

#if !LOCAL_MEDIA_STORAGE
                    var container = new CloudBlobContainer(Global.ModifyContainerWithSharedAccessSignature("assets"));

                    var directory = container.GetDirectoryReference(ReleasePathName);
                    IEnumerable<CloudBlockBlob> blobs = directory.GetDirectoryReference(Release.Key.ToLower()).ListBlobs().OfType<CloudBlockBlob>();
                    if (blobs.Count() == 0)
                    {
                        blobs = directory.GetDirectoryReference(Release.Key).ListBlobs().OfType<CloudBlockBlob>();
                    }

                    foreach (CloudBlockBlob blob in blobs)
                    {
                        string fileName;
                        blob.FetchAttributes();
                        mediaAssets.Add(blob.Metadata.TryGetValue("filename", out fileName) ? fileName : Path.GetFileName(blob.Name));
                    }
#else

                    if (!String.IsNullOrEmpty(Settings.Default.MediaAssetsUnc))
                    {
                        string directory = Path.Combine(Settings.Default.MediaAssetsUnc, ReleasePathName, Release.Key);

                        if (Directory.Exists(directory))
                        {
                            foreach (string file in Directory.GetFiles(directory))
                            {
                                string key = "/" + Release.Key + "/" + Path.GetFileName(file);
                                mediaAssets.Add(Path.GetFileName(file));
                            }
                        }
                    }
#endif
                }
                return mediaAssets;
            }
        }

        public string RequiredTranslations()
        {
            var rvl = "";

            if (Release.ActivityId.HasValue)
            {
                rvl = db.Database.SqlQuery<string>(@"select Translations from calendar.activity where Id=@Id",
                    new object[] { new SqlParameter("@Id", Release.ActivityId) }).FirstOrDefault();
            }

            return rvl;

        }

        public bool HasTranslations
        {
            get { return Release.HasTranslations; }
            set { Release.HasTranslations = value; }
        }

        private List<string> translations;
        public IEnumerable<string> Translations
        {
            get
            {
                if (translations == null)
                {
                    translations = new List<string>();

#if !LOCAL_MEDIA_STORAGE
                    var container = new CloudBlobContainer(Global.ModifyContainerWithSharedAccessSignature("translations"));

                    var directory = container.GetDirectoryReference(ReleasePathName);
                    IEnumerable<CloudBlockBlob> blobs = directory.GetDirectoryReference(Release.Key.ToLower()).ListBlobs().OfType<CloudBlockBlob>();
                    if (blobs.Count() == 0)
                    {
                        blobs = directory.GetDirectoryReference(Release.Key).ListBlobs().OfType<CloudBlockBlob>();
                    }

                    foreach (CloudBlockBlob blob in blobs)
                    {
                        string fileName;
                        blob.FetchAttributes();
                        translations.Add(blob.Metadata.TryGetValue("filename", out fileName) ? fileName : Path.GetFileName(blob.Name));
                    }
#else

                    if (!String.IsNullOrEmpty(Settings.Default.MediaAssetsUnc))
                    {
                        string directory = Path.Combine(Settings.Default.MediaAssetsUnc, ReleasePathName, Release.Key);

                        if (Directory.Exists(directory))
                        {
                            foreach (string file in Directory.GetFiles(directory))
                            {
                                string key = "/" + Release.Key + "/" + Path.GetFileName(file);
                                mediaAssets.Add(Path.GetFileName(file));
                            }
                        }
                    }
#endif
                }
                return translations;
            }
        }

        public Uri Asset
        {
            get { return Release.AssetUrl == "" ? null : new Uri(Release.AssetUrl); }
            set { Release.AssetUrl = value == null ? "" : value.ToString(); }
        }

        public string Summary
        {
            get { return Release.English().Summary; }
            set { Release.English().Summary = value; }
        }

        public string SocialMediaSummary
        {
            get { return Release.English().SocialMediaSummary; }
            set { Release.English().SocialMediaSummary = value; }
        }

        public string SocialMediaHeadline
        {
            get { return Release.English().SocialMediaHeadline; }
            set { Release.English().SocialMediaHeadline = value; }
        }

        public string LocationSummary
        {
            get
            {
                return Location.ToUpper() + (Location == "" ? "" : " – ") + Summary;
            }
        }

        public bool IsActive
        {
            get { return Release.IsActive; }
        }

        public void ReorderDocuments(int source, int target)
        { // insertBefore target
            var documents = Release.Documents.OrderBy(e => e.SortIndex);
            var sourceDoc = documents.ElementAt(source);

            if (target <= source)
            {
                //Moving up: update the items after the target
                for (int i = source - 1; i >= target; i--)
                    documents.ElementAt(i).SortIndex++;
            }
            else
            {
                //Moving down: update the items after the source
                for (int i = source + 1; i <= target; i++)
                    documents.ElementAt(i).SortIndex--;
            }
            sourceDoc.SortIndex = target; // do this last because it reorders our documents variable
            base.Save();
        }

        public void AddDocumentLanguages(NewsReleaseDocument document, List<ListItem> documents)
        {
            foreach (NewsReleaseDocumentLanguage documentLanguage in document.Languages.OrderBy(e => e.Language.SortOrder).ThenBy(e => e.Language.Name))
            {
                ListItem item = new ListItem();
                //TODO: Consider item.Text = document.English().PageTitle + ": " + document.English().Headline;
                item.Text = document.English().PageTitle;
                if (item.Text == "") item.Text = "New Document";

                if (documentLanguage.Language.Id != db.English().Id)
                    item.Text += " (" + documentLanguage.Language.Name + ")";

                item.Value = (document.SortIndex + 1).ToString() + (documentLanguage.Language.Id == db.English().Id ? "" : "/" + (CultureInfo.GetCultureInfo(documentLanguage.Language.Id).Name));

                documents.Add(item);
            }

        }

        public void AddDocumentLanguages(Guid documentId, List<ListItem> documents)
        {
            AddDocumentLanguages(Release.Documents.SingleOrDefault(d => d.Id == documentId), documents);
        }

        public IEnumerable<ListItem> Documents
        {
            get
            {
                List<ListItem> documents = new List<ListItem>();

                foreach (NewsReleaseDocument document in Release.Documents.OrderBy(e => e.SortIndex))
                {
                    AddDocumentLanguages(document, documents);
                }

                return documents;
            }
        }
        public IEnumerable<Guid> DocumentIds
        {
            get
            {
                return Release.Documents.OrderBy(e => e.SortIndex).Select(e => e.Id);
            }
        }

        public IEnumerable<NewsReleaseDocument> NewsReleaseDocuments
        {
            get
            {
                return Release.Documents.OrderBy(e => e.SortIndex);
            }
        }

        public bool ShowVerboseHistory { get; set; }

        public int HiddenHistoryCount
        {
            get
            {
                int count = 0;

                List<HistoryHtml> history = new List<HistoryHtml>();

                bool isReleased = false;

                foreach (var log in Release.Logs.OrderBy(l => l.DateTime))
                {
                    string userHtml = HttpUtility.HtmlEncode(log.User == null ? "News Release Management" : log.User.DisplayName + " ");
                    string descriptionHtml = HttpUtility.HtmlEncode(log.Description);

                    bool showHistory = false;

                    if (ShowVerboseHistory)
                    {
                        showHistory = true;
                    }
                    else if (isReleased)
                    {
                        showHistory = true;
                    }
                    else if (log.Description.StartsWith("Created "))
                    {
                        showHistory = true;
                    }
                    else if (log.Description.StartsWith("Approved "))
                    {
                        showHistory = true;
                    }
                    else if (log.Description.StartsWith("Released "))
                    {
                        isReleased = true;
                        showHistory = true;
                    }

                    if (!showHistory)
                        count++;
                }

                return count;
            }
        }

        private bool displayTranslationsAnchorPanel = false;
        public bool DisplayTranslationsAnchorPanel
        {
            get { return displayTranslationsAnchorPanel; }
            set { displayTranslationsAnchorPanel = value; }
        }

        public List<NewsReleaseLog> MostRecentLogMessages
        {
            get { return Release.Logs.OrderBy(l => l.DateTime).ToList(); }
        }

        public IEnumerable<HistoryHtml> History
        {
            get
            {
                List<HistoryHtml> history = new List<HistoryHtml>();

                bool isReleased = false;

                foreach (var log in Release.Logs.OrderBy(l => l.DateTime))
                {
                    string userHtml = HttpUtility.HtmlEncode(log.User == null ? "News Release Management" : log.User.DisplayName + " ");
                    string descriptionHtml = HttpUtility.HtmlEncode(log.Description);

                    bool showHistory = false;

                    if (ShowVerboseHistory)
                    {
                        showHistory = true;
                    }
                    else if (isReleased)
                    {
                        showHistory = true;
                    }
                    else if (log.Description.StartsWith("Created "))
                    {
                        showHistory = true;
                    }
                    else if (log.Description.StartsWith("Approved "))
                    {
                        showHistory = true;
                    }
                    else if (log.Description.StartsWith("Released ") || log.Description == "Published")
                    {
                        isReleased = true;
                        showHistory = true;
                    }

                    if (showHistory)
                        history.Insert(0, new HistoryHtml() { DateTime = log.DateTime, UserHtml = userHtml, DescriptionHtml = descriptionHtml });
                }

                //foreach (var hist in release.History.OrderBy(h => h.PublishDateTime))
                //{
                //    string url = String.Format("/News/ReleaseManagement/ReleaseHistoryFile.ashx?BlobId={0}&MimeType={1}", Page.EncodeGuid(hist.BlobId), HttpUtility.UrlEncode(hist.MimeType));
                //    history.Insert(0, new HistoryHtml() { DateTime = hist.PublishDateTime, DescriptionHtml = @"Migrated <a href=""" + url + @""">Word Document</a>" });
                //}

                return history;
            }
        }

        public int DocumentCount
        {
            get { return Release.Documents.Count; }
        }

        public ReleaseModel(Guid id)
        {
            Release = db.NewsReleases.Find(id);
        }

        public ReleaseModel(string reference)
        {
            Release = db.NewsReleases.Single(e => e.Reference == reference);
        }

        public class HistoryHtml
        {
            public DateTimeOffset DateTime { get; set; }

            public string UserHtml { get; set; }

            public string DescriptionHtml { get; set; }
        }

        public void Approve()
        {
            if (!string.IsNullOrEmpty(Release.Reference))
            {
                throw new HubModelException(new string[] { "This has already been approved." });
            }
            if (Release.Ministry == null)
            {
                var singleMinistryItem = Ministries.SingleOrDefault(e => e.Selected);

                if (singleMinistryItem != null)
                {
                    Release.MinistryId = singleMinistryItem.Value;
                    Release.Ministry = db.Ministries.Find(singleMinistryItem.Value);
                }
            }

            if (Release.ReleaseType != ReleaseType.Advisory)
            {
                // Advisories are allowed to have no Ministry, everything else *must* have a Ministry.
                Debug.Assert(Ministries.Any(e => e.Selected && e.Value == MinistryId));
            }

            if (!IsKeyEditable)
            {
                int year = DateTime.Today.Year;
                int nextSerialForYear = (db.NewsReleases.Any(e => e.Year == year) ? db.NewsReleases.Where(e => e.Year == year).Max(e => e.YearRelease.Value) : 0) + 1;
                int nextSerialForMinistry = (db.NewsReleases.Any(e => e.Year == year && e.MinistryId == MinistryId) ? db.NewsReleases.Where(e => e.Year == year && e.MinistryId == MinistryId).Max(e => e.MinistryRelease.Value) : 0) + 1;

                Release.Year = year;
                Release.YearRelease = nextSerialForYear;
                Release.MinistryRelease = nextSerialForMinistry;

                Release.Key = string.Format("{0}{1}{2:0000}-{3:000000}", year, Release.Ministry?.Abbreviation ?? "ADVIS", nextSerialForMinistry, nextSerialForYear);
            }
            string currentSerial = db.NewsReleases.Where(e => e.Reference.StartsWith("NEWS-")).Max(e => e.Reference);

            Release.Reference = string.Format("NEWS-{0:00000}", string.IsNullOrWhiteSpace(currentSerial) ? 1 : int.Parse(currentSerial.Substring(5)) + 1);
            try
            {
                SaveWithLog("Approved " + ReleaseTypeName); /* NewsReleaseLog.Description */
            }
            catch
            {
                Release.Year = null;
                Release.YearRelease = null;
                Release.MinistryRelease = null;

                Release.Reference = string.Empty;

                throw;
            }
        }

        private static ListItem<Guid> ApplyCategoryChanges<T>(IEnumerable<ListItem<Guid>> items, T cat, Guid catId, ICollection<T> releaseCategories, ref bool changed)
        {
            bool wasSelected = releaseCategories.Contains(cat);
            ListItem<Guid> newSelectedItem = items.FirstOrDefault(m => m.Value == catId && m.Selected);
            bool isSelected = newSelectedItem != null;
            if (isSelected != wasSelected)
            {
                changed |= true;
                if (wasSelected) releaseCategories.Remove(cat);
                else releaseCategories.Add(cat);
            }
            return newSelectedItem;
        }

        private void ProcessForSave()
        {
            if (db.Entry(Release).State == EntityState.Deleted)
                return;

            bool categoryChange = false;
            foreach (var ministry in db.Ministries.Where(e => e.IsActive))
            {
                var item = (CategoryItem<Guid>)ApplyCategoryChanges(Ministries, ministry, ministry.Id, Release.Ministries, ref categoryChange);
                if (item != null && (ministry.TopReleaseId != item.TopReleaseId || ministry.FeatureReleaseId != item.FeatureReleaseId))
                {
                    ministry.TopReleaseId = item.TopReleaseId;
                    ministry.FeatureReleaseId = item.FeatureReleaseId;
                    ministry.Timestamp = DateTime.Now;
                }
            }

            foreach (var sector in db.Sectors.Where(e => e.IsActive))
            {
                var item = (CategoryItem<Guid>)ApplyCategoryChanges(Sectors, sector, sector.Id, Release.Sectors, ref categoryChange);
                if (item != null && (sector.TopReleaseId != item.TopReleaseId || sector.FeatureReleaseId != item.FeatureReleaseId))
                {
                    sector.TopReleaseId = item.TopReleaseId;
                    sector.FeatureReleaseId = item.FeatureReleaseId;
                    sector.Timestamp = DateTime.Now;
                }
            }

            foreach (var tag in db.Tags.Where(e => e.IsActive))
            {
                ApplyCategoryChanges(Tags, tag, tag.Id, Release.Tags, ref categoryChange);
            }

            if (categoryChange)
            {
                Release.NodSubscribers = ReleasePublisher.NewsReleaseSubscriberCount(SelectedNodDistributionLists());
            }

            foreach (var theme in db.Themes.Where(e => e.IsActive))
            {
                var item = (CategoryItem<Guid>)ApplyCategoryChanges(Themes, theme, theme.Id, Release.Themes, ref categoryChange);
                if (item != null && (theme.TopReleaseId != item.TopReleaseId || theme.FeatureReleaseId != item.FeatureReleaseId))
                {
                    theme.TopReleaseId = item.TopReleaseId;
                    theme.FeatureReleaseId = item.FeatureReleaseId;
                    theme.Timestamp = DateTime.Now;
                }
            }

            bool mdlChange = false;
            foreach (var mdl in db.MediaDistributionLists.Where(e => e.IsActive))
            {
                ApplyCategoryChanges(MediaDistributionLists, mdl, mdl.Id, Release.MediaDistributionLists, ref mdlChange);
            }
            if (mdlChange)
            {
                var selectedMediaDistributionLists = SelectedMediaDistributionLists().ToList();
                Release.MediaSubscribers = ReleasePublisher.NewsReleaseSubscriberCount(selectedMediaDistributionLists);
                PublishMediaContacts = selectedMediaDistributionLists.Any();
            }

            // Advisories never go to NoD!
            if (Release.ReleaseType == ReleaseType.Advisory)
            {
                PublishNewsOnDemand = false;
            }

            if (CanEditDocuments)
            {
                //TODO: Find "lead paragraph to populate RSS Summary field
                //Save the Summary field so that it can be customized; it may be overwritten when the first document is saved (Document.btnSaveDocument_Click)
                Release.English().Summary = Summary;
            }
        }

        public override void Save()
        {
            ProcessForSave();

            base.Save();
        }

        public override void SaveWithLog(string logDescription)
        {
            ProcessForSave();

            base.SaveWithLog(logDescription);
        }

        public void Delete()
        {
            if (!String.IsNullOrEmpty(Release.Reference) || Release.History.Any())
            {
                Debug.Assert(Release.IsActive);
                Release.IsActive = false;

                SaveWithLog("Deleted Release"); /* NewsReleaseLog.Description */
            }
            else
            {
                db.NewsReleases.Remove(Release);

                Save();
            }
        }

        public void Publish()
        {
            Debug.Assert(!IsCommitted);
            if (IsScheduled)
            {
                IsCommitted = true;
                SaveWithLog("Scheduled for Release on " + PublishDateTime.Value.ToString("MMMM d, yyyy") + " at " + PublishDateTime.Value.ToString("h:mm tt").ToLower()); /* NewsReleaseLog.Description */
            }
            else
            {
                var now = DateTimeOffset.Now;
                if (PublishDateTime.HasValue && (DateTimeOffset.Now - PublishDateTime.Value).TotalMinutes > 5)
                {
                    throw new HubModelException(new string[] { "The publish date cannot be set in the past." });
                }

                if (string.IsNullOrEmpty(NodSubscriberCount))
                { // first time publish
                    if (Release.IsPublishNewsOnDemand())
                    {
                        Release.NodSubscribers = ReleasePublisher.NewsReleaseSubscriberCount(SelectedNodDistributionLists());
                    }
                    Release.MediaSubscribers = ReleasePublisher.NewsReleaseSubscriberCount(SelectedMediaDistributionLists().ToList());
                }

                PublishDateTime = new DateTimeOffset(now.Date.AddHours(now.TimeOfDay.Hours).AddMinutes(now.TimeOfDay.Minutes), now.Offset);
                IsCommitted = true;
                SaveWithLog("Scheduled for Immediate Release"); /* NewsReleaseLog.Description */
            }
            ReleasePublisher.UpdateNextNewsReleasePublishDate(Release);
        }

        public void Unpublish()
        {
            Debug.Assert(IsCommitted);
            IsCommitted = false;

            if (Release.ReleaseDateTime.HasValue)
            {
                Release.IsPublished = true;
                SaveWithLog("Unpublished Release"); /* NewsReleaseLog.Description */
            }
            else
            {
                SaveWithLog("Cancelled Release"); /* NewsReleaseLog.Description */
            }
            ReleasePublisher.UnpublishNewsRelease(PermanentUri);
        }

        public bool CanEdit
        {
            get { return true; }
        }

        public bool CanEditDocuments
        {
            get { return true; }
        }

        public bool CanDelete
        {
            get { return !Release.IsCommitted; }
        }

        public bool IsApproved
        {
            get
            {
                bool approved;
                if (Release.ReleaseType == ReleaseType.Advisory)
                {
                    // Advisories are Ministry-optional, we need to check the Reference number instead.
                    approved = !string.IsNullOrEmpty(Release.Reference);
                }
                else
                {
                    approved = Release.Ministry != null;
                }
                return approved;
            }
        }

        public bool IsCommitted
        {
            get { return Release.IsCommitted; }
            set { Release.IsCommitted = value; }
        }

        public bool IsPublished
        {
            get { return Release.IsPublished; }
        }

        public byte[] GetPortableDocument()
        {
            var releaseTemplate = Gcpe.News.ReleaseManagement.Templates.Release.FromEntity(Release);
            return releaseTemplate.ToPortableDocument();
        }

        public bool IsScheduled
        {
            get { return PublishDateTime.HasValue && PublishDateTime.Value > DateTimeOffset.Now; }
        }

        public DateTime? ReleaseDate
        {
            get { return Release.ReleaseDateTime; }
            set { Release.ReleaseDateTime = value; }
        }

        public string LeadOrganization { get { return Release.Ministry == null ? "" : Release.Ministry.English().Name; } }

        public static string DraftReference(string newsReference, Guid newsId) { return string.IsNullOrEmpty(newsReference) ? EncodeGuid(newsId) : newsReference; }

        public string SuperAssetInHtml()
        {
            if (Asset == null)
                return null;
            try
            {


                if (Asset.Host == "www.youtube.com")
                {
                    string videoId = null;
                    if (Asset.Host.Contains("youtube"))
                    {
                        var query = HttpUtility.ParseQueryString(Asset.Query);

                        videoId = query["v"];
                    }

                    var youtubeIframeUrl = String.Format("//www.youtube.com/embed/{0}?rel=0&amp;modestbranding=1", videoId);

                    return String.Format(
                                            "<div style=\"display:table-row;\">" +
                                            "<iframe src=\"{0}\" frameborder=\"0\" allowfullscreen style=\"width:400px;height:255px;margin:0 auto;\"></iframe>" +
                                            "</div>"
                                            , youtubeIframeUrl);

                }
                else if (Asset.Host.EndsWith("staticflickr.com"))
                {
                    return String.Format(
                                            "<div style=\"display:table-row;\">" +
                                            "<img src=\"{0}\" style=\"width:400px;height:255px;margin:0 auto;\"/>" +
                                            "</div>"
                                            , Asset.ToString());
                }
            }
            catch (UriFormatException)
            {
                return "<!--" + Asset.ToString() + "-->";
            }

            return "<a href=\"" + Asset.ToString() + "\">" + Asset.ToString() + "</a>";
        }

        public Uri RedirectUrl
        {
            get { return Release.RedirectUrl == "" ? null : new Uri(Release.RedirectUrl); }
            set { Release.RedirectUrl = value == null ? "" : value.ToString(); }
        }

        public string ReleaseStatus
        {
            get
            {
                string status;

                if (!PublishDateTime.HasValue)
                    status = IsApproved ? "Approved" : "Draft";
                else if (IsPublished)
                {
                    if (Release.ReleaseType == ReleaseType.Advisory) status = IsCommitted ? "Sent" : "Unscheduling...";
                    else status = IsCommitted ? "Published" : "Unpublishing...";
                }
                else if (!IsCommitted)
                    status = "Planned";
                else
                { // Committed but not Published
                    if (IsScheduled)
                        status = "Scheduled";
                    else
                        status = Release.ReleaseDateTime.HasValue ? "Republishing..." : "Publishing...";
                }

                //if (!IsPublished && release.ReleaseDate.HasValue)
                //    status = status + " (Unpublished)";

                return status;
            }
        }

        public static string FormatDateTime(DateTimeOffset date)
        {
            string text;

            System.Globalization.Calendar invariantCalendar = System.Globalization.CultureInfo.InvariantCulture.Calendar;
            var firstFullWeek = System.Globalization.CalendarWeekRule.FirstFullWeek;

            if (date.Date.Date == DateTime.Today.AddDays(-1d))
                text = "Yesterday";
            else if (date.Date.Date == DateTime.Today)
                text = "Today";
            else if (date.Date.Date == DateTime.Today.AddDays(1d))
                text = "Tomorrow";
            //else if (date.Date.Date > DateTime.Today && date.Date.Date < DateTime.Today.AddDays(7d))
            //    text = date.ToString("dddd") + ", " + date.ToString("MMMM d, yyyy");
            else if (date.Date.Date > DateTime.Today && invariantCalendar.GetWeekOfYear(date.Date, firstFullWeek, DayOfWeek.Sunday) == invariantCalendar.GetWeekOfYear(DateTime.Today, firstFullWeek, DayOfWeek.Sunday))
                text = date.ToString("dddd") + ", " + date.ToString("MMMM d, yyyy");
            //else if (date.Date.Date > DateTime.Today && invariantCalendar.GetWeekOfYear(date.Date, firstFullWeek, DayOfWeek.Sunday) == 1 + invariantCalendar.GetWeekOfYear(DateTime.Today, firstFullWeek, DayOfWeek.Sunday))
            //    text = "Next " + date.ToString("dddd") + ", " + date.ToString("MMMM d, yyyy");
            else
                text = date.ToString("MMMM d, yyyy");

            if (date.TimeOfDay != TimeSpan.Zero)
            {
                text += " at " + date.ToString("h:mm tt").ToLower();
            }

            return text;
        }

        public static string FormatPublishStatusDate(DateTimeOffset publishDateTime, bool isPublished, bool isCommitted)
        {
            if (isPublished)
                return "Published " + FormatDateTime(publishDateTime);
            if (isCommitted)
                return "Scheduled for " + FormatDateTime(publishDateTime);
            return "Planned for " + FormatDateTime(publishDateTime);
        }

        public static string ReleaseHubUrl(bool isPublished, bool isCommitted, string newsReference, Guid newsId)
        {
            string folder = "~/News/ReleaseManagement/" + (isPublished ? "Published/" : (isCommitted ? "Scheduled/" : "Drafts/"));
            return System.Web.VirtualPathUtility.ToAbsolute(folder + DraftReference(newsReference, newsId));
        }
        public static string ReleaseHubUrl(NewsRelease nr)
        {
            return ReleaseHubUrl(nr.IsPublished, nr.IsCommitted, nr.Reference, nr.Id);
        }
        public static string ReleaseColor(string releaseType)
        {
            switch (releaseType)
            {
                case "Release":
                    return "rgb(72, 88, 113)";
                case "Story":
                    return "rgb(113, 72, 73)";
                case "Update":
                    return "rgb(226, 222, 69)";
                case "Advisory":
                    return "rgb(226, 109, 90)";
                default:
                    return "rgb(92, 92, 92)";
            }
        }
        public static string ReleaseDocumentType(string releaseType, string pageTitle)
        {
            if (releaseType == "Story" && pageTitle == "Release") return "Entry";
            return (releaseType == "Release" && pageTitle == "News Release") || releaseType == pageTitle ? "" : releaseType.ToUpper() + ": ";
        }
    }
}
