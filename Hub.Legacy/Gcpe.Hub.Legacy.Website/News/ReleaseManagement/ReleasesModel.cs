extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gcpe.Hub.News.ReleaseManagement
{
    using legacy::Gcpe.Hub.Data.Entity;

    public class ReleasesModel
    {
        public string ResultSetName { get; set; }

        public IQueryable<NewsRelease> ResultSet
        {
            get;
            set;
        }

        public IEnumerable<ReleasesModel.Result> ActivityResultSet { get; set; }

        public IEnumerable<ReleasesModel.Result> GetResults(int maximumRows, int startRowIndex)
        {
            if (ResultSetName == "Forecast")
                return new List<ReleasesModel.Result>();

            var results = ResultSet.Skip(startRowIndex).Take(maximumRows).ToArray().Select(r => new ReleasesModel.Result()
            {
                PageTitle = r.Documents.OrderBy(d => d.SortIndex).First().English().PageTitle,
                Headline = r.Documents.OrderBy(d => d.SortIndex).First().English().Headline,
                IsPublished = r.IsPublished,
                IsCommitted = r.IsCommitted,
                Reference = r.Reference,
                Id = r.Id,
                Summary = r.Languages.Where(l => l.LanguageId == 4105).Select(l => l.Location.ToUpper() + (string.IsNullOrEmpty(l.Location) ? "" : " – ") + l.Summary).FirstOrDefault(),
                //ReleaseDate = r.ReleaseDate,
                CreateDate = r.Logs.Where(l => l.Description.StartsWith("Created ")).Select(l => l.DateTime).FirstOrDefault(),
                FirstOrganization = GetFirstOrganization(r),
                PublishDateTime = r.PublishDateTime,
                ReleaseType = Enum.GetName(typeof(ReleaseType), r.ReleaseType),
                ActivityId = r.ActivityId
            });

            return results;
        }

        int? countResults;
        public int CountResults()
        {
            if (!countResults.HasValue)
                countResults = ResultSet.Count();

            return countResults.Value;
        }

        public IEnumerable<ReleasesModel.Result> GetActiveActivities(int maximumRows, int startRowIndex)
        {
            if (ActivityResultSet == null)
            {
                if (ResultSetName == "Forecast")
                    ActivityResultSet = GetFutureActivitiesAndNR();
                else
                    ActivityResultSet = new List<ReleasesModel.Result>();

            }
            return ActivityResultSet.Skip(startRowIndex).Take(maximumRows);
        }

        public int GetActivityCount()
        {
            if (ActivityResultSet == null)
            {
                if (ResultSetName == "Forecast")
                    ActivityResultSet = GetFutureActivitiesAndNR();
                else
                    ActivityResultSet = new List<ReleasesModel.Result>();
            }
            return ActivityResultSet.Count();
        }

        //public IEnumerable<System.Web.UI.WebControls.ListItem> GetPages()
        //{
        //    List<System.Web.UI.WebControls.ListItem> items = new List<System.Web.UI.WebControls.ListItem>();

        //    int Count = ResultSet.Count();
        //    for (int i = 0; i < Count; i += PageSize)
        //    {
        //        items.Add(new System.Web.UI.WebControls.ListItem()
        //        {
        //            Text = (i / PageSize + 1).ToString(),
        //            Selected = i == StartRowIndex,
        //            Value = i.ToString()
        //        });
        //    }

        //    return items;
        //}

        public string GetFirstOrganization(NewsRelease nr)
        {
            string organization = "";
            try
            {
                if (organization == "" && nr.Ministry != null)
                    organization = nr.Ministry.DisplayName;

                NewsReleaseDocument firstDocument = nr.Documents.OrderBy(d => d.SortIndex).First();

                if (firstDocument.PageLayout == PageLayout.Formal)
                {
                    if (organization == "")
                        organization = firstDocument.English().Organizations.Split('\n')[0].Replace("Ministry of ", "").Trim();
                }
                else if (firstDocument.PageLayout == PageLayout.Informal)
                {
                    if (organization == "")
                    {
                        string minister = firstDocument.English().Byline.Split('\n').FirstOrDefault(b => b.StartsWith("Minister of "));

                        if (minister != null)
                        {
                            minister = minister.Substring("Minister of ".Length).Trim();
                            organization = minister.Trim();
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }

                if (organization == "" && nr.Ministries.Count == 1)
                    organization = nr.Ministries.Single().DisplayName;
            }
            catch
            {
            }

            return organization;
        }

        public List<ReleasesModel.Result> GetFutureActivitiesAndNR()
        {
            List<CorporateCalendar.Data.ActiveActivity> futureActivities;
            List<CorporateCalendar.Data.Ministry> ministries;

            using (var db = new CorporateCalendar.Data.CorporateCalendarDataContext())
            {
                futureActivities = db.ActiveActivities
                    .Where(a => a.StartDateTime >= DateTime.Now && !a.IsConfidential && a.IsConfirmed && a.IsActive &&
                        (a.Categories.StartsWith(" Approved") ))
                    .ToList();

                ministries = db.Ministries.Where(m => m.IsActive).ToList();
            }

            IEnumerable<Result> results;

            using (HubEntities dbContext = new HubEntities())
            {
                List<NewsRelease> futureReleases;

                if (ResultSet != null)
                {
                    futureReleases = ResultSet.ToList();
                }
                else
                {
                    futureReleases = (from nr in dbContext.NewsReleases
                                      where nr.IsActive && nr.PublishDateTime != null && nr.PublishDateTime >= DateTimeOffset.Now
                                      orderby nr.PublishDateTime descending
                                      select nr).ToList();
                }

                results = futureReleases.Select(r => new Result()
                {
                    PageTitle = r.Documents.OrderBy(d => d.SortIndex).First().English().PageTitle,
                    Headline = r.Documents.OrderBy(d => d.SortIndex).First().English().Headline,
                    IsPublished = r.IsPublished,
                    IsCommitted = r.IsCommitted,
                    Reference = r.Reference,
                    Id = r.Id,
                    Summary = r.Languages.Where(l => l.LanguageId == 4105).Select(l => l.Location.ToUpper() + (string.IsNullOrEmpty(l.Location) ? "" : " – ") + l.Summary).FirstOrDefault(),
                    //ReleaseDate = r.ReleaseDate,
                    CreateDate = r.Logs.Where(l => l.Description.StartsWith("Created ")).Select(l => l.DateTime).FirstOrDefault(),
                    FirstOrganization = GetFirstOrganization(r),
                    PublishDateTime = r.PublishDateTime,
                    ReleaseType = Enum.GetName(typeof(ReleaseType), r.ReleaseType),
                    ActivityId = r.ActivityId,
                    IsCalActivity = false
                }).ToList();
            }

            var activityIds = results.Select(nr => nr.ActivityId).Distinct();

            var results1 = futureActivities.Where(a => !activityIds.Contains(a.Id)).Select(a => new Result()
            {
                PageTitle = "",
                Headline = a.Title,
                IsPublished = false,
                IsCommitted = false,
                Reference = "",
                ActivityId = a.Id,
                Summary = a.Details,
                //ReleaseDate = r.ReleaseDate,
                CreateDate = a.CreatedDateTime,
                FirstOrganization = ministries.Where(m => m.Id == a.ContactMinistryId).Select(m => m.DisplayName).FirstOrDefault(),
                PublishDateTime = a.NRDateTime ?? a.StartDateTime,
                ReleaseType = "",
                IsCalActivity = true,
                IsConfidential = a.IsConfidential,
                IsConfirmed = a.IsConfirmed,
                EventType = a.Categories,
                Location = a.City == null ? "" : a.City.Replace(", BC", "").Trim(),
                EventEndDateTime = a.EndDateTime,
                IsAllDay = a.IsAllDay
            });

            foreach (Result release in results)
            {
                if (release.ActivityId != null)
                {
                    var activity = futureActivities.Where(a => a.Id == release.ActivityId).FirstOrDefault();
                    if (activity != null)
                    {
                        release.IsAllDay = activity.IsAllDay;
                        release.EventType = activity.Categories;
                    }
                }
            }

            return results.Concat(results1).OrderBy(a => a.PublishDateTime).ToList();
        }

        public class Result
        {
            public string PageTitle { get; set; }
            public string Headline { get; set; }
            public string Summary { get; set; }
            public bool IsPublished { get; set; }
            public bool IsCommitted { get; set; }
            public string Reference { get; set; }
            public Guid? Id { get; set; }
            //public DateTime? ReleaseDate { get; set; }
            public DateTimeOffset? CreateDate { get; set; }
            public string FirstOrganization { get; set; }
            public DateTimeOffset? PublishDateTime { get; set; }
            public string ReleaseType { get; set; }

            public bool IsCalActivity { get; set; }
            public int? ActivityId { get; set; }

            public bool? IsConfidential { get; set; }

            public string Location { get; set; }

            public string EventType { get; set; }
            public bool? IsConfirmed { get; set; }

            public DateTimeOffset? EventEndDateTime { get; set; }
            public bool? IsAllDay { get; set; }

        }
    }
}