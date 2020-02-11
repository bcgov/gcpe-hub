using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Gcpe.Hub.Data.Entity;
using Gcpe.Hub.Services.Legacy;
using Gcpe.Hub.WebApp.Providers;
using Gcpe.Hub.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gcpe.Hub.WebApp.Controllers
{
    using Gcpe.Hub.Services.Legacy.Models;
    using static Utility;

    [Route("api/mediacontacts")]
    public class MediaContactsApiController : BaseController
    {
        private ISubscribe subscribe;

        static Guid? workPhoneGuid;
        static Guid? cellPhoneGuid;
        static Guid? emailGuid;
        static Dictionary<Guid, string> mediaJobTitles;
        const int duplicateSqlKey = 2601;

        public MediaContactsApiController(HubDbContext db, ISubscribeClient subscribeClient) : base(db)
        {
            subscribe = subscribeClient.Subscribe;
        }

        [HttpGet("outlets")]
        public Task<OutletDto[]> GetOutlets()
        {

            return db.Company
                     .Where(cmp => cmp.IsOutlet)
                     .Where(cmp => cmp.IsActive)
                     .OrderBy(cmp => cmp.CompanyName)
                     .Select(cmp => new OutletDto
                     {
                         Id = cmp.Id,
                         Name = cmp.CompanyName,
                         IsMajor = cmp.IsMajorMedia.HasValue ? cmp.IsMajorMedia.Value : false
                     }).ToArrayAsync();
        }

        private async Task<Contact> GetOrCreateContactAsync(System.Linq.Expressions.Expression<Func<Contact, bool>> criteria)
        {
            var c = await db.Contact
                      .Include(e => e.ContactMediaJobTitle)
                      .FirstOrDefaultAsync(criteria);

            if (c == null)
            {
                c = new Contact() { Id = Guid.NewGuid(), ShowNotes = "", CreationDate = DateTime.Now };
                db.Contact.Add(c);
            }
            else
            {
                string inClause = SqlHelper.ToInClause(new List<Guid>() { c.Id });
                SqlHelper.LoadContactNavigationProperties(inClause, db);
                c.IsActive = true; // reactivate if it was rejected
                db.Contact.Update(c);
            }
            return c;
        }

        /*[HttpGet("{contactId}/{outletId}")]
        public async Task<MediaContactDto> Get(Guid contactId, Guid outletId)
        {
            ContactCompany contact = await GetContactCompanyAsync(contactId, outletId);
            if (contact == null)
                throw new ApiHttpException(HttpStatusCode.NotFound);

            return ConvertToDto(db, contact.Contact, contact.Company);
        }*/

        [HttpGet("titles")]
        public Task<string[]> GetTitles()
        {
            return GetTitles(db);
        }
        public static Task<string[]> GetTitles(HubDbContext db)
        {
            return db.MediaJobTitle
                     .OrderBy(e => e.SortOrder).ThenBy(e => e.MediaJobTitleName)
                     .Select(e => e.MediaJobTitleName)
                     .ToArrayAsync();
        }

        private void ValidateDtoRequiredFields(MediaContactDto dto)
        {
            if (dto.FirstName == null)
            {
                throw new ApiHttpException(HttpStatusCode.BadRequest, new ArgumentNullException(nameof(dto.FirstName)));
            }
            if (dto.LastName == null)
            {
                throw new ApiHttpException(HttpStatusCode.BadRequest, new ArgumentNullException(nameof(dto.LastName)));
            }
            if (dto.Job == null)
            {
                throw new ApiHttpException(HttpStatusCode.BadRequest, new ArgumentNullException(nameof(dto.Job)));
            }
            if (dto.Job.Title == null)
            {
                throw new ApiHttpException(HttpStatusCode.BadRequest, new ArgumentNullException(nameof(dto.Job.Title)));
            }
            if (dto.Job.Outlet == null)
            {
                throw new ApiHttpException(HttpStatusCode.BadRequest, new ArgumentNullException(nameof(dto.Job.Outlet)));
            }
        }

        [HttpPost]
        public async Task<MediaContactDto> Post([FromBody]MediaContactDto dto)
        {
            ValidateDtoRequiredFields(dto);

            await SaveContactDtoAsync(dto, true);

            //TODO Throw security exception if users provide a created time/user etc.
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as SqlException;
                if (sqlException == null || sqlException.Number != duplicateSqlKey) throw;
                throw new ApiHttpException(HttpStatusCode.BadRequest, new Exception("This contact already exists in the database"));
            }

            return dto;
        }

        public async Task<Guid> CreateOutletAsync(OutletDto dto)
        {
            dto.Name = dto.Name.Trim();
            // Check first if this outlet has been rejected and can be re-activated
            Company outlet = await db.Company.FirstOrDefaultAsync(o => o.CompanyName == dto.Name && o.IsOutlet);
            if (outlet == null)
            {
                outlet = new Company();
                outlet.Id = Guid.NewGuid();
                outlet.CompanyName = dto.Name;
                outlet.CompanyDescription = " ";
                outlet.IsOutlet = true;
                outlet.CreationDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                outlet.ModifiedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                db.Company.Add(outlet);
                WriteActivityLogEntry(outlet, "", 4); //ActivityType.Record_Created
            }
            else
            {
                db.Company.Update(outlet);
            }
            outlet.IsActive = true;
            outlet.RecordEditedBy = UserMe.UserDomainName;

            return outlet.Id;
        }

        [HttpPut("{id}")]
        public async Task<MediaContactDto> Put(Guid id, [FromBody]MediaContactDto dto)
        {
            if (id != dto.Id)
                throw new ApiHttpException(HttpStatusCode.BadRequest);

            await SaveContactDtoAsync(dto, false);

            await db.SaveChangesAsync();

            return dto;
        }

        [HttpGet]
        public async Task<List<MediaContactDto>> Get(string filter = "")
        {
            //TODO: Allow client to specify sort options (https://gcpe.visualstudio.com/Hub/_workitems?id=2232)

            List<MediaContactDto> results = new List<MediaContactDto>();

            // restore first name search ..  but only first 2 characters for fuzzy matching
            string s = "(CONTAINS(CompanyName, '\"*{0}*\"') OR CONTAINS(LastName, '\"{0}*\"') OR CONTAINS(FirstName, '\"{1}*\"'))";
            string whereClause = SqlHelper.CreateSearchClause(filter, s);

            var contactsQuery = db.ContactMediaJobTitle
                                .FromSqlRaw("SELECT cc.* FROM media.ContactMediaJobTitle cc LEFT JOIN media.Contact c ON cc.ContactId = c.Id LEFT JOIN media.Company cp ON cc.CompanyId = cp.Id" + whereClause)
                                .Include(e => e.Contact)
                                .Where(e => e.Contact.IsActive)
                                .Where(e => !e.Contact.MinisterialJobTitleId.HasValue)
                                .AsQueryable();


            var contactJobTitles = await contactsQuery.ToListAsync();

            if (contactJobTitles.Count == 0) return results;

            var contactsGuids = new List<Guid>();
            var outletGuids = new List<Guid>();
            foreach (var contactJobTitle in contactJobTitles)
            {
                if (!contactsGuids.Contains(contactJobTitle.ContactId))
                    contactsGuids.Add(contactJobTitle.ContactId);
                if (!outletGuids.Contains(contactJobTitle.CompanyId))
                    outletGuids.Add(contactJobTitle.CompanyId);
            }
            db.Company.FromSqlRaw("SELECT * FROM media.Company WHERE Id " + SqlHelper.ToInClause(outletGuids)).Load();
            SqlHelper.LoadContactNavigationProperties(SqlHelper.ToInClause(contactsGuids), db);

            foreach (ContactMediaJobTitle contactJob in contactJobTitles.OrderBy(e => e.Contact.FirstName).ThenBy(e => e.Contact.LastName).ThenBy(e => e.Company.CompanyName))
            {
                results.Add(ConvertToDto(db, contactJob));
            }

            return results;
        }

        /// <summary>
        /// Return a specific Contact.  Used by the integration test
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The contact, or null if it does not exist</returns>
        [HttpGet("{id}")]
        public MediaContactDto GetById(Guid id)
        {
            MediaContactDto result = null;

            Contact contact = db.Contact
                .Include(x => x.ContactMediaJobTitle)
                .Include(x => x.ContactPhoneNumber)
                .Include(x => x.ContactWebAddress) // needed for email addresses
                .Include(x => x.Ministry)
                .Include(x => x.MinisterialJobTitle)
                .FirstOrDefault(x => x.Id == id);

            if (contact != null)
            {
                Company outlet = db.Company.FirstOrDefault(x => x.Id == contact.ContactMediaJobTitle.SingleOrDefault().CompanyId);

                result = ConvertToDto(db, contact, outlet);
            }

            return result;
        }


        public static MediaContactDto ConvertToDto(HubDbContext db, Contact contact, Company outlet)
        {
            // It makes sense to me that a contact shouldn't have more than 1 job within the SAME outlet
            ContactMediaJobTitle contactJob;
            if (contact.ContactMediaJobTitle.Count == 1)
            {
                contactJob = contact.ContactMediaJobTitle.SingleOrDefault();

                // display the outlet of historical requests
                contactJob.Company = outlet;
                contactJob.CompanyId = outlet.Id;
            }
            else
            {
                contactJob = contact.ContactMediaJobTitle.FirstOrDefault(e => e.CompanyId.ToString() == outlet.Id.ToString());
                if (contactJob == null)
                {
                    var contactOldJob = contact.ContactMediaJobTitle.FirstOrDefault();
                    contactJob = new ContactMediaJobTitle { Contact = contact, CompanyId = outlet.Id, Company = outlet };
                    if (contactOldJob != null)
                    {
                        contactJob.MediaJobTitleId = contactOldJob.MediaJobTitleId;
                    }
                }
            }

            return ConvertToDto(db, contactJob);
        }

        private static MediaContactDto ConvertToDto(HubDbContext db, ContactMediaJobTitle contactJob)
        {
            var dto = new MediaContactDto();
            Contact contact = contactJob.Contact;

            dto.Id = contact.Id;
            dto.FirstName = contact.FirstName;
            dto.LastName = contact.LastName;

            // Work phone
            Guid workPhoneGuid = GetWorkPhoneGuid(db);
            var phoneNumber = contact.ContactPhoneNumber.FirstOrDefault(e => e.PhoneTypeId == workPhoneGuid);
            if (phoneNumber != null)
            {
                dto.WorkPhone = phoneNumber.PhoneNumber;
                dto.WorkPhoneExtension = phoneNumber.PhoneNumberExtension;
            }

            // Cell phone
            Guid cellPhoneGuid = GetCellPhoneGuid(db);
            dto.CellPhone = contact.ContactPhoneNumber.FirstOrDefault(e => e.PhoneTypeId == cellPhoneGuid)?.PhoneNumber ?? "";

            // e-mail
            Guid emailId = GetEmailGuid(db);
            dto.Email = contact.ContactWebAddress.FirstOrDefault(e => e.WebAddressTypeId == emailId)?.WebAddress ?? "";

            // Job Title
            dto.Job = new MediaJobDto()
            {
                Id = contactJob.Id,
            };
            var contactJobTitleId = contactJob.MediaJobTitleId;
            foreach (var jobTitle in GetAllMediaJobTitles(db))
            {
                if (contactJobTitleId == jobTitle.Key)
                {
                    dto.Job.Title = jobTitle.Value;
                }
            }

            dto.Job.Outlet = new OutletDto()
            {
                Id = contactJob.CompanyId,
                Name = contactJob.Company?.CompanyName,
                IsMajor = contactJob.Company?.IsMajorMedia == true
            };
            return dto;
        }

        private static Guid GetWorkPhoneGuid(HubDbContext db)
        {
            if (!workPhoneGuid.HasValue)
            {
                //TODO: This call should be SingleAsync; change will require refactoring of ref argument in ConvertToDto method
                var phoneType = db.PhoneType.Single(e => e.PhoneTypeName == "Primary");
                workPhoneGuid = phoneType.Id;
            }
            return workPhoneGuid.Value;
        }

        private static Guid GetCellPhoneGuid(HubDbContext db)
        {
            if (!cellPhoneGuid.HasValue)
            {
                //TODO: This call should be SingleAsync; change will require refactoring of ref argument in ConvertToDto method
                var phoneType = db.PhoneType.Single(e => e.PhoneTypeName == "Cell");
                cellPhoneGuid = phoneType.Id;
            }
            return cellPhoneGuid.Value;
        }

        private static Guid GetEmailGuid(HubDbContext db)
        {
            if (!emailGuid.HasValue)
            {
                //TODO: This call should be SingleAsync; change will require refactoring of ref argument in ConvertToDto method
                var addressType = db.WebAddressType.Single(e => e.WebAddressTypeName == "Email Address");
                emailGuid = addressType.Id;
            }
            return emailGuid.Value;
        }
        private static Dictionary<Guid, string> GetAllMediaJobTitles(HubDbContext db)
        {
            if (mediaJobTitles == null)
            {
                mediaJobTitles = db.MediaJobTitle.OrderBy(e => e.SortOrder).ThenBy(e => e.MediaJobTitleName).ToDictionary(t => t.Id, t => t.MediaJobTitleName);
            }
            return mediaJobTitles;
        }

        private async Task SaveContactDtoAsync(MediaContactDto dto, bool isNew)
        {
            MediaJobDto jobDto = dto.Job;
            dto.FirstName = dto.FirstName.Trim();
            dto.LastName = dto.LastName.Trim();
            DateTime now = DateTime.Now;

            System.Linq.Expressions.Expression<Func<Contact, bool>> criteria;
            if (isNew)
            {
                // Check if this contact has been rejected and can be re-activated
                criteria = c => !c.IsActive && c.FirstName == dto.FirstName && c.LastName == dto.LastName;
            }
            else
            {
                criteria = c => c.Id == dto.Id;
            }

            Contact contact = await GetOrCreateContactAsync(criteria);
            dto.Id = contact.Id;
            if (jobDto.Outlet.Id == Guid.Empty)
            {
                jobDto.Outlet.Id = await CreateOutletAsync(jobDto.Outlet);
            }

            ContactMediaJobTitle contactJob = contact.ContactMediaJobTitle.SingleOrDefault(j => j.CompanyId == jobDto.Outlet.Id);

            if (isNew && contactJob == null)
            {
                contactJob = new ContactMediaJobTitle { Id = Guid.NewGuid(), ContactId = contact.Id, Contact = contact };
                contact.ContactMediaJobTitle.Add(contactJob);
                contact.FirstName = dto.FirstName;
                contact.LastName = dto.LastName;
                if (isNew)
                {
                    WriteActivityLogEntry(contact, "", 4); //  4=ActivityType.Record_Created
                }
                jobDto.Id = contactJob.Id;
                db.Entry(contactJob).State = EntityState.Added;
            }
            else
            {
                // Handle the case of a user having selected the wrong outlet of a reporter and is now trying to select the other outlet (Yes this will happen)
                if (contactJob == null)
                {
                    contactJob = contact.ContactMediaJobTitle.SingleOrDefault(j => j.Id == jobDto.Id);

                    if (contactJob == null)
                    {
                        var ex = new Exception("This reporter has changed outlet. Please remove and re-add this reporter to that Media request");
                        throw new ApiHttpException(HttpStatusCode.BadRequest, ex);
                    }
                }
                contact.FirstName = Change(contact.FirstName, dto.FirstName, contact, "First Name");
                contact.LastName = Change(contact.LastName, dto.LastName, contact, "Last Name");
            }

            if (!string.IsNullOrEmpty(dto.WorkPhone))
            {
                var workPhoneNumber = contact.ContactPhoneNumber.SingleOrDefault(t => t.PhoneTypeId == GetWorkPhoneGuid(db));

                if (workPhoneNumber == null)
                {
                    workPhoneNumber = new ContactPhoneNumber
                    {
                        Id = Guid.NewGuid(),
                        ContactId = contact.Id,
                        PhoneTypeId = GetWorkPhoneGuid(db),
                        CreationDate = now
                    };
                    contact.ContactPhoneNumber.Add(workPhoneNumber);
                }
                workPhoneNumber.PhoneNumber = Change(FormatPhoneNumber(workPhoneNumber.PhoneNumber), FormatPhoneNumber(dto.WorkPhone), contact, "Work Phone");
                workPhoneNumber.PhoneNumberExtension = Change(workPhoneNumber.PhoneNumberExtension, dto.WorkPhoneExtension, contact, "Work Phone Extension");
                workPhoneNumber.ModifiedDate = now;
            }
            if (!string.IsNullOrEmpty(dto.CellPhone))
            {
                var cellPhoneNumber = contact.ContactPhoneNumber.SingleOrDefault(t => t.PhoneTypeId == GetCellPhoneGuid(db));
                if (cellPhoneNumber == null)
                {
                    cellPhoneNumber = new ContactPhoneNumber
                    {
                        Id = Guid.NewGuid(),
                        ContactId = contact.Id,
                        PhoneTypeId = GetCellPhoneGuid(db),
                        CreationDate = now
                    };
                    contact.ContactPhoneNumber.Add(cellPhoneNumber);
                }
                cellPhoneNumber.PhoneNumber = Change(FormatPhoneNumber(cellPhoneNumber.PhoneNumber), FormatPhoneNumber(dto.CellPhone), contact, "Cell Phone");
                cellPhoneNumber.ModifiedDate = now;
            }

            if (!string.IsNullOrEmpty(dto.Email))
            {
                var webAddress = contact.ContactWebAddress.SingleOrDefault(t => t.WebAddressTypeId == GetEmailGuid(db));

                if (webAddress == null)
                {
                    webAddress = new ContactWebAddress
                    {
                        Id = Guid.NewGuid(),
                        ContactId = contact.Id,
                        WebAddressTypeId = GetEmailGuid(db),
                        CreationDate = now
                    };
                    contact.ContactWebAddress.Add(webAddress);
                }

                // if the email address has changed send an update to subscription management system.
                if (webAddress.WebAddress != null && webAddress.WebAddress != dto.Email)
                {
                    SubscriberInfo subscriberInfo = new SubscriberInfo()
                    {
                        EmailAddress = dto.Email
                    };

                    await subscribe.UpdateNewsOnDemandEmailSubscriptionWithPreferencesAsync(subscriberInfo, webAddress.WebAddress);
                }

                webAddress.WebAddress = Change(webAddress.WebAddress, dto.Email, contact, "Email");
                webAddress.ModifiedDate = now;
            }
            if (contactJob.CompanyId != jobDto.Outlet.Id)
            {
                if (contactJob.CompanyId == Guid.Empty)
                {
                    WriteActivityLogEntry(contact, "Attached to " + jobDto.Outlet.Name, 9); //  9=ActivityType.Outlet_Attached
                }
                else
                {
                    var oldCompanyName = db.Company.FirstOrDefault(c => c.Id == contactJob.CompanyId)?.CompanyName;
                    Change(contactJob.CompanyId, contact, "Outlet", oldCompanyName, jobDto.Outlet.Name);
                }
                contactJob.CompanyId = jobDto.Outlet.Id;
            }

            var newTitleId = GetAllMediaJobTitles(db).SingleOrDefault(e => e.Value == jobDto.Title).Key;
            if (contactJob.MediaJobTitleId != newTitleId)
            {
                var oldTitle = GetAllMediaJobTitles(db).SingleOrDefault(e => e.Key == contactJob.MediaJobTitleId).Value;
                Change(contactJob.MediaJobTitleId, contact, "Job", oldTitle, jobDto.Title);
                contactJob.MediaJobTitleId = newTitleId;
            }

            if (User.Identity.Name != null)
            {
                contact.RecordEditedBy = User.Identity.Name;
            }
            contact.ModifiedDate = now;
            // dto.email
        }

        void Change(Guid oldValue, Contact contact, string propertyName, string oldValueName, string newValueName)
        {
            bool added = (oldValue == Guid.Empty);
            propertyName += ": " + (added ? newValueName : oldValueName + " to " + newValueName);
            WriteActivityLogEntry(contact, (added ? "Added " : "Changed ") + propertyName);
        }

        string Change(string oldValue, string newValue, Contact contact, string propertyName)
        {
            bool added = string.IsNullOrEmpty(oldValue);
            if (oldValue == newValue || (string.IsNullOrEmpty(newValue) && added)) // null == string.Empty
                return oldValue;

            propertyName += ": " + (added ? newValue : oldValue + " to " + newValue);
            WriteActivityLogEntry(contact, (added ? "Added " : "Changed ") + propertyName);
            return newValue;
        }

        private void WriteActivityLogEntry(int actionType, int entityType, Guid entityId, string entityData, Guid eventId, string eventData, string eventUser)
        {
            SysLog log = new SysLog();
            log.Action = actionType.ToString();
            log.EntityType = entityType.ToString();
            log.EntityId = entityId;
            log.EntityData = entityData;
            log.EventId = eventId;
            log.EventData = eventData;
            log.EventUser = eventUser;
            log.EventDate = DateTime.Now;

            db.SysLog.Add(log);
        }

        public void WriteActivityLogEntry(Contact contact, string eventData, int activityType = 3) //ActivityType.Record_Edited
        {
            if (User.Identity.Name != null)
            {
                WriteActivityLogEntry(activityType, 2, contact.Id, contact.FirstName + " " + contact.LastName,
                        Guid.Empty, eventData, User.Identity.Name);
            }
        }

        public void WriteActivityLogEntry(Company outlet, string eventData, int activityType = 3) //ActivityType.Record_Edited
        {
            if (User.Identity.Name != null)
            {
                WriteActivityLogEntry(activityType, 0, outlet.Id, outlet.CompanyName,
                        Guid.Empty, eventData, User.Identity.Name);
            }
        }
    }
}