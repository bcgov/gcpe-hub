extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using Gcpe.News.ReleaseManagement;

namespace Gcpe.Hub.News.ReleaseManagement
{
    using legacy::Gcpe.Hub.Data.Entity;

    public class NewModel : ReleaseManagementModel
    {
        //private readonly NewsRelease release;
        private readonly NewsReleaseLanguage englishRelease;
        private readonly NewsReleaseDocument document;
        private readonly NewsReleaseDocumentLanguage englishDocument;

        public bool IsNew { get { return true; } }

        public ReleaseType ReleaseType
        {
            get { return Release.ReleaseType; }
            set { Release.ReleaseType = value; }
        }

        public string PageTitle
        {
            get { return englishDocument.PageTitle; }
            set { englishDocument.PageTitle = value; }
        }

        public PageLayout PageLayout
        {
            get { return document.PageLayout; }
            set { document.PageLayout = value; }
        }

        public Guid? PageImageId
        {
            get { return englishDocument.PageImage == null ? (Guid?)null : englishDocument.PageImage.Id; }
            set { englishDocument.PageImage = value == null ? null : db.NewsReleaseImages.Find(value); }
        }

        //public IEnumerable<Guid> MinistryIds
        //{
        //    get
        //    {
        //        return release.Ministries.Select(e => e.Id);
        //    }
        //    set
        //    {
        //        release.Ministries.Clear();

        //        foreach (var id in value)
        //            release.Ministries.Add(db.Ministries.Find(id));
        //    }
        //}

        //public IEnumerable<Guid> SectorIds
        //{
        //    get
        //    {
        //        return release.Sectors.Select(e => e.Id);
        //    }
        //    set
        //    {
        //        release.Sectors.Clear();

        //        foreach (var id in value)
        //            release.Sectors.Add(db.Sectors.Find(id));
        //    }
        //}

        public string Organizations
        {
            get { return englishDocument.Organizations; }
            set { englishDocument.Organizations = value; }
        }

        public string Location
        {
            get { return englishRelease.Location; }
            set { englishRelease.Location = value; }
        }

        private List<ListItem<Guid>> ministries;
        public IEnumerable<ListItem<Guid>> Ministries
        {
            get
            {
                if (ministries == null)
                {
                    ministries = new List<ListItem<Guid>>();

                    //rl.Ministry.Id == MinistryId.Value

                    var englishMinistries = from rl in db.MinistryLanguages where rl.LanguageId == 4105 orderby rl.Ministry.SortOrder, rl.Name select rl;

                    foreach (var englishMinistry in englishMinistries)
                    {
                        bool isSelected = Release.Ministries.Contains(englishMinistry.Ministry);

                        if (englishMinistry.Ministry.IsActive || isSelected)
                            ministries.Add(new ListItem<Guid>() { Text = englishMinistry.Name, Value = englishMinistry.Ministry.Id, Selected = isSelected });
                    }
                }

                return ministries.AsReadOnly();
            }
        }

        private List<ListItem<Guid>> sectors;
        public IEnumerable<ListItem<Guid>> Sectors
        {
            get
            {
                if (sectors == null)
                {
                    sectors = new List<ListItem<Guid>>();

                    var englishSectors = from rl in db.SectorLanguages where rl.LanguageId == 4105 orderby rl.Sector.SortOrder, rl.Name select rl;
                    foreach (var englishSector in englishSectors)
                    {
                        bool isSelected = Release.Sectors.Contains(englishSector.Sector);
                        if (englishSector.Sector.IsActive || isSelected)
                            sectors.Add(new ListItem<Guid>() { Text = englishSector.Name, Value = englishSector.Sector.Id, Selected = isSelected });
                    }
                }

                return sectors.AsReadOnly();
            }
        }

        private List<ListItem<Guid>> themes;
        public IEnumerable<ListItem<Guid>> Themes
        {
            get
            {
                if (themes == null)
                {
                    themes = new List<ListItem<Guid>>();

                    var dbThemes = from t in db.Themes orderby t.SortOrder, t.DisplayName select t;
                    foreach (var dbTheme in dbThemes)
                    {
                        bool isSelected = Release.Themes.Contains(dbTheme);
                        if (dbTheme.IsActive || isSelected)
                            themes.Add(new ListItem<Guid>() { Text = dbTheme.DisplayName, Value = dbTheme.Id, Selected = isSelected });
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
                    foreach (var dbTag in dbTags)
                    {
                        bool isSelected = Release.Tags.Contains(dbTag);
                        if (dbTag.IsActive || isSelected)
                            tags.Add(new ListItem<Guid>() { Text = dbTag.DisplayName, Value = dbTag.Id, Selected = isSelected });
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

        public bool HasMediaAssets
        {
            get { return Release.HasMediaAssets; }
            set { Release.HasMediaAssets = value; }
        }

        public string AssetUrl
        {
            get { return Release.AssetUrl; }
            set { Release.AssetUrl = value; }
        }

        public int? ActivityId
        {
            get { return Release.ActivityId; }
            set { Release.ActivityId = value; }
        }

        public string Headline
        {
            get { return englishDocument.Headline; }
            set { englishDocument.Headline = value; }
        }

        public string Subheadline
        {
            get { return englishDocument.Subheadline; }
            set { englishDocument.Subheadline = value; }
        }

        public string Byline
        {
            get { return englishDocument.Byline; }
            set { englishDocument.Byline = value; }
        }

        public string BodyHtml
        {
            get { return englishDocument.BodyHtml; }
            set { englishDocument.BodyHtml = value; }
        }

        public IEnumerable<string> Contacts
        {
            get
            {
                return englishDocument.Contacts.OrderBy(e => e.SortIndex).Select(e => e.Information);
            }
            set
            {
                englishDocument.Contacts.Clear();

                int sortIndex = 0;
                foreach (string contactInformation in value)
                {
                    NewsReleaseDocumentContact contact = new NewsReleaseDocumentContact();

                    contact.SortIndex = sortIndex++;
                    contact.Information = contactInformation;

                    englishDocument.Contacts.Add(contact);
                }
            }
        }

        public DateTimeOffset? PublishDateTime
        {
            get { return Release.PublishDateTime; }
            set { Release.PublishDateTime = value; }
        }

        public NewModel()
        {
            Release = new NewsRelease();
            Release.Id = Guid.NewGuid();
            //TODO: Add AtomId format
            //release.AtomId = String.Format("tag:{0},{1}://1.{2}", "news.gov.bc.ca", DateTime.Today.Year, release.Id);
            Release.Reference = "";
            Release.IsActive = true;
            Release.ReleaseType = ReleaseType.Release;
            Release.PublishOptions = 0;
            Release.Timestamp = DateTimeOffset.Now;

            //TODO: Set fields based on input
            Release.AssetUrl = "";
            Release.RedirectUrl = "";
            Release.AtomId = "";
            Release.Keywords = "";

            //TODO: Set Collection based on current date
            Release.Collection = db.NewsReleaseCollections.OrderByDescending(e => e.Name).FirstOrDefault();

            englishRelease = new NewsReleaseLanguage();
            englishRelease.Language = db.English();
            
            Release.Languages.Add(englishRelease);

            document = new NewsReleaseDocument();
            document.Id = Guid.NewGuid();
            document.SortIndex = 0;
            Release.Documents.Add(document);

            englishDocument = new NewsReleaseDocumentLanguage();
            englishDocument.Language = db.English();
            document.Languages.Add(englishDocument);

            db.NewsReleases.Add(Release);
        }

        internal NewsRelease GetNewsRelease()
        {
            return Release;
        }

        public Guid Save(bool addReleaseType = false)
        {
            //TODO: Find "lead paragraph to populate RSS Summary field
            {
                string body = TidyAndTruncateDocumentBodyText(Gcpe.News.ReleaseManagement.Templates.Convert.HtmlToText(BodyHtml));

                //englishRelease.Summary = Location.ToUpper() + (Location == "" ? "" : " – ") + body;

                englishRelease.Summary = Utils.TrimSummary(body, 500);

                Key = GenerateSlug(Headline);
            }

            if (addReleaseType)
            {
                var releaseType = db.NewsReleaseTypes.SingleOrDefault(e => e.PageTitle == PageTitle);
                if (releaseType == null)
                {
                    releaseType = new NewsReleaseType();
                    releaseType.ReleaseType = (int)Release.ReleaseType;

                    releaseType.Language = db.English();

                    releaseType.PageTitle = PageTitle;

                    //TODO: Change ReleaseType PageLayout to Enum
                    releaseType.PageLayout = (int)PageLayout;

                    if (PageImageId.HasValue)
                        releaseType.PageImage = db.NewsReleaseImages.Find(PageImageId.Value);

                    releaseType.SortOrder = 0;
                    db.NewsReleaseTypes.Add(releaseType);
                }
            }

            Release.Ministries.Clear();
            foreach (var item in Ministries.Where(e => e.Selected))
                Release.Ministries.Add(db.Ministries.Find(item.Value));

            Release.Sectors.Clear();
            foreach (var item in Sectors.Where(e => e.Selected))
                Release.Sectors.Add(db.Sectors.Find(item.Value));

            Release.Tags.Clear();
            foreach (var item in Tags.Where(e => e.Selected))
                Release.Tags.Add(db.Tags.Find(item.Value));

            Release.Themes.Clear();
            foreach (var item in Themes.Where(e => e.Selected))
                Release.Themes.Add(db.Themes.Find(item.Value));

            Release.MediaDistributionLists.Clear();
            foreach (var item in MediaDistributionLists.Where(e => e.Selected))
                Release.MediaDistributionLists.Add(db.MediaDistributionLists.Find(item.Value));

            // Assign the PublishOptions *after* setting up the MediaDistributionLists!!
            if (Release.ReleaseType == ReleaseType.Advisory)
            {
                // Advisories are *NEVER* published to the BC Gov News website.
                // Advisories *always* get published to Media Distribution Lists, and no-where else!
                Release.PublishOptions = PublishOptions.PublishMediaContacts;
            }
            else
            {
                // Non-Advisories are *ALWAYS* published to the BC Gov News website.
                Release.PublishOptions = PublishOptions.PublishNewsArchives;

                // News Release and Update types *ALWAYS* go out to News-on-Demand.
                if (Release.ReleaseType == ReleaseType.Release || Release.ReleaseType == ReleaseType.Update)
                {
                    Release.PublishOptions |= PublishOptions.PublishNewsOnDemand;
                }
                Release.NodSubscribers = ReleasePublisher.NewsReleaseSubscriberCount(SelectedNodDistributionLists());
                // ...but *could* also be distributed to Media Distribution Lists.....
            }
            if (Release.MediaDistributionLists.Any())
            {
                Release.PublishOptions |= PublishOptions.PublishMediaContacts;
                Release.MediaSubscribers = ReleasePublisher.NewsReleaseSubscriberCount(SelectedMediaDistributionLists().ToList());
            }

            //TODO: Add New Type to Database
            SaveWithLog("Created " + englishDocument.PageTitle); /* NewsReleaseLog.Description */

            return Release.Id;
        }

        internal static string TidyAndTruncateDocumentBodyText(string bodyText)
        {
            bodyText = bodyText.Replace("\r", "");
            if (bodyText.Contains("\n"))
                bodyText = bodyText.Substring(0, bodyText.IndexOf("\n"));
            return bodyText;
        }
    }

    
}