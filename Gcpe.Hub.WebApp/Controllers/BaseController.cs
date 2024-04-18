using System;
using System.Linq;
using Gcpe.Hub.Data.Entity;
using Gcpe.Hub.Website.Models;
using Microsoft.AspNetCore.Mvc;
using static Gcpe.Hub.WebApp.Controllers.UsersApiController;
using Microsoft.Extensions.Configuration;

namespace Gcpe.Hub.WebApp.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly HubDbContext db;
        protected readonly IConfiguration Configuration;
        private string excludedMinistry;

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

        public BaseController(HubDbContext db, IConfiguration configuration)
        {
            this.db = db;
            this.Configuration = configuration;
            if (!string.IsNullOrEmpty(Configuration.GetValue<string>("ExcludedMinistry")))
            {
                excludedMinistry = Configuration.GetValue<string>("ExcludedMinistry");
            }
            else
            {
                excludedMinistry = "";
            }
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
            var temp = UsersApiController.ConvertToDto(user);

            if (!string.IsNullOrEmpty(excludedMinistry))
            {
                //get all open and closed request for those ministries.
                var listMinistries = db.SystemUserMinistry.Where(e => (e.SystemUserId == user.Id && e.IsActive == true)).Select(e => e.MinistryId).ToList();

                var excludedMinitry = db.Ministry.FirstOrDefault(e => e.Abbreviation == excludedMinistry);
                if (listMinistries.Contains(excludedMinitry.Id) && listMinistries.Count==1)
                {
                    temp.IsBCWSOnly = true;
                }
                else
                {
                    temp.IsBCWSOnly = false;
                }
            }
            else
            {
                temp.IsBCWSOnly = false;
            }
                
            


            return temp;
        }
    }
}
