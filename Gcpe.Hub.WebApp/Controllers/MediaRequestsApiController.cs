using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Gcpe.Hub.Data.Entity;
using Gcpe.Hub.WebApp.Models;
using Gcpe.Hub.WebApp.Providers;
using Gcpe.Hub.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Search.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Gcpe.Hub.WebApp.Controllers
{
    [Route("api/mediarequests")]
    public class MediaRequestsApiController : BaseController
    {
        readonly MailProvider mailProvider;
        readonly string HQMinistry = "GCPEMedia";
        static HttpClient Client = new HttpClient();
        static int PageSize = 40;
        private readonly IConfiguration Configuration;

        public MediaRequestsApiController(HubDbContext db, MailProvider mailProvider, IConfiguration configuration) : base(db)
        {
            this.mailProvider = mailProvider;
            Configuration = configuration;
        }

        [HttpGet]
        public async Task<IEnumerable<MediaRequestDto>> Get(string responded = null,
            int _skip = 0,
            int _limit = 20,
            string ministries = null,
            Boolean idsOnly = false,
            DateTimeOffset? requestsToday = null,
            DateTimeOffset? requestsBefore = null,
            DateTimeOffset? requestsAfter = null,
            DateTimeOffset? modifiedAfter = null)
        {
            string suffixClause = "";
            if (ministries != null)
            {
                string ministryInClause = "IN('" + SqlHelper.ProtectAgainstSqlInjection(ministries).Replace(",", "', '") + "')";
                suffixClause = " LEFT JOIN media.MediaRequestSharedMinistry s ON rq.Id = s.MediaRequestId";
                suffixClause += " WHERE (LeadMinistryId " + ministryInClause + " OR s.MinistryId " + ministryInClause + ") ";
            }

            // responded == "all" - nothing to filter out!
            if (responded == "open")
            {
                SqlHelper.AddToWhereClause(ref suffixClause, "RespondedAt IS NULL");
            }
            else if (responded == "completed")
            {
                SqlHelper.AddToWhereClause(ref suffixClause, "RespondedAt IS NOT NULL");
            }
            else if (responded == "active" && requestsToday.HasValue)
            {
                // Open MediaRequests or MediaRequests that were closed today.
                SqlHelper.AddDateToWhereClause(ref suffixClause, "RespondedAt IS NULL OR RespondedAt >= ", requestsToday);
            }

            if (requestsBefore.HasValue)
            {
                SqlHelper.AddDateToWhereClause(ref suffixClause, "CreatedAt < ", requestsBefore);
            }

            if (requestsAfter.HasValue)
            {
                SqlHelper.AddDateToWhereClause(ref suffixClause, "CreatedAt > ", requestsAfter);
            }

            if (modifiedAfter.HasValue)
            {
                SqlHelper.AddDateToWhereClause(ref suffixClause, "ModifiedAt > CreatedAt AND ModifiedAt > ", modifiedAfter);
            }

            return await QueryMediaRequests(suffixClause, requestsAfter.HasValue ? null : (int?)_skip, _limit, idsOnly);
        }

        [HttpGet("search")]
        public async Task<SearchResultsDto> Get(string query, int page, string leadMinistryDisplayName, string companyNames, string contactNames, string resolutionId)
        {
            var results = new SearchResultsDto();
            bool useSearchService = Configuration.GetValue<bool>("SearchService:Enable");
            int skip = page * PageSize;
            if (useSearchService && query != null)
            {
                // adjust the query to allow for partial text matches.
                // 
                string adjustedQuery;
                if (query.Contains('/') || query.Contains(' '))
                {
                    adjustedQuery = query;
                }
                else
                {
                    adjustedQuery = "/.*" + query + ".*/";
                }
                var temp = await GetResolutions();
                if (resolutionId != null)
                {
                    resolutionId = temp.FirstOrDefault(x => x.DisplayAs == resolutionId).Id.ToString();
                }
                // get a list of IDs from the search service.
                var facets = new Dictionary<string, string> { { "leadMinistryDisplayName", leadMinistryDisplayName }, { "companyNames", companyNames }, { "contactNames", contactNames }, { "resolutionId", resolutionId } };
                DocumentSearchResult searchServiceResult = await QueryHubMediaRequestSearchService(adjustedQuery, facets, skip, PageSize);
                List<FacetDto> facetResults = new List<FacetDto>();

                // get full information on each MediaRequest from the database
                if (searchServiceResult.Results != null && searchServiceResult.Results.Count > 0)
                {
                    // extract the ids.
                    string inClause = string.Join("','", searchServiceResult.Results.Select(r => r.Document.Values.First()));
                    List<MediaRequest> data = db.MediaRequest.FromSqlRaw("SELECT * FROM media.MediaRequest WHERE Id IN ('" + inClause + "') order by RequestedAt DESC").ToList();
                    
                    LoadNavigationProperties(data);

                    // Convert results to MediaRequestDto

                    results.MediaRequests = data.Select(e => ConvertToDto(e)).ToList();

                    foreach (var facet in facets.Keys) // iterate in the order we asked for
                    {
                        if (facet != "fromDate" | facet != "toDate")
                        {
                            FilterDto fdto = new FilterDto();
                            if (facet!= "resolutionId")
                            {
                                fdto.Name = item.Value.ToString();
                            }
                            else
                            {
                                //var id = new Guid();
                                Guid.TryParse(item.Value.ToString(), out var id);
                                fdto.Name = temp.FirstOrDefault(x => x.Id == id).DisplayAs;
                            }
                            //fdto.Name = item.Value.ToString();
                            fdto.Count = (int)item.Count;
                            facetFilters.Add(fdto);
                        }
                        facetResults.Add(new FacetDto() { Name = facet, Filters = facetFilters });
                    }
                }
                else // no search results, return an empty list of media requests.
                {
                    results.MediaRequests = new List<MediaRequestDto>();
                }
                results.Facets = facetResults;

            }
            else
            {
                string s = "(CONTAINS((RequestTopic, RequestContent, Response), 'FORMSOF(INFLECTIONAL, {0})') OR CONTAINS((FirstName, LastName), '\"{0}*\"') OR CONTAINS(CompanyName, '\"{0}*\"'))";
                string suffixClause = SqlHelper.CreateSearchClause(query, s);

                // Prepend additional joins to Contact and Company tables
                suffixClause = " LEFT JOIN media.MediaRequestContact rc ON rc.MediaRequestId = rq.Id" +
                                     " LEFT JOIN media.Contact ct ON ct.Id = rc.ContactId" +
                                     " LEFT JOIN media.Company cp ON cp.Id = rc.CompanyId" + suffixClause;

                // Search never does idsOnly.
                const Boolean idsOnly = false;
                results.MediaRequests = await QueryMediaRequests(suffixClause, skip, PageSize, idsOnly);
            }

            return results;
        }

        private async Task<DocumentSearchResult> QueryHubMediaRequestSearchService(string query, IDictionary<string, string> facets, int? _skip, int _limit)
        {
            DocumentSearchResult result = null;
            // Add the Http Get parameters
            string newUri = QueryHelpers.AddQueryString("search", "query", query);
            if (_skip != null)
            {
                newUri = QueryHelpers.AddQueryString(newUri, "skip", _skip.ToString());
            }
            newUri = QueryHelpers.AddQueryString(newUri, "limit", _limit.ToString());
            foreach (var facet in facets)
            {
                newUri = QueryHelpers.AddQueryString(newUri, "facets", facet.Key);
                if (!string.IsNullOrEmpty(facet.Value))
                {
                    string azureFormat = facet.Key.EndsWith('s') ? "{0}/any(t: t eq '{1}')" : "{0} eq '{1}'";
                    newUri = QueryHelpers.AddQueryString(newUri, "filters", string.Format(azureFormat, facet.Key, facet.Value.Replace("'", "''")));
                }
            }
            newUri = QueryHelpers.AddQueryString(newUri, "selectedFields", "id");
            newUri = QueryHelpers.AddQueryString(newUri, "sortFields", "respondedAt desc");
            try
            {
                HttpResponseMessage response = await SendRequestToAzureSearchServiceAsync(HttpMethod.Get, newUri);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ApiHttpException(HttpStatusCode.InternalServerError, new Exception("Unauthorized - Token was rejected by the search server"));
                }
                var stringtask = response.Content.ReadAsStringAsync();
                stringtask.Wait();
                // parse as JSON.
                string jsonString = stringtask.Result;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApiHttpException(HttpStatusCode.InternalServerError);
                }
                // Success. Remove the continuationToken so that the deserialization succeeds
                result = JsonConvert.DeserializeObject<DocumentSearchResult>(jsonString.Replace(",\"continuationToken\":null", ""));
            }
            catch (Exception e)
            {
                throw new ApiHttpException(HttpStatusCode.InternalServerError);
            }
            return result;
        }


        /// <summary>
        /// Trigger a removal or a re-index for a specific document
        /// </summary>
        /// <param name="method"></param>
        /// <param name="id"></param>
        void SendRequestToAzureSearchService(HttpMethod method, Guid id)
        {
            try
            {
                Task<HttpResponseMessage> responseTask = SendRequestToAzureSearchServiceAsync(method, id.ToString());
                responseTask.Wait();
            }
            catch (Exception e)
            {
                // non-critical error - the document will be indexed at the next pass of the scheduled job.
            }
        }

        async Task<HttpResponseMessage> SendRequestToAzureSearchServiceAsync(HttpMethod method, string uriSuffix)
        {
            string baseUri = Configuration.GetValue<string>("SearchService:BaseUri") + "/api/index/mediarequest/";
            string accessToken = Configuration.GetValue<string>("SearchService:AccessToken");
            var request = new HttpRequestMessage(method, baseUri + uriSuffix);
            request.Headers.Clear();
            request.Headers.Add("Authorization", "Bearer " + accessToken);

            return await Client.SendAsync(request);
        }

        private async Task<IEnumerable<MediaRequestDto>> QueryMediaRequests(string suffixClause, int? _skip, int _limit, Boolean idsOnly)
        {
            //Only return Active Media Requests. 
            SqlHelper.AddToWhereClause(ref suffixClause, "rq.IsActive='true'");
            suffixClause += " ORDER BY rq.CreatedAt DESC";

            // idsOnly requests aren't paginated.
            if (idsOnly == false && _skip.HasValue)
            {
                //when were getting new ones, we have to get them all. Skipping and limiting is applied to the 'before' requests
                suffixClause += string.Format(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", _skip, _limit);
            }
            IQueryable<MediaRequest> query = null;
            if (!string.IsNullOrEmpty(suffixClause))
            {
                string sql = "SELECT DISTINCT rq.* FROM media.MediaRequest rq" + suffixClause;

                query = db.MediaRequest.FromSqlRaw(sql);
            }

            var list = await QueryMediaRequestsAsync(query ?? db.MediaRequest, idsOnly);


            return list.Select(e => ConvertToDto(e));
        }

        //[HttpGet]
        //[Route("modified")]
        //public async Task<IEnumerable<MediaRequestDto>> Get(DateTimeOffset? after)
        //{
        //    if (after == null)
        //    {
        //        throw new HttpResponseException(HttpStatusCode.BadRequest);
        //    }
        //    var models = QueryMediaRequests(db.MediaRequest
        //                    .Where(e => e.ModifiedAt > after)
        //                    .Where(e => e.ModifiedAt > e.CreatedAt)
        //                    .OrderByDescending(e => e.CreatedAt));

        //    var list = await models;
        //    return list.Select(e => ConvertToDto(e));
        //}

        private async Task<IList<MediaRequest>> QueryMediaRequestsAsync(IQueryable<MediaRequest> query, Boolean idsOnly)
        {
            IList<MediaRequest> list = await query.ToListAsync();
            /*.Include(e => e.LeadMinistry) // very inefficient and can't use with raw SQL (for text search)
            .Include(e => e.ResponsibleUser)
            .Include(e => e.CreatedBy)
            .Include(e => e.ModifiedBy);
            .Include(e => e.MediaRequestContact).ThenInclude(e => e.Contact).ThenInclude(e => e.ContactPhoneNumber)
            .Include(e => e.MediaRequestContact).ThenInclude(e => e.Contact).ThenInclude(e => e.ContactMediaJobTitle).ThenInclude(e => e.MediaJobTitle)
            .Include(e => e.MediaRequestContact).ThenInclude(e => e.Contact).ThenInclude(e => e.ContactWebAddress)
            .Include(e => e.MediaRequestContact).ThenInclude(e => e.Company)
            .Include(e => e.MediaRequestSharedMinistry).ThenInclude(e => e.Ministry);*/

            // Optionally, don't get the details!
            if (idsOnly)
            {
                // TODO: This should be refactored somehow to strip out the 
                //       undefined properties in the JSON and send ONLY the Ids.
                IList<MediaRequest> idList = new List<MediaRequest>();
                foreach (MediaRequest mr in list)
                {
                    // Reply with a set of MediaRequest with ONLY the Ids.
                    //if (mr.Id.Equals(new Guid("19d2949d-e1c9-4b7c-a185-bd39ca12e82d")))
                    //{
                    //    MediaRequest tmp = new MediaRequest();
                    //}
                    //else { 
                    MediaRequest tmp = new MediaRequest();
                    tmp.Id = mr.Id;
                    idList.Add(tmp);
                    //}
                }
                return idList;
            }

            LoadNavigationProperties(list);
            return list;
        }

        //TODO: Assess this method's impact on query performance
        private void LoadNavigationProperties(IList<MediaRequest> list)
        {
            // Explicit and efficient load of Navigation properties
            if (list.Count() != 0)
            {
                IEnumerable<MediaRequest> withParents = list.Where(r => r.RequestParentId.HasValue);
                IEnumerable<MediaRequest> list2populate = list;
                if (withParents.Count() != 0)
                {
                    db.MediaRequest.FromSqlRaw("SELECT * FROM media.MediaRequest WHERE Id " + SqlHelper.ToInClause(withParents.Select(r => r.RequestParentId.Value))).Load();
                    list2populate = list2populate.Union(withParents.Select(r => r.RequestParent));
                }

                string inClause = SqlHelper.ToInClause(list2populate.Select(r => r.Id));
                db.MediaRequest.FromSqlRaw("SELECT * FROM media.MediaRequest WHERE Id " + inClause)
                     .Include(e => e.Resolution)
                     .Include(e => e.ResponsibleUser)
                     .Include(e => e.CreatedBy)
                     .Include(e => e.ModifiedBy)
                     .Include(e => e.LeadMinistry)
                     .Include(e => e.MediaRequestSharedMinistry).ThenInclude(e => e.Ministry)
                     .Include(e => e.TakeOverRequestMinistry)
                     .Load();

                db.MediaRequestContact.FromSqlRaw("SELECT * FROM media.MediaRequestContact WHERE MediaRequestId " + inClause)
                    .Include(e => e.Company).Include(e => e.Contact).Load();

                List<Guid> contactsGuids = new List<Guid>();
                foreach (var mediaRequestContact in list2populate.SelectMany(r => r.MediaRequestContact))
                {
                    if (!contactsGuids.Contains(mediaRequestContact.ContactId))
                        contactsGuids.Add(mediaRequestContact.ContactId);
                }
                inClause = SqlHelper.ToInClause(contactsGuids);
                db.ContactMediaJobTitle.FromSqlRaw("SELECT * FROM media.ContactMediaJobTitle WHERE ContactId " + inClause).Load();
                SqlHelper.LoadContactNavigationProperties(inClause, db);
            }
        }

        private void ValidateDtoRequiredFields(MediaRequestDto dto)
        {
            if (dto.LeadMinistry == null)
            {
                throw new ApiHttpException(HttpStatusCode.BadRequest, new ArgumentNullException(nameof(dto.LeadMinistry)));
            }

            if (dto.RequestedAt == default(DateTimeOffset))
            {
                throw new ApiHttpException(HttpStatusCode.BadRequest, new ArgumentNullException(nameof(dto.RequestedAt)));
            }

            if (string.IsNullOrWhiteSpace(dto.RequestContent))
            {
                throw new ApiHttpException(HttpStatusCode.BadRequest, new ArgumentNullException(nameof(dto.RequestContent)));
            }

            if (string.IsNullOrWhiteSpace(dto.RequestTopic))
            {
                throw new ApiHttpException(HttpStatusCode.BadRequest, new ArgumentNullException(nameof(dto.RequestTopic)));
            }

            if (!dto.MediaContacts.Any())
            {
                throw new ApiHttpException(HttpStatusCode.BadRequest, new ArgumentNullException(nameof(dto.MediaContacts)));
            }

            if (dto.ResponsibleUser == null)
            {
                throw new ApiHttpException(HttpStatusCode.BadRequest, new ArgumentNullException(nameof(dto.ResponsibleUser)));
            }
        }

        [HttpPost]
        public async Task<Guid> Post([FromBody] MediaRequestDto dto, Boolean triggerEmail = false, Boolean onlyEmailMyself = false)
        {
            //TODO: Consider if this method should return DTO instead of Guid

            ValidateDtoRequiredFields(dto);

            var mediaRequest = await ConvertFromDtoAsync(dto);
            //TODO Throw security exception if users provide a created time/user etc.

            mediaRequest.CreatedAt = mediaRequest.ModifiedAt;
            mediaRequest.CreatedBy = mediaRequest.ModifiedBy;

            db.MediaRequest.Add(mediaRequest);

            await db.SaveChangesAsync();
            // update dto with the new ID.
            dto.Id = mediaRequest.Id;

            if (triggerEmail || onlyEmailMyself)
            {
                try
                {
                    await EmailMediaRequest(dto, onlyEmailMyself);
                }
#if !DEBUG
                catch (Exception)
                {
                    // Don't prevent users from getting things done if there are errors sending mail.
                    // throw this.BadRequest(new System.ApplicationException("Unable to send mail for saved Media Request."));
                }
#endif
                finally
                {
                }
            }

            // trigger the index service to index the new document.
            SendRequestToAzureSearchService(HttpMethod.Post, mediaRequest.Id);

            return mediaRequest.Id;
        }

        [HttpPost("postendofdayupdates")]
        public async Task UpdateEndOfDay([FromBody] List<ReportUpdate> updates)
        {
            var mediaRequestIds = updates.Select(e => e.Id).ToArray();

            var mediaRequests = await db.MediaRequest
                                        .Where(e => mediaRequestIds.Contains(e.Id))
                                        .ToListAsync();

            //First we updated all the Media Requests with the updates.
            foreach (ReportUpdate update in updates)
            {
                var mediaRequest = mediaRequests.SingleOrDefault(e => e.Id == update.Id);

                if (mediaRequest == null)
                    throw new ApiHttpException(HttpStatusCode.BadRequest, new KeyNotFoundException(nameof(update.Id) + " Not Found"));

                //Uses the Enum definition.
                mediaRequest.EodReportWith = update.EodReportWith;
            }

            var systemUserMe = await db.SystemUser.SingleAsync(e => e.RowGuid == UserMe.Id);

            //get all open and closed request for those ministries.
            List<Ministry> listMinistries = await db.Ministry
                                                    .Where(e => e.SystemUserMinistry.Any(f => f.SystemUserId == systemUserMe.Id && f.IsActive))
                                                    .ToListAsync();

            var localNow = DateTimeOffset.Now;

            var emails = new Dictionary<Ministry, IEnumerable<MediaRequest>>();

            foreach (var ministry in listMinistries)
            {

                //Do Null checking on EodFinalizedDateTime/EodLastRunDateTime as will be null for all new records. 
                if (ministry.EodFinalizedDateTime == null)
                {
                    ministry.EodFinalizedDateTime = localNow;
                }
                if (ministry.EodLastRunDateTime == null)
                {
                    ministry.EodLastRunDateTime = localNow;
                }
                //Need to use a variable here, since LoadNavigationProperties method can blow away updates to the Ministry object. 
                var closedSince = ministry.EodFinalizedDateTime;
                //Compare the EodLastRunDateTime Calendar Date with today's date. If todays date is in the future (i.e. it's been 24 hours since running) update the
                //EodFinalizedDateTime. 
                if (ministry.EodLastRunDateTime.Value.Date < localNow.Date)
                {
                    closedSince = ministry.EodLastRunDateTime;
                    //ministry.EodFinalizedDateTime = ministry.EodLastRunDateTime;
                }

                //Retrieve the open requests for the current ministry (all of them back through history).
                var requests = await db.MediaRequest
                        .Where(e => e.RespondedAt == null || e.RespondedAt > closedSince || e.RespondedAt >= localNow.Date)
                        .Where(e => e.LeadMinistryId == ministry.Id)
                        .Where(e => e.IsActive == true)
                        .OrderByDescending(e => e.RespondedAt)
                        .ThenByDescending(e => e.RequestedAt)
                        .ToListAsync();

                LoadNavigationProperties(requests);

                //NOTE: This code block must be below the LoadNavigationProperties calls or changes to the Ministry can be overridden. 
                //update the Report Last Run By user on the ministry object.
                ministry.EodLastRunUser = systemUserMe;
                //update the EodfinalizeDateTime - updated to closedSince local variable based on the condition above.
                ministry.EodFinalizedDateTime = closedSince;
                //Always update the last run date time when a report is run. 
                ministry.EodLastRunDateTime = DateTimeOffset.Now;

                emails.Add(ministry, requests);
            }

            await db.SaveChangesAsync();

            try
            {
                foreach (var email in emails)
                {
                    await EmailEodReport(email.Key, email.Value);
                }

                await AutoSendEodSummary();
            }
            catch
            {
                //TODO: Log this exception
            }
        }

        private async Task AutoSendEodSummary()
        {
            //Keep EodStatusVm.EodState computed in sync with changes to this logic.

            IEnumerable<EodStatusDto> summary = await MinistriesApiController.GetEodSummaryData(db);

            //now loop through the summary to see if all's good 
            Boolean allGood = true;
            var localNow = DateTimeOffset.Now;
            foreach (EodStatusDto eodSum in summary)
            {
                Boolean eodStat = false;
                if (!eodSum.LastActivity.HasValue)
                {
                    //There have been no activities opened or closed since the last report
                    eodStat = true;
                }
                else
                {
                    //The last activity date was before the last report

                    //bad. allthough...if the last activity was before the eodLastRunDate of the ministry, then we should be ok
                    eodStat = (eodSum.Ministry.EodLastRunDateTime.HasValue && eodSum.LastActivity < eodSum.Ministry.EodLastRunDateTime
                        && localNow.Date == eodSum.Ministry.EodLastRunDateTime.Value.Date);

                }

                if (!eodStat)
                {
                    allGood = eodStat;
                    break;
                }
            }

            if (allGood)
            {
                await SendEodSummary(manualRun: false);
            }
        }

        [HttpPost("postendofdaysummary")]
        public async Task EndOfDaySummary()
        {
            //use the eod dates from the GcPE HQ ministry to generate the eod report content of each of the ministries
            //make sure that new or stale eodReportWith values are defaulted appropriately.

            if (UserMe.IsAdvanced)
            {
                await SendEodSummary();
            }
            else
            {
                //not an advanced user, respond with a 403
                throw new ApiHttpException(HttpStatusCode.Forbidden, new Exception("Not an advanced user"));
            }
        }

        private async Task SendEodSummary(bool manualRun = true)
        {
            //get the dates off the gcpe hq record

            var mediaMinistry = db.Ministry
                 .Include(m => m.SystemUserMinistry).ThenInclude(m => m.SystemUser).ThenInclude(su => su.CommunicationContact)
                 .FirstOrDefault(m => m.Abbreviation == HQMinistry);

            var localNow = DateTimeOffset.Now;
            if (!manualRun && mediaMinistry.EodLastRunDateTime.HasValue && localNow.Date == mediaMinistry.EodLastRunDateTime.Value.Date)
            {
                return;
            }

            //var mediaMinistry = await mediaMinistryList.SingleOrDefaultAsync();

            //do the date magic
            //Compare the EodLastRunDateTime Calender Date with today's date. If today's date is in the future (i.e. it's been 24 hours since running) update the
            //EodFinalizedDateTime. 

            if (mediaMinistry.EodFinalizedDateTime == null)
            {
                mediaMinistry.EodFinalizedDateTime = localNow;
            }

            if (mediaMinistry.EodLastRunDateTime == null)
            {
                mediaMinistry.EodLastRunDateTime = localNow;
            }
            else if (mediaMinistry.EodLastRunDateTime.Value.Date < localNow.Date)
            {
                mediaMinistry.EodFinalizedDateTime = mediaMinistry.EodLastRunDateTime;
            }
            mediaMinistry.EodLastRunDateTime = localNow;


            //now get the list of ministries
            var ministries = await MinistriesApiController.QueryMinistries(db)
                     .Include(m => m.SystemUserMinistry).ThenInclude(m => m.SystemUser).ThenInclude(su => su.CommunicationContact) //Required for UsersController.ConvertToDto
                     .Select(m => MinistriesApiController.ConvertToDto(m, false)).ToListAsync();

            //setup the email
            string emailAddress = UserMe.EmailAddress;
            string displayAs = UserMe.DisplayAs;

            //TODO: Use populate template
            var subject = "Emerging Issues/Media Calls - " + DateTime.Today.ToString("D");
            string bodyHtml = @"<html style='font-family:Calibri, sans-serif;'>" + @"<style type=""text/css""></style>" + @"<body style='font-family:Calibri, sans-serif; font-size:12.0pt; color: black;'>";

            //Retrieve the open requests for the current ministry (all of them back through history).
            var requests = await db.MediaRequest
                    .Where(e => e.RespondedAt == null || e.RespondedAt > mediaMinistry.EodFinalizedDateTime || e.RespondedAt >= localNow.Date)
                    .Where(e => e.IsActive == true)
                    .OrderByDescending(e => e.RespondedAt)
                    .ThenByDescending(e => e.RequestedAt)
                    .ToListAsync();

            LoadNavigationProperties(requests);

            foreach (var ministry in ministries)
            {
                var ministryRequests = requests.Where(e => e.LeadMinistryId == ministry.Id);

                string header = "<span><b>{{MinistryNm}}:</b></span>";
                bodyHtml += header.Replace("{{MinistryNm}}", ministry.DisplayAs);

                if (ministryRequests.Any())
                {
                    bodyHtml += GenerateEodEmail(ministryRequests);
                }
                else
                {
                    bodyHtml += " No New Media Requests<br /><br />";
                }
            }

            await db.SaveChangesAsync(); //persist the update to the HQ ministry record.

            try
            {
                //now make a list of the media contact users' email addresses
                List<MailAddress> userList = new List<MailAddress>();

                foreach (var userMinistry in mediaMinistry.SystemUserMinistry.Where(sum => sum.IsActive))
                {
                    var usr = userMinistry.SystemUser;
                    if (!string.IsNullOrEmpty(usr.EmailAddress) && usr.IsActive && usr.CommunicationContact.Any(c => c.SortOrder != 0))
                    {
                        userList.Add(MailAddressFor(usr));
                    }
                }

                MailAddress fromAddress = new MailAddress("no-reply@gov.bc.ca", "No Reply");

                if (string.IsNullOrEmpty(emailAddress))
                    return;
#if DEBUG
                userList = new List<MailAddress>();
                userList.Add(MailAddressFor(UserMe));
#endif

                await mailProvider.SendAsync(subject, bodyHtml, fromAddress, userList);
            }
            catch
            {
                //TODO: Log this exception
            }
        }

        [HttpGet("{id}")]
        public async Task<MediaRequestDto> Get(Guid id)
        {
            var idsOnly = false;
            var mediaRequest = await QueryMediaRequestsAsync(db.MediaRequest.Where(e => e.Id == id), idsOnly);

            var singleMediaRequest = mediaRequest.SingleOrDefault();

            if (singleMediaRequest == null)
            {
                throw new ApiHttpException(HttpStatusCode.BadRequest, new KeyNotFoundException(id.ToString() + " Not Found"));
            }

            if (!singleMediaRequest.IsActive)
            {
                throw new ApiHttpException(HttpStatusCode.Gone);
            }

            return ConvertToDto(singleMediaRequest);
        }

        [HttpGet("children/{id}")]
        public int CountChildren(Guid id)
        {
            var children = db.MediaRequest
                            .Where(e => e.RequestParentId == id)
                            .Where(e => e.IsActive);
            return children.Count();
            //throw new ApiHttpException(HttpStatusCode.BadRequest, new KeyNotFoundException("Media Request ID: " + id.ToString() + " has no children."));
        }

        [HttpGet("openforme")]
        public async Task<IEnumerable<MediaRequestDto>> GetOpenForCurrentUserMinistries()
        {
            //Check if user is an advanced user. Throw an error here to make sure they can't directly navigate to the wrong
            //Reports page. If they try to navigate directly, this error will stop them. 
            //if (me.RoleId >= 3)
            //{
            //    throw new ApiHttpException(HttpStatusCode.Forbidden);
            //}

            var systemUserMe = db.SystemUser.First(e => e.RowGuid == UserMe.Id);

            //get all open and closed request for those ministries.
            var listMinistries = await db.Ministry
                                         .Where(e => e.SystemUserMinistry.Any(f => f.SystemUserId == systemUserMe.Id && f.IsActive))
                                         .ToListAsync();

            var listMinistryIds = listMinistries.Select(e => e.Id).ToArray();

            var openMediaRequests = await db.MediaRequest
                                            .Where(e => e.RespondedAt == null || e.RespondedAt >= DateTimeOffset.Now.Date)
                                            .Where(e => listMinistryIds.Contains(e.LeadMinistryId))
                                            .Where(e => e.IsActive)
                                            .OrderBy(e => e.LeadMinistry.SortOrder)
                                            .ThenBy(e => e.LeadMinistry.Abbreviation)
                                            .ThenByDescending(e => e.RespondedAt)         
                                            .ThenBy(e => e.RequestedAt)
                                            .ToListAsync();

            LoadNavigationProperties(openMediaRequests);

            //Need be a list because Enums cannot be set to null if the collection is IEnumberable
            var items = openMediaRequests.Select(e => ConvertToDto(e)).ToList();

            var localNow = DateTimeOffset.Now;

            foreach (Ministry min in listMinistries)
            {
                if (!min.EodLastRunDateTime.HasValue)
                    continue;

                foreach (var item in items.Where(mr => mr.LeadMinistry.Id == min.Id))
                {
                    if (min.EodLastRunDateTime.Value.Date < localNow.Date && item.EodReportWith !=null)
                    {
                        item.EodReportWith = null;
                    }     
<<<<<<< HEAD
                    if ( item.RespondedAt >= DateTime.Now.Date && item.EodReportWith == null)
=======
                    if ( item.RespondedAt >= localNow.Date && item.EodReportWith == null)
>>>>>>> ce9e449004407fb8a984a9ba06bed0f91aabfd82
                    {
                        item.EodReportWith = 0;
                    }
                }
            }

            return items;

        }
        [HttpPut("{id}")]
        public async Task Put(Guid id, [FromBody] MediaRequestDto dto, Boolean triggerEmail = false, Boolean onlyEmailMyself = false)
        {
            //TODO: Consider if this method should return DTO instead of Guid
            if (id != dto.Id)
                throw new ApiHttpException(HttpStatusCode.BadRequest);

            var mediaRequest = await ConvertFromDtoAsync(dto);

            db.MediaRequest.Update(mediaRequest);

            await db.SaveChangesAsync();

            if (triggerEmail || onlyEmailMyself)
            {
                try
                {
                    await EmailMediaRequest(dto, onlyEmailMyself);
                }
#if !DEBUG
                catch (Exception)
                {
                    // Don't prevent users from getting things done if there are errors sending mail.
                    // throw this.BadRequest(new System.ApplicationException("Unable to send mail for saved Media Request."));
                }
#else
                finally
                {
                }
#endif
            }

            // Trigger the document to be re-indexed.
            SendRequestToAzureSearchService(HttpMethod.Post, id);
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            var model = await db.MediaRequest.FindAsync(id);

            int numChildren = db.MediaRequest.Count(p => (p.RequestParentId == id && p.IsActive == true));
            if (numChildren > 0)
            {
                // Pass a useful error to the client
                Exception ex = new Exception(String.Format("Unable to delete MediaRequest with {0} follow-up {1}.", numChildren, (numChildren == 1 ? "request" : "requests")));
                throw new ApiHttpException(HttpStatusCode.BadRequest, ex);
            }

            //first update who modified it.
            model.ModifiedAt = DateTimeOffset.UtcNow;

            model.ModifiedBy = await db.SystemUser.SingleAsync(e => e.RowGuid == UserMe.Id);

            //then set it to inactive.
            model.IsActive = false;
            //db.MediaRequest.Remove(model);
            db.MediaRequest.Update(model);

            await db.SaveChangesAsync();

            // trigger a removal the document from the search index.
            SendRequestToAzureSearchService(HttpMethod.Delete, id);
        }

        private MediaRequestDto ConvertToDto(MediaRequest mediaRequest)
        {
            var dto = new MediaRequestDto();

            //TODO: Address scenario where join (media contacts / outlets) no longer exists.

            dto.Id = mediaRequest.Id;
            dto.CreatedAt = mediaRequest.CreatedAt;
            dto.CreatedBy = UsersApiController.ConvertToDto(mediaRequest.CreatedBy);
            dto.ModifiedAt = mediaRequest.ModifiedAt;
            dto.ModifiedBy = UsersApiController.ConvertToDto(mediaRequest.ModifiedBy);
            dto.ResponsibleUser = UsersApiController.ConvertToDto(mediaRequest.ResponsibleUser);

            if (mediaRequest.LeadMinistry != null)
            {
                dto.LeadMinistry = MinistriesApiController.ConvertToDto(mediaRequest.LeadMinistry, false);
            }
            if (mediaRequest.MediaRequestSharedMinistry != null)
            {
                dto.SharedMinistries = mediaRequest.MediaRequestSharedMinistry
                                     .Select(rq => MinistriesApiController.ConvertToDto(rq.Ministry, false));
            }

            if (mediaRequest.TakeOverRequestMinistry != null)
            {
                dto.TakeOverRequestMinistry = MinistriesApiController.ConvertToDto(mediaRequest.TakeOverRequestMinistry, false);
            }


            if (mediaRequest.MediaRequestContact != null)
            {
                dto.MediaContacts = mediaRequest.MediaRequestContact
                                     .Select(rq => MediaContactsApiController.ConvertToDto(db, rq.Contact, rq.Company))
                                     .OrderByDescending(e => e.Job.Outlet.IsMajor).ThenBy(e => e.FirstName).ThenBy(e => e.LastName);
            }

            dto.DeadlineAt = mediaRequest.DeadlineAt;
            dto.RequestTopic = mediaRequest.RequestTopic;
            dto.RequestContent = mediaRequest.RequestContent;
            dto.RequestedAt = mediaRequest.RequestedAt;
            dto.AcknowledgedAt = mediaRequest.AcknowledgedAt;
            dto.RespondedAt = mediaRequest.RespondedAt;
            dto.Response = mediaRequest.Response;
            if (mediaRequest.RequestParent != null)
            {
                dto.ParentRequest = ConvertToDto(mediaRequest.RequestParent);
            }

            dto.EodReportWith = mediaRequest.EodReportWith;
            dto.Resolution = mediaRequest.Resolution == null ? null : new ResolutionDto { Id = mediaRequest.Resolution.Id, DisplayAs = mediaRequest.Resolution.DisplayAs };

            return dto;
        }

        /// <summary>
        /// Utility function used by ConvertFromDtoAsync.  Processes any changes to the SharedMinistries field.
        /// </summary>
        /// <param name="sharedMinistries">List of shared ministries</param>
        /// <param name="Id">Id of the MediaRequest</param>
        /// <returns></returns>
        private async Task UpdateSharedMinistries(IEnumerable<MinistryDto> sharedMinistries, Guid Id)
        {
            IList<MediaRequestSharedMinistry> existingSharedMinistries = db.MediaRequestSharedMinistry.Where(e => e.MediaRequestId == Id).ToList();
            IEnumerable<MediaRequestSharedMinistry> removedSharedMinistries = existingSharedMinistries;
            if (sharedMinistries != null)
            {
                removedSharedMinistries = existingSharedMinistries.Where(e => !sharedMinistries.Any(m => m.Id == e.MinistryId));
                foreach (MinistryDto sharedMinistry in sharedMinistries)
                {
                    if (!existingSharedMinistries.Any(m => m.MinistryId == sharedMinistry.Id))
                    {
                        await db.MediaRequestSharedMinistry.AddAsync(new MediaRequestSharedMinistry
                        {
                            MediaRequestId = Id,
                            MinistryId = sharedMinistry.Id
                        });
                    }
                }
            }
            db.MediaRequestSharedMinistry.RemoveRange(removedSharedMinistries);
        }



        private async Task<MediaRequest> ConvertFromDtoAsync(MediaRequestDto dto)
        {
            MediaRequest mediaRequest;
            bool isNew = dto.Id == Guid.Empty;
            if (isNew)
            {
                mediaRequest = new MediaRequest();
                mediaRequest.Id = Guid.NewGuid();
            }
            else
            {
                mediaRequest = await db.MediaRequest.FindAsync(dto.Id);
            }

            //Mandatory fields are first name, last name, phone number, city, province, region, electoral district, media job title, and press gallery (Y/N)

            mediaRequest.ResponsibleUser = await db.SystemUser.SingleAsync(e => e.RowGuid == dto.ResponsibleUser.Id);

            mediaRequest.ResponsibleUserId = mediaRequest.ResponsibleUser.Id; // We save using the Id, not the full object.
            mediaRequest.DeadlineAt = dto.DeadlineAt?.ToUniversalTime();
            mediaRequest.RequestTopic = dto.RequestTopic?.Trim();
            mediaRequest.RequestContent = dto.RequestContent?.Trim();
            mediaRequest.RequestedAt = dto.RequestedAt.ToUniversalTime();
            mediaRequest.RequestParentId = dto.ParentRequest?.Id;
            mediaRequest.AcknowledgedAt = dto.AcknowledgedAt?.ToUniversalTime();
            mediaRequest.RespondedAt = dto.RespondedAt?.ToUniversalTime();
            mediaRequest.Response = dto.Response?.Trim();

            //TODO: Test if this can simply be mediaRequest.Resolution = dto.Resolution.Id instead of requiring a lookup;
            mediaRequest.Resolution = dto.Resolution == null ? null : await db.MediaRequestResolution.FindAsync(dto.Resolution.Id);
            mediaRequest.LeadMinistryId = dto.LeadMinistry.Id;

            await UpdateSharedMinistries(dto.SharedMinistries, mediaRequest.Id);

            if (dto.TakeOverRequestMinistry != null)
            {
                mediaRequest.TakeOverRequestMinistryId = dto.TakeOverRequestMinistry.Id;
            }
            else
            {
                mediaRequest.TakeOverRequestMinistry = null;
                mediaRequest.TakeOverRequestMinistryId = null;
            }

            IList<MediaRequestContact> existingMediaRequestContacts = db.MediaRequestContact.Where(e => e.MediaRequestId == mediaRequest.Id).ToList();
            IEnumerable<MediaRequestContact> removedMediaRequestContacts = existingMediaRequestContacts.Where(e => !dto.MediaContacts.Any(mc => mc.Id == e.ContactId));
            db.MediaRequestContact.RemoveRange(removedMediaRequestContacts);

            foreach (MediaContactDto mediaContact in dto.MediaContacts)
            {
                // a contact belonging to 2 outlets can only appear once in a media request (enforced by primary key)
                MediaRequestContact rq = existingMediaRequestContacts.SingleOrDefault(mc => mc.ContactId == mediaContact.Id);

                if (rq == null)
                {
                    rq = new MediaRequestContact
                    {
                        MediaRequestId = mediaRequest.Id,
                        ContactId = mediaContact.Id
                    };
                    db.MediaRequestContact.Add(rq);
                }
                rq.CompanyId = mediaContact.Job.Outlet.Id;
            }

            mediaRequest.ModifiedAt = DateTimeOffset.UtcNow;
            mediaRequest.ModifiedBy = db.SystemUser.Single(e => e.RowGuid == UserMe.Id);

            return mediaRequest;
        }

        private async Task EmailEodReport(Ministry ministry, IEnumerable<MediaRequest> requests)
        {
            if (string.IsNullOrEmpty(UserMe.EmailAddress))
                return;

            //MailboxAddress toContact = new MailboxAddress(ministry.ContactUser.DisplayName, ministry.ContactUser.EmailAddress);

            //TODO: Use populate template.
            var subjectTemplate = "{{MinistryAbbreviation}}: End of Day Media Request Report";

            string subject = subjectTemplate.Replace("{{MinistryAbbreviation}}", ministry.Abbreviation);
            string bodyHtml = @"<html style='font-family:Calibri, sans-serif;'>" + @"<style type=""text/css""></style>" + @"<body style='font-family:Calibri, sans-serif; font-size:12.0pt; color: black;'>";
            bodyHtml += GenerateEodEmail(requests);

            await mailProvider.SendAsync(subject, bodyHtml, MailAddressFor(UserMe));

        }

        private async Task EmailMediaRequest(MediaRequestDto dto, Boolean onlyEmailMyself = false)
        {
            if (UserMe.EmailAddress == null)
                throw new Exception("User cannot send email");

            //Always going to be from/BCC to,  the currently logged in user. 
            var myContact = MailAddressFor(UserMe);

            //var subjectTemplate = "{{MinistryAbbreviation}} Media Request: {{OutletName}} - {{ReporterName}} - {{RequestTopic}}";
            var subjectTemplate = "{{MinistryAbbreviation}} " + (dto.ParentRequest == null ? "" : "Follow-up ") + "Media Request: {{RequestTopic}}";
            //TODO: Use PopulateTemplate(subjectTemplate, dto);

            string subject = subjectTemplate.Replace("{{MinistryAbbreviation}}", dto.LeadMinistry.Abbreviation).Replace("{{RequestTopic}}", dto.RequestTopic);

            string bodyHtml = GenerateEmail(dto, onlyEmailMyself);

            var toList = new List<MailAddress>();
            var ccList = new List<MailAddress>();

            //This is a new or updated Media Request
            //Email responsible user from the currently logged in user, no CC required. 

            //Responsible User Email
            toList.Add(MailAddressFor(dto.ResponsibleUser));

            //is this media request being shared with other ministries?
            //If so email Primary Contact and CC - Secondary Contact from/BCC the currently logged in user for each ministry. 
            foreach (var minDto in dto.SharedMinistries)
            {
                var ministry = db.Ministry
                                .Where(e => e.Id == minDto.Id)
                                .Include(e => e.ContactUser)
                                .Include(e => e.SecondContactUser)
                                .FirstOrDefault();

                //This is the primary contact.  As of 2017-11-03 this is now put into the CC list.
                if (ministry.ContactUser != null)
                    ccList.Add(MailAddressFor(ministry.ContactUser));

                //This is the secondary contact
                if (ministry.SecondContactUser != null)
                    ccList.Add(MailAddressFor(ministry.SecondContactUser));
            }

            if (dto.CommContacts.Any())
            {
                foreach (var userDto in dto.CommContacts)
                {
                    ccList.Add(new MailAddress(userDto.EmailAddress, userDto.DisplayAs));
                }
            }

            //List<MailboxAddress> fromContacts = new List<MailboxAddress>();
            //fromContacts.Add(fromContact);
            //await mailProvider.SendAsync(subject, bodyHtml, fromContacts, fromContact, fromContacts);

            //Commented out for testing.
            //List<MailboxAddress> fromContacts = new List<MailboxAddress>();
            //fromContacts.Add(fromContact);
            //await mailProvider.SendAsync(subject, bodyHtml, fromContacts, fromContact);

            if (!onlyEmailMyself)
            {
                await mailProvider.SendAsync(subject, bodyHtml, myContact, toList, ccList);
            }
            else
            {
                var toMyself = new List<MailAddress>();
                toMyself.Add(myContact);
                await mailProvider.SendAsync(subject, bodyHtml, myContact, toMyself, null);
            }


            // send any pending take over requests.
            if (dto.TakeOverRequestMinistry != null)
            {
                await EmailTakeOverRequest(subject, dto, myContact, bodyHtml);
            }

        }

        private async Task EmailTakeOverRequest(string subject, MediaRequestDto dto, MailAddress myContact, string bodyHtml)
        {
            string messageSubject = "Take over as lead for " + subject;

            var toList = new List<MailAddress>();
            var ccList = new List<MailAddress>();
            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = this.Request.Scheme,
                Host = this.Request.Host.Host,
                Path = "/MediaRequests/request/" + dto.Id.ToString() // Can't use the current path, as it is an api reference.
            };

            if (this.Request.Host.Port != null)
            {
                uriBuilder.Port = (int)this.Request.Host.Port;
            }

            string url = uriBuilder.Uri.ToString();
            string messageBody = "<p><a href=\"" + url + "\">Respond to this Takeover Request</a></p>" + bodyHtml;

            ccList.Add(myContact);

            var ministry = db.Ministry
                            .Where(e => e.Id == dto.TakeOverRequestMinistry.Id)
                            .Include(e => e.ContactUser)
                            .Include(e => e.SecondContactUser)
                            .FirstOrDefault();

            //This is the primary contact.  Put into the To address as they are the primary recipient.
            if (ministry.ContactUser != null)
                toList.Add(MailAddressFor(ministry.ContactUser));

            //This is the secondary contact
            if (ministry.SecondContactUser != null)
                ccList.Add(MailAddressFor(ministry.SecondContactUser));

            await mailProvider.SendAsync(messageSubject, messageBody, myContact, toList, ccList);

        }

        private static string FormatTelephone(string telephone, string extension)
        {
            if (telephone == null)
                return "";

            telephone = telephone.Trim();

            if (telephone.Length == 10)
            {
                telephone = telephone.Substring(0, 3) + "-" + telephone.Substring(3, 3) + "-" + telephone.Substring(6, 4);
            }

            return string.IsNullOrEmpty(extension) ? telephone : telephone + " x " + extension;
        }

        private static string FormatTelephone(string telephone)
        {
            if (telephone == null)
                return "";

            telephone = telephone.Trim();

            if (telephone.Length == 10)
            {
                telephone = telephone.Substring(0, 3) + "-" + telephone.Substring(3, 3) + "-" + telephone.Substring(6, 4);
            }

            return telephone;
        }

        private string GenerateEodEmail(IEnumerable<MediaRequest> requests)
        {
            var openMediaRequests = requests.Where(e => !e.RespondedAt.HasValue);

            var closedMediaRequests = requests.Where(e => e.RespondedAt.HasValue);

            var sb = new StringBuilder();

            //sb.Append(@"<html style='font-family:Calibri, sans-serif;'>" + @"<style type=""text/css""></style>" + @"<body style='font-family:Calibri, sans-serif; font-size:11.0pt; color: black;'>");
            sb.Append(@"<div style='font-family:Calibri, sans-serif;'>");

            //sb.Append("<h1>End of Day Media Request Report</h1>");
            //if (openMediaRequests.Count > 0)
            //{
            //    sb.Append("Open:");
            //}
            //else
            //{
            //    sb.Append("No Open Media Requests");
            //}
            // sb.Append("<table style='font-family:Calibri, sans-serif;'>");

            string mediaString = "";
            foreach (MediaRequest mr in openMediaRequests)
            {
                //mediaString += "<td style=\"padding-right: 25px; vertical-align:top;\">Reporters: </td>";
                //mediaString += "<td>{{Contacts}}</td>";
                //mediaString += "<td style=\"padding-right: 25px; vertical-align:top;\">Issue: </td>";
                //mediaString += "<td>{{Topic}}</td>";
                //mediaString += "<td style=\"padding-right: 25px; vertical-align:top;\">Status:</td>";
                //mediaString += "<td>With {{EodReportWith}}</td>";
                //mediaString += "<tr style=\"margin-bottom: 10px;\"><td></td><tr></td></tr>";
                string placeholder = "";
                if (mr.EodReportWith.HasValue)
                {
                    placeholder += "With ";
                }
                else
                {
                    placeholder += "Unknown ";
                }
                mediaString += "<p>{{Contacts}}ISSUE: {{Topic}}<br />STATUS: " + placeholder + "{{EodReportWith}}<br /></p>";
                mediaString = mediaString.Replace("{{EodReportWith}}", EodReportWithString(mr.EodReportWith));
                string contacts = "";
                //TODO: Change to a for loop
                //foreach (MediaRequestContact contact in mr.MediaRequestContact)
                //{  
                //    contacts += contact.Contact.FirstName + " " + contact.Contact.LastName + " - " + contact.Company.CompanyName;
                //}

                for (int i = 0; i < mr.MediaRequestContact.Count; i++)
                {
                    var array = mr.MediaRequestContact.ToArray();
                    if (i >= 1)
                        contacts += "; ";
                    //contacts += "<b>" + array[i].Contact.FirstName + " " + array[i].Contact.LastName + "</b>" + " - " + array[i].Company.CompanyName;
                    contacts += array[i].Contact.FirstName + " " + array[i].Contact.LastName + " - " + array[i].Company.CompanyName;
                }
                mediaString = mediaString.Replace("{{Contacts}}", contacts + "<br />");
                mediaString = mediaString.Replace("{{Topic}}", mr.RequestTopic);
            }
            sb.Append(mediaString);

            //sb.Append("</table>");
            sb.Append("</div>");

            //if (closedMediaRequests.Count > 0)
            //{
            //    sb.Append("Closed:");
            //}
            //else
            //{
            //    sb.Append("No Closed Media Requests");
            //}
            //sb.Append("<table style='font-family:Calibri, sans-serif;'>");
            sb.Append("<div style='font-family:Calibri, sans-serif;'>");

            string closedMediaString = "";
            foreach (MediaRequest mr in closedMediaRequests)
            {
                closedMediaString += "<p>{{Contacts}}ISSUE: {{Topic}}<br />STATUS: {{Resolution}}<br /><ul style=\"margin-top: -8px;\"><li>{{Response}}</li></ul></p>";
                closedMediaString = closedMediaString.Replace("{{EodReportWith}}", mr.EodReportWith.ToString());
                string contacts = "";
                //foreach (MediaRequestContact contact in mr.MediaRequestContact)
                //{
                //    contacts += contact.Contact.FirstName + " " + contact.Contact.LastName + ", " + contact.Company.CompanyName + "<br />";
                //}
                for (int i = 0; i < mr.MediaRequestContact.Count; i++)
                {
                    var array = mr.MediaRequestContact.ToArray();
                    if (i >= 1)
                        contacts += "; ";
                    contacts += array[i].Contact.FirstName + " " + array[i].Contact.LastName + " - " + array[i].Company.CompanyName;
                }
                closedMediaString = closedMediaString.Replace("{{Contacts}}", contacts + "<br />");
                closedMediaString = closedMediaString.Replace("{{Topic}}", mr.RequestTopic);
                closedMediaString = closedMediaString.Replace("{{Resolution}}", mr.Resolution.DisplayAs);
                closedMediaString = closedMediaString.Replace("{{Response}}", mr.Response.Replace("\r\n", "\n").Replace("\n", "<br />"));
            }
            sb.Append(closedMediaString);
            sb.Append("</Div></body></html>");
            //sb.Append("</table></body></html>");

            return sb.ToString();

        }

        private string GenerateEmail(MediaRequestDto dto, Boolean onlyEmailMyself)
        {
            var isClosed = dto.RespondedAt != null;

            string template = "";
            //closedMediaString += "<p>{{Contacts}}ISSUE: {{Topic}}<br />STATUS: {{Resolution}}<br /><ul style=\"margin-top: -8px;\"><li>{{Response}}</li></ul></p>"
            template += "<p style=\"padding-right: 8px; vertical-align:top;\">";
            template += "<b>Deadline</b>  ";
            template += "<span style=\"color:black;\">&#8203;</span><span style=\"color:{{DeadlineColor}};\">{{Deadline}}</span><br />";
            if (isClosed) // add responded date to closed request
            {
                template += "<b>Responded</b>  ";
                template += "<span style=\"color:black;\">&#8203;</span>{{RespondedAt}}<br />";
            }
            template += "<br /><b>Request</b><br />";
            template += "<span style=\"color:black;\">{{Request}}</span><br />";
            if (isClosed) // add response to closed request
            {
                template += "<br /><b>Response</b><br />";
                template += "<span style=\"color:black;\">{{Response}}</span><br />";
            }
            if (!isClosed)  // only add recommendation to open requests, remove if closed
            {
                template += "<br /><b>Recommendation</b>";
                template += "<span style=\"color:black;\"></span>";
                template += "</p>";
            }
            if (!isClosed || (isClosed && dto.ParentRequest != null)) // only add background to open requests, remove if closed (or sending a closed follow-up to myself)
            {
                template += "<br /><b>Background</b><br />";
                template += "<span style=\"color:black;\">{{Background}}</span><br />";
            }



            var sb = new StringBuilder();

            sb.Append(@"<html style='font-family:Calibri, sans-serif;'>" + @"<style type=""text/css""> td { font-family: Calibri, sans-serif; }</style>" + @"<body style='font-family:Calibri, sans-serif; font-size:11.0pt; color: black;'>");


            sb.Append(@"<div style='font-family:Calibri, sans-serif;'>");
            sb.Append(@"<b>" + (dto.MediaContacts.Count() == 1 ? "Reporter" : "Reporters") + " </b>");
            sb.Append("<div style=\"margin-left: 20px;\"> ");
            string reporters = "";

            foreach (var mediaContact in dto.MediaContacts.OrderByDescending(e => e.Job.Outlet.IsMajor).ThenBy(e => e.FirstName).ThenBy(e => e.LastName))
            {
                string reporter = "{{ReporterName}}, {{ReporterTitle}}<br />{{OutletName}}<br />{{ReporterEmailLine}}{{ReporterTelephoneLine}}";

                reporter = reporter.Replace("{{OutletName}}", mediaContact.Job.Outlet.Name);
                reporter = reporter.Replace("{{ReporterName}}", mediaContact.FirstName + " " + mediaContact.LastName);
                reporter = reporter.Replace("{{ReporterTitle}}", mediaContact.Job.Title);

                bool showWorkPhone = !string.IsNullOrEmpty(mediaContact.WorkPhone);
                bool showCellPhone = !string.IsNullOrEmpty(mediaContact.CellPhone);

                string telephone = "";
                if (showWorkPhone && showCellPhone)
                {
                    telephone = FormatTelephone(showWorkPhone ? mediaContact.WorkPhone : null, showWorkPhone ? mediaContact.WorkPhoneExtension : null);
                    telephone += " c: " + FormatTelephone(showCellPhone ? mediaContact.CellPhone : null);
                }
                else if (showWorkPhone)
                {
                    telephone = FormatTelephone(showWorkPhone ? mediaContact.WorkPhone : null, showWorkPhone ? mediaContact.WorkPhoneExtension : null);
                }
                else if (showCellPhone)
                {
                    telephone = FormatTelephone(showCellPhone ? mediaContact.CellPhone : null);
                }

                reporter = reporter.Replace("{{ReporterTelephoneLine}}", string.IsNullOrEmpty(telephone) ? "" : (telephone + "<br />"));

                reporter = reporter.Replace("{{ReporterEmailLine}}", string.IsNullOrEmpty(mediaContact.Email) ? "" : (mediaContact.Email + "<br />"));
                //template.Insert(0, mediaContactTemplate);

                reporters += (reporters == "" ? "" : "<br />") + reporter;
            }

            sb.Append(reporters);
            sb.Append("</div>");
            //sb.Append("</td></tr>");

            var sharedMinistries = new StringBuilder();
            foreach (var ministry in dto.SharedMinistries)
            {
                //would be cool to make these look like the tags. 
                sharedMinistries.Append(ministry.Abbreviation + " - " + ministry.DisplayAs + "<br />");
            }

            string takeOverRequestMinistry = "";
            if (dto.TakeOverRequestMinistry != null)
            {
                takeOverRequestMinistry = dto.TakeOverRequestMinistry.Abbreviation + " - " + dto.TakeOverRequestMinistry.DisplayAs + "<br />";
            }

            template = template.Replace("{{MinistryAbbreviation}}", dto.LeadMinistry.Abbreviation);
            template = template.Replace("{{RequestTopic}}", dto.RequestTopic);
            template = template.Replace("{{ResponsibleUser}}", dto.ResponsibleUser.DisplayAs);
            template = template.Replace("{{SharedMinistries}}", sharedMinistries.ToString());
            template = template.Replace("{{TakeOverRequestMinistry}}", takeOverRequestMinistry);

            if (dto.DeadlineAt.HasValue)
            {
                template = template.Replace("{{DeadlineColor}}", "red");
                template = template.Replace("{{Deadline}}", dto.DeadlineAt?.DateTime.ToLocalTime().ToString("f"));
            }
            else //removing ASAP
            {
                //template = template.Replace("{{DeadlineColor}}", "black");
                template = template.Replace("{{DeadlineColor}}", "red");
                template = template.Replace("{{Deadline}}", "ASAP");
            }

            if (dto.RespondedAt.HasValue)
            {
                template = template.Replace("{{RespondedAt}}", dto.RespondedAt?.DateTime.ToLocalTime().ToString("f"));
            }

            template = template.Replace("{{MinistryName}}", dto.LeadMinistry.DisplayAs);
            template = template.Replace("{{RequestedAt}}", dto.RequestedAt.DateTime.ToLocalTime().ToString("f"));
            template = template.Replace("{{Request}}", dto.RequestContent.Replace("\r\n", "\n").Replace("\n", "<br />"));
            if (isClosed) template = template.Replace("{{Response}}", dto.Response.Replace("\r\n", "\n").Replace("\n", "<br />")); // add the response to closed requests


            string background = "";

            if (dto.ParentRequest != null)
            {
                background = "This is a follow-up to a media request from " + dto.ParentRequest.RequestedAt.ToString("MMMM d, yyyy") + "." + "<br />";
                background += "<br />";
                background += "<div style=\"margin-left: 20px;\">";
                background += "<i>REQUEST:</i>" + "<br />";
                background += dto.ParentRequest.RequestContent.Replace("\r\n", "\n").Replace("\n", "<br />");
                //background += "<br />";

                if (dto.ParentRequest.RespondedAt.HasValue)
                {
                    background += "<br /><br />";
                    background += "<i>RESPONSE:</i>" + "<br />";
                    background += dto.ParentRequest.Resolution.DisplayAs.Replace("\r\n", "\n").Replace("\n", "<br />") + "<br />";
                    background += dto.ParentRequest.Response.Replace("\r\n", "\n").Replace("\n", "<br />");
                }
                background += "</div>";
            }

            if (!isClosed || (isClosed && dto.ParentRequest != null)) template = template.Replace("{{Background}}", background); // add the background to open requests

            sb.Append(template);

            sb.Append("</div></body></html>");

            return sb.ToString();
        }
        //TODO: This should be eliminated and the ENUM type built into the database to be automatically returned as a string
        private string EodReportWithString(Data.Entity.EodReportWith? reportWith)
        {
            string result = "";

            if (reportWith.HasValue)
            {
                switch (reportWith.Value)
                {
                    case EodReportWith.CommOffice:
                        result = "GCPE";
                        break;
                    case EodReportWith.MinistersOffice:
                        result = "Minister's Office";
                        break;
                    case EodReportWith.ProgramArea:
                        result = "Program Area";
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return result;

        }

        [HttpGet("resolutions")]
        public async Task<IEnumerable<ResolutionDto>> GetResolutions()
        {
            var resolutions = await db.MediaRequestResolution.Select(e => new ResolutionDto
            {
                Id = e.Id,
                DisplayAs = e.DisplayAs
            }).OrderBy(r => r.DisplayAs).ToListAsync();

            return resolutions;
        }

        public MailAddress MailAddressFor(SystemUser systemUser)
        {
            return new MailAddress(systemUser.EmailAddress, systemUser.FullName);
        }

        public MailAddress MailAddressFor(UserDto user)
        {
            return new MailAddress(user.EmailAddress, user.DisplayAs);
        }

    }
}