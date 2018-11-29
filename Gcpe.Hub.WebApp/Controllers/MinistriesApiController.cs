using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Gcpe.Hub.Data.Entity;
using Gcpe.Hub.WebApp.Models;
using Gcpe.Hub.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Gcpe.Hub.WebApp.Controllers
{
    using static Utility;

    [Route("api/ministries")]
    public class MinistriesApiController : BaseController
    {
        public MinistriesApiController(HubDbContext db, IConfiguration configuration) : base(db, configuration)
        {
        }

        [HttpGet]
        public async Task<IEnumerable<MinistryDto>> Get()
        {
            var ministries = await QueryMinistries()
                .OrderBy(m => m.SortOrder).ThenBy(m => m.DisplayName)
                .ToListAsync();

            return ministries.Select(e => ConvertToDto(e));
        }

        [HttpGet("eodsummary")]
        public async Task<IEnumerable<EodStatusDto>> GetEodSummary()
        {
            return await MinistriesApiController.GetEodSummaryData(db);
        }

        public static async Task<IEnumerable<EodStatusDto>> GetEodSummaryData(HubDbContext db)
        {
            var ministries = await (from m in db.Ministry
                                    where m.IsActive && (m.MinisterName.StartsWith("Honourable") || m.Abbreviation == "IGRS") && (m.ParentId == null || m.Abbreviation.Equals("EMBC"))
                                    orderby m.SortOrder, m.DisplayName
                                    select m)
                                    .Include(m => m.EodLastRunUser)
                                    .ToListAsync();

            var requests = await (from m in db.Ministry.Include(e => e.EodLastRunUser)
                                  join r in db.MediaRequest on m.Id equals r.LeadMinistryId
                                  //where (!r.RespondedAt.HasValue || (r.RespondedAt ?? r.RequestedAt) > m.EodLastRunDateTime)
                                  where (!r.RespondedAt.HasValue || r.RespondedAt.Value > m.EodLastRunDateTime)
                                  select r).ToListAsync();

            return ministries.Select(e => new EodStatusDto
            {
                Ministry = MinistriesApiController.ConvertToDto(e, false),
                LastActivity = requests.Where(r => r.LeadMinistryId == e.Id).Any() ? requests.Where(r => r.LeadMinistryId == e.Id).Max(r => r.RespondedAt ?? r.RequestedAt) : (DateTimeOffset?)null
                //,OpenCount = requests.Where(r => r.LeadMinistryId == e.Id && !r.RespondedAt.hasValue).Count()
            });
        }

        /*[HttpGet("me")]
        public async Task<IEnumerable<MinistryDto>> GetForMe()
        {
            var user = await UsersApiController.GetUserFromContext(db, User);

            var userDto = UsersApiController.ConvertToDto(user);

            var ministries = await QueryMinistries()
                                   .Where(m => userDto.IsAdvanced || m.SystemUserMinistry.Any(su => su.SystemUserId == user.Id && su.IsActive))
                                   .OrderBy(m => m.SortOrder).ThenBy(m => m.DisplayName)
                                   .ToListAsync();

            return ministries.Select(e => ConvertToDto(e));
        }*/

        private IQueryable<Ministry> QueryMinistries()
        {
            return db.Ministry
                     .Where(m => m.IsActive && (m.MinisterName.StartsWith("Honourable") || m.Abbreviation == "IGRS") && (m.ParentId == null || m.Abbreviation.Equals("EMBC")))
                     .Include(m => m.SystemUserMinistry).ThenInclude(m => m.SystemUser).ThenInclude(su => su.CommunicationContact) //Required for UsersController.ConvertToDto
                     .Include(m => m.EodLastRunUser)
                     .Include(m => m.ContactUser)
                     .Include(m => m.SecondContactUser);
        }

        internal static MinistryDto ConvertToDto(Ministry ministry, bool includeUsers = true)
        {
            string afterHoursPhoneWithoutExtension;
            var dto = new MinistryDto
            {
                Id = ministry.Id,
                DisplayAs = ministry.DisplayName,
                Abbreviation = ministry.Abbreviation,
                EodFinalizedDateTime = ministry.EodFinalizedDateTime,
                EodLastRunDateTime = ministry.EodLastRunDateTime,
                EodLastRunUser = UsersApiController.ConvertToDto(ministry.EodLastRunUser),
                PrimaryContact = UsersApiController.ConvertToDto(ministry.ContactUser),
                SecondaryContact = UsersApiController.ConvertToDto(ministry.SecondContactUser),
                AfterHoursPhoneExtension = GetPhoneExtension(ministry.WeekendContactNumber, out afterHoursPhoneWithoutExtension),
                AfterHoursPhone = FormatPhoneNumber(afterHoursPhoneWithoutExtension)
            };

            if (includeUsers)
            {
                //TODO: Investigate performance - is this requiring a round trip for every ministry when client calls ministries/all?

                dto.Users = ministry.SystemUserMinistry
                                .Where(um => um.IsActive)
                                .Select(um => um.SystemUser)
                                .Distinct()
                                .Where(u => u.IsActive)
                                .Where(u => u.CommunicationContact.Any(c => c.SortOrder != 0))
                                .OrderBy(u => u.FullName)
                                .Select(u => UsersApiController.ConvertToDto(u));
            }

            return dto;
        }

        [HttpPut("{id}")]
        public async Task Put(Guid id, [FromBody]MinistryDto dto)
        {
            if (id != dto.Id)
            {
                throw new ApiHttpException(HttpStatusCode.BadRequest);
            }

            var ministry = await ConvertFromDtoAsync(dto);
            db.Ministry.Update(ministry);

            await db.SaveChangesAsync();
        }

        private async Task<Ministry> ConvertFromDtoAsync(MinistryDto dto)
        {
            Ministry ministry = await db.Ministry.FindAsync(dto.Id);

            // Only permit updating select properties.
            SystemUser priUser = null; 
            if (dto.PrimaryContact != null)
            {
                priUser = await db.SystemUser.SingleOrDefaultAsync(e => e.RowGuid == dto.PrimaryContact.Id);
            }
            ministry.ContactUserId = (priUser != null ? priUser.Id : (int?)null);

            SystemUser secUser = null;
            if (dto.SecondaryContact != null)
            {
                secUser = await db.SystemUser.SingleOrDefaultAsync(e => e.RowGuid == dto.SecondaryContact.Id);
            }
            ministry.SecondContactUserId = (secUser != null ? secUser.Id : (int?)null);

            //ministry.ContactUserId = dto.PrimaryContact == null ? (int?)null : (await db.SystemUser.SingleAsync(e => e.RowGuid == dto.PrimaryContact.Id)).Id;
            //ministry.SecondContactUserId = dto.SecondaryContact == null ? (int?)null : (await db.SystemUser.SingleAsync(e => e.RowGuid == dto.SecondaryContact.Id)).Id;

            ministry.WeekendContactNumber = string.IsNullOrEmpty(dto.AfterHoursPhone) ? "" : dto.AfterHoursPhone + (string.IsNullOrEmpty(dto.AfterHoursPhoneExtension) ? "" : " x " + dto.AfterHoursPhoneExtension);

            ministry.Timestamp = DateTime.Now;

            return ministry;
        }
    }
}