using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gcpe.Hub.Data.Entity;
using Gcpe.Hub.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gcpe.Hub.WebApp.Controllers
{
    using static Utility;

    [Route("api/users")]
    public class UsersApiController : BaseController
    {
        /// <summary>
        /// The Role Id MUST match the id of the Role in the Role Table in the database
        /// </summary>
        public enum SecurityRole { ReadOnly = 1, Editor, Advanced, Administrator, SysAdmin };

        public UsersApiController(HubDbContext db) : base(db)
        {
        }

        [HttpGet("me")]
        public UserDto GetForMe()
        {
            return UserMe;
        }

        [HttpGet]
        public async Task<IEnumerable<UserDto>> Get(Guid? ministryId = null)
        {
            var users = db.SystemUser
                          //.Include(e => e.CommunicationContact)
                          .Where(e => e.IsActive)
                          .Where(e => e.CommunicationContact.Any(f => f.SortOrder != 0))
                          .Where(e => e.RoleId != (int)SecurityRole.ReadOnly && e.RoleId != (int)SecurityRole.SysAdmin)
                          .OrderBy(e => e.FullName)
                          .AsQueryable();

            if (ministryId.HasValue)
            {
                users = users//.Include(e => e.SystemUserMinistrySystemUser)
                             .Where(e => e.SystemUserMinistrySystemUser.Any(f => f.MinistryId == ministryId.Value));

                //TODO: Consider excluding HQ users from list associated with this ministry (because they are associated with all ministries)
            }

            return (await users.ToListAsync()).Select(e => ConvertToDto(e));
        }

        
        //public UserDto GetUserById(Guid? userId = null)
        //{
           
        //    var user = db.SystemUser.Single(e => e.RowGuid == userId);

        //    return ConvertToDto(user);
        //}

        internal static UserDto ConvertToDto(SystemUser user)
        {
            if (user == null)
            {
                return null;
            }
            else
            {
                string workPhoneWithoutExtension;
                var dto = new UserDto();

                dto.Id = user.RowGuid;
                dto.DisplayAs = user.FullName;
                dto.IsEditor = user.RoleId >= (int)SecurityRole.Editor;
                dto.IsAdvanced = user.RoleId >= (int)SecurityRole.Advanced;

                dto.WorkTelephoneExtension = GetPhoneExtension(user.PhoneNumber, out workPhoneWithoutExtension);
                dto.WorkTelephone = FormatPhoneNumber(workPhoneWithoutExtension);
                dto.MobileTelephone = FormatPhoneNumber(user.MobileNumber);
                dto.EmailAddress = user.EmailAddress;

                dto.UserDomainName = "IDIR\\" + user.Username.ToUpper();

                return dto;
            }
        }
    }
}