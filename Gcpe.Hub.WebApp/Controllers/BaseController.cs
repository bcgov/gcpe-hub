using System;
using System.Linq;
using System.Net;
using Gcpe.Hub.Data.Entity;
using Gcpe.Hub.Website.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gcpe.Hub.WebApp.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly HubDbContext db;

        public UserDto UserMe
        {
            get
            {
                if (ViewBag.UserMe == null)
                {
                    ViewBag.UserMe = GetUserFromContext();
                }
                return ViewBag.UserMe;
            }
        }

        public BaseController(HubDbContext db)
        {
            this.db = db;
        }

        protected UserDto GetUserFromContext()
        {
            string debugUsername = Environment.GetEnvironmentVariable("DebugUsername");
            string userName = string.IsNullOrEmpty(debugUsername) ? User.Identity.Name : debugUsername;

            if (string.IsNullOrEmpty(userName))
                throw new ApiHttpException(HttpStatusCode.Forbidden);

            userName = userName.Substring(userName.LastIndexOf("\\") + 1);

            IQueryable<SystemUser> users;
            /*if (returnSystemMinistries)
            {
                users = db.SystemUser
                          .Include(e => e.SystemUserMinistrySystemUser).ThenInclude(e => e.Ministry)
                          .Include(e => e.CommunicationContact)//.ThenInclude(e => e.Ministry)
                          .Where(e => e.Username == userName && e.IsActive);
            }
            else*/
            {
                users = db.SystemUser
                  //.Include(e => e.CommunicationContact).ThenInclude(e => e.Ministry)
                  .Where(e => e.Username == userName && e.IsActive);
            }

            var user = users.SingleOrDefault();

            if (user == null)
                throw new ApiHttpException(HttpStatusCode.Forbidden);

            if (user.RoleId == 1)
            {
                //Prevent read-only user from making changes to media requests
                throw new ApiHttpException(HttpStatusCode.Forbidden);
            }

            return UsersApiController.ConvertToDto(user);
        }
    }
}
