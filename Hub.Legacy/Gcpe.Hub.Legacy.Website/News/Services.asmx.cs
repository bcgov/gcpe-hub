extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using CorporateCalendar.Data;

namespace Gcpe.Hub.News
{
    /// <summary>
    /// Summary description for Services
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Services : System.Web.Services.WebService
    {
        [WebMethod]
        public string StripDisallowedTags(string html)
        {
            return Gcpe.News.ReleaseManagement.Controls.HtmlTagCleaner.StripDisallowedTags(html);
        }

        [WebMethod]
        public string GetSearchUrl(string query)
        {
            //Search for News Release by Key or Reference
            using (legacy::Gcpe.Hub.Data.Entity.HubEntities db = new legacy::Gcpe.Hub.Data.Entity.HubEntities())
            {
                if (query.Contains("/"))
                {
                    string[] fields = query.Split('/');

                    if (fields.Length > 1)
                    {
                        string path = fields[fields.Length - 2];
                        legacy::Gcpe.Hub.Data.Entity.ReleaseType releaseType = path == "releases" ? legacy::Gcpe.Hub.Data.Entity.ReleaseType.Release : (path == "stories" ? legacy::Gcpe.Hub.Data.Entity.ReleaseType.Story : path == "factsheets" ? legacy::Gcpe.Hub.Data.Entity.ReleaseType.Factsheet : 0);

                        if (releaseType != 0)
                        {
                            string key = fields[fields.Length - 1];
                            var release = db.NewsReleases.SingleOrDefault(r => r.ReleaseType == releaseType && r.Key == key);

                            if (release != null)
                                return ReleaseManagement.ReleaseModel.ReleaseHubUrl(release);
                        }
                    }
                }
                else if (System.Text.RegularExpressions.Regex.IsMatch(query, @"\d\d\d\d\d") || System.Text.RegularExpressions.Regex.IsMatch(query.ToUpper(), @"NEWS-\d\d\d\d\d"))
                {
                    string newsId = "NEWS-" + query.ToUpper().Replace("NEWS-", "");

                    var release = db.NewsReleases.SingleOrDefault(r => r.Reference == newsId);

                    if (release != null)
                        return System.Web.VirtualPathUtility.ToAbsolute("~/News/ReleaseManagement/" + (release.IsPublished ? "Published" : (release.IsCommitted ? "Scheduled" : "Drafts")) + "/" + release.Reference);
                }
                else if (!string.IsNullOrWhiteSpace(query))
                {
                    var release = db.NewsReleases.SingleOrDefault(r => r.Reference == query);

                    if (release != null)
                        return System.Web.VirtualPathUtility.ToAbsolute("~/News/ReleaseManagement/" + (release.IsPublished ? "Published" : (release.IsCommitted ? "Scheduled" : "Drafts")) + "/" + release.Reference);
                }
            }

            //Default to Advanced Search Page
            return System.Web.VirtualPathUtility.ToAbsolute("~/News/Search") + "?q=" + HttpUtility.UrlEncode(query);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ActivityInfo FindCalendarActivity(string id)
        {
            int activityId;
            bool isInt = int.TryParse(id, out activityId);
            if (!isInt || activityId <= 0)
                return null;
            ActivityInfo activity;
            using (var corporateCalendarDataContext = new CorporateCalendarDataContext())
            {
                IQueryable<Activity> activities = corporateCalendarDataContext.Activities.Where(a => a.Id == activityId && a.EndDateTime >= DateTime.Today && a.IsConfidential == false
                            && a.ActivityCategories.Any(c => c.Category.Name.StartsWith("Approved")));
                activity = activities.Select(a => new ActivityInfo()
                {
                    Id = a.Id,
                    LeadMinistryId = a.ContactMinistryId,
                    Title = a.Title,
                    LeadOrganization = a.LeadOrganization,
                    CityId = a.CityId,
                    City = a.OtherCity,
                    StartDateTime = a.StartDateTime
                }).FirstOrDefault();

                if (activity != null)
                {
                    activity.LeadMinistry = corporateCalendarDataContext.Ministries.Where(a => a.Id == activity.LeadMinistryId)
                                    .Select(m => m.Abbreviation).FirstOrDefault();
                    activity.Ministries = corporateCalendarDataContext.Activities.Where(a => a.Id == activity.Id).FirstOrDefault()
                        .ActivitySharedWiths.Where(s => s.IsActive).Select(s => s.Ministry.Abbreviation).ToList();
                    activity.Themes = corporateCalendarDataContext.Activities.Where(a => a.Id == activity.Id).FirstOrDefault()
                        .ActivityThemes.Where(s => s.IsActive).Select(s => s.Theme.DisplayName).ToList();
                    activity.Tags = corporateCalendarDataContext.Activities.Where(a => a.Id == activity.Id).FirstOrDefault()
                        .ActivityTags.Where(s => s.IsActive).Select(s => s.Tag.DisplayName).ToList();

                    activity.Initiatives = corporateCalendarDataContext.ActivityInitiatives.Where(a => a.ActivityId == activity.Id && a.IsActive)
                        .Select(s => s.Initiative.Name).ToList();

                    IEnumerable<Sector> sectors = corporateCalendarDataContext.Activities.Where(a => a.Id == activity.Id).FirstOrDefault()
                        .ActivitySectors.Where(a => a.IsActive).Select(s => s.Sector);

                    sectors = sectors.Where(s => s.IsActive);
                    activity.Sectors = sectors.Select(s => s.DisplayName).ToList();
                    
                    if (activity.CityId != null)
                    {
                        string city = corporateCalendarDataContext.Cities.Where(c => c.Id == activity.CityId).Select(c => c.Name.Replace(", BC", "").Trim()).FirstOrDefault();
                        if(city != "Other...")
                        {
                            activity.City = city;
                        }
                        else if(!string.IsNullOrWhiteSpace(activity.City))
                        {
                            activity.City = activity.City.Replace(", BC", "");
                        }
                    }
                    if (activity.StartDateTime != null)
                    {
                        activity.FormatedStartDateTime = ((DateTime)activity.StartDateTime).ToString("yyyy-MM-dd hh:mm tt");
                    }
                    
                }
            }
            if(activity != null)
            {
                using (legacy::Gcpe.Hub.Data.Entity.HubEntities db = new legacy::Gcpe.Hub.Data.Entity.HubEntities())
                {
                    //Convert ministries to Hub Ministry id
                    activity.LeadMinistry = db.Ministries.Where(m => m.Abbreviation == activity.LeadMinistry).Select(m => m.Id.ToString()).FirstOrDefault();
                    activity.Ministries = db.Ministries.Where(m => activity.Ministries.Any(am => am == m.Abbreviation || (am == "PREMIER" && m.Abbreviation == "PREM")))
                        .SelectMany(m => m.Languages.Where(l => l.LanguageId == 4105)).OrderBy(l => l.Name).Select(m => m.MinistryId.ToString()).ToList();
                }
            }
            
            return activity;
        }
        public class ActivityInfo
        {
            public int Id { get; set; }
            public Guid? LeadMinistryId { get; set; }
            public string LeadMinistry { get; set; }

            public List<string> Ministries { get; set; }
            public List<string> Sectors { get; set; }
            public List<string> Themes { get; set; }
            public List<string> Tags { get; set; }
            public List<string> Initiatives { get; set; }
            public string Title { get; set; }
            public string LeadOrganization { get; set; }
            public int? CityId { get; set; }
            public string City { get; set; }
            public DateTime? StartDateTime { get; set; }
            public string FormatedStartDateTime { get; set; }
        }
        
    }
}