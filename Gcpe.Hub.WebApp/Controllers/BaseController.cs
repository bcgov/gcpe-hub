using System;
using System.Linq;
using Gcpe.Hub.Data.Entity;
using Gcpe.Hub.Website.Models;
using Microsoft.AspNetCore.Mvc;
using static Gcpe.Hub.WebApp.Controllers.UsersApiController;

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

        static string error = "User '{0}' does not have the proper permissions to access this site. Please contact your administrator.";
        protected UserDto GetUserFromContext()
        {
            string debugUsername = Environment.GetEnvironmentVariable("DebugUsername");
            string userName = string.IsNullOrEmpty(debugUsername) ? User.Identity.Name : debugUsername;

            SystemUser user = null;
            if (!string.IsNullOrEmpty(userName))
            {
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

                user = users.SingleOrDefault();
            }

            if (user == null || user.RoleId == (int)SecurityRole.ReadOnly)
            {
                //Prevent read-only user from making changes to media requests
                throw new UnauthorizedAccessException(string.Format(error, userName ?? ""));
            }

            return UsersApiController.ConvertToDto(user);
        }
    }
}
