using System;
using System.Linq;
using System.Net;
using Gcpe.Hub.Data.Entity;
using Gcpe.Hub.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Gcpe.Hub.WebApp.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly HubDbContext db;
        protected readonly IConfiguration Configuration;

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

        public BaseController(HubDbContext db, IConfiguration configuration)
        {
            this.db = db;
            this.Configuration = configuration;
        }

        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            ViewBag.FaviconImg = this.Configuration["FaviconImg"];
        }

        protected UserDto GetUserFromContext()
        {
            string activeDirectoryDomain = Configuration["ActiveDirectoryDomain"];
            string debugUsername = null;
            if (User.Identity.Name == null || !User.Identity.Name.StartsWith(activeDirectoryDomain, StringComparison.OrdinalIgnoreCase))
            {
                debugUsername = Environment.GetEnvironmentVariable("DebugUsername");
            }
            string userName = string.IsNullOrEmpty(debugUsername) ? User.Identity.Name : debugUsername;


            if (userName.StartsWith(activeDirectoryDomain, StringComparison.OrdinalIgnoreCase))
                userName = userName.Substring((activeDirectoryDomain + "\\").Length);

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
