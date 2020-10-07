extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using legacy::Gcpe.Hub.Data.Entity;

namespace Gcpe.Hub.News
{

    public class SearchModel : HubModel
    {

        private string keyword;


        private List<KeyValuePair<Guid, string>> filterByMinistries;
        public List<KeyValuePair<Guid, string>> FilterByMinistries
        {
            get { return filterByMinistries; }
            set { filterByMinistries = value; }
        }

        private List<KeyValuePair<Guid, string>> filterBySectors;
        public List<KeyValuePair<Guid, string>> FilterBySectors
        {
            get { return filterBySectors; }
            set { filterBySectors = value; }
        }

        private StatusOptions? filterByStatus;
        public StatusOptions? FilterByStatus
        {
            get { return filterByStatus; }
            set { filterByStatus = value; }
        }

        private DateRangeOptions? filterByDateRange;
        public DateRangeOptions? FilterByDateRange
        {
            get { return filterByDateRange; }
            set { filterByDateRange = value; }
        }

        private DateTime? filterByStartDate;
        public DateTime? FilterByStartDate
        {
            get { return filterByStartDate; }
            set { filterByStartDate = value; }
        }

        private DateTime? filterByEndDate;
        public DateTime? FilterByEndDate
        {
            get { return filterByEndDate; }
            set { filterByEndDate = value; }
        }


        public IEnumerable<SearchResult> GetResults(int maximumRows, int startRowIndex)
        {
            List<SearchResult> list = new List<SearchResult>();

            var results = (from nr in Results.Distinct()
                           join nrl in db.NewsReleaseLanguages.Where(l => l.LanguageId == 4105) on nr.Id equals nrl.ReleaseId
                           let doc =
                               (
                                   from nd in db.NewsReleaseDocuments
                                   join doc in db.NewsReleaseDocumentLanguages on nd.Id equals doc.DocumentId
                                   where doc.LanguageId == 4105 //MICHAEL - is this ok to hardcode the language id?
                                   && nd.ReleaseId == nr.Id
                                   select new
                                   {
                                       item_id = nd.Id,
                                       release_id = nd.ReleaseId,
                                       sort = nd.SortIndex,
                                       headline = doc.Headline,
                                       desc = nrl.Summary
                                   }
                                 ).OrderBy(nds => nds.sort).FirstOrDefault()
                           orderby nr.ReleaseDateTime descending, doc.headline
                           select new SearchResult
                           {
                               Release = nr,
                               Title = doc.headline,
                               Date = nr.ReleaseDateTime,
                               Description = doc.desc,
                               IsCommitted = nr.IsCommitted,
                               Source = "News Release",
                               IsPublished = nr.IsPublished,
                               Reference = nr.Reference,
                               Id = nr.Id
                           }).Skip(startRowIndex).Take(maximumRows).ToList();

            foreach (SearchResult s in results)
                s.Url = ReleaseManagement.ReleaseModel.ReleaseHubUrl(s.Release);

            return results;
        }

        public IQueryable<NewsRelease> Results
        {
            get
            {
                var ministriesFilter = filterByMinistries.Select(m => m.Key).ToList();
                var sectorsFilter = filterBySectors.Select(m => m.Key).ToList();
                var isCCIDsearch = false;

                IQueryable<NewsRelease> results;

                if (string.IsNullOrEmpty(keyword))
                {
                    results = from nr in db.NewsReleases
                              where nr.IsActive
                              select nr;
                }
                else if (System.Text.RegularExpressions.Regex.IsMatch(keyword, @"[a-zA-Z]+-\d+"))
                {
                    var activityId = Int32.Parse(keyword.Split('-')[1]);
                    isCCIDsearch = true;

                    results = from nr in db.NewsReleases
                              where nr.IsActive
                              && nr.ActivityId == activityId
                              select nr;
                }
                else
                {
                    results = from nr in db.NewsReleases
                              join nrd in db.NewsReleaseDocuments on nr.Id equals nrd.ReleaseId
                              join nrdl in db.NewsReleaseDocumentLanguages on nrd.Id equals nrdl.DocumentId
                              where nr.IsActive && nrdl.Headline.Contains(keyword)
                              select nr;
                }

                if (filterByStartDate.HasValue)
                    results = results.Where(nr => nr.ReleaseDateTime >= filterByStartDate);

                //if (filterByStatus == StatusOptions.Published)
                if (!isCCIDsearch)
                    results = results.Where(nr => nr.IsCommitted);

                if (filterByEndDate.HasValue)
                    results = results.Where(nr => nr.ReleaseDateTime <= filterByEndDate);

                if (ministriesFilter.Any())
                    results = results.Where(nr => ministriesFilter.All(m => nr.Ministries.Any(nrm => m == nrm.Id)));

                if (sectorsFilter.Any())
                    results = results.Where(nr => sectorsFilter.All(m => nr.Sectors.Any(nrm => m == nrm.Id)));

                return results;
            }
        }

        int? countResults;
        public int CountResults()
        {
            if (!countResults.HasValue)
                countResults = Results.Distinct().Count();

            return countResults.Value;
        }

        public SearchModel(string searchText)
        {
            keyword = searchText.Trim();
            filterByMinistries = new List<KeyValuePair<Guid, string>>();
            filterBySectors = new List<KeyValuePair<Guid, string>>();
            filterByStatus = null;
            filterByDateRange = null;
        }

        //public void Search()
        //{
        //}

        public Dictionary<DateRangeOptions, string> DatePresets
        {
            get
            {
                IEnumerable<DateRangeOptions> pageLayouts = Enum.GetValues(typeof(DateRangeOptions)).Cast<DateRangeOptions>();
                return pageLayouts.ToDictionary(pl => pl, pl => pl.ToString().Replace("_", " "));
            }
        }

        public Dictionary<StatusOptions, string> Statuses
        {
            get
            {
                IEnumerable<StatusOptions> pageLayouts = Enum.GetValues(typeof(StatusOptions)).Cast<StatusOptions>();
                return pageLayouts.ToDictionary(pl => pl, pl => pl.ToString());
            }
        }



        /// <summary>
        /// The list of ministries associated with the current search results
        /// </summary>
        private List<ListItem<Guid>> ministries;
        public List<ListItem<Guid>> Ministries
        {
            get
            {
                if (ministries == null)
                {
                    IEnumerable<Ministry> results = Results.SelectMany(nr => nr.Ministries).Distinct();
                    results = results.Where(m => !filterByMinistries.Any(f => f.Key == m.Id));
                    results = results.OrderBy(m => m.SortOrder).ThenBy(m => m.DisplayName);

                    ministries = results.Select(m => new ListItem<Guid>()
                    {
                        Text = m.DisplayName,
                        Value = m.Id
                    }).ToList();
                }

                return ministries;
            }
        }

        /// <summary>
        /// The list of sectors based on the current search
        /// </summary>
        private List<ListItem<Guid>> sectors;
        public List<ListItem<Guid>> Sectors
        {
            get
            {
                if (sectors == null)
                {
                    IEnumerable<Sector> results = Results.SelectMany(nr => nr.Sectors).Distinct();
                    results = results.Where(m => !filterBySectors.Any(f => f.Key == m.Id));

                    //TODO: Use DisplayName instead of English().Name
                    results = results.OrderBy(m => m.SortOrder).ThenBy(m => m.English().Name);

                    sectors = results.Select(m => new ListItem<Guid>()
                    {
                        Text = m.English().Name,
                        Value = m.Id
                    }).ToList();
                }

                return sectors;
            }
        }

        public string AllQueryFilters()
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            var filters = new Dictionary<string, string>();
            if (filterByDateRange.HasValue)
            {
                filters["DateRange"] = filterByDateRange.ToString();
            }
            if (filterByStatus.HasValue)
            {
                filters["Status"] = filterByStatus.Value.ToString();
            }
            if (filterByMinistries.Any())
            {
                filters["Ministries"] = filterByMinistries.First().Value;
            }
            if (filterBySectors.Any())
            {
                filters["Sector"] = filterBySectors.First().Value;
            }
            return json.Serialize(filters);
        }

        public class SearchResult
        {
            public NewsRelease Release;
            public string Title;
            public string Description;
            public DateTime? Date;
            public bool IsCommitted;
            public string Source;
            public string Url;
            public string Status
            {
                get
                {
                    string status = "";
                    if (Url.Contains("Drafts")) status = "Draft";
                    if (Url.Contains("Published")) status = "Published";
                    if (Url.Contains("Scheduled")) status = "Scheduled";

                    return status;
                }
            }

            public bool IsPublished;
            public string Reference;
            public Guid Id;

        }
    }



    public enum DateRangeOptions
    {
        Today = 0,
        This_Week = 1,
        This_Month = 3,
        This_Year = 4,
        Last_Year = 5,
        Any_Time = 6

        //Anytime = 0,
        //Past_hour = 1,
        //Past_24_hour = 3,
        //Past_week = 4,
        //Past_month = 5,
        //Past_year = 6
    }

    public enum StatusOptions
    {
        Draft = 0,
        Published = 1
    }
}