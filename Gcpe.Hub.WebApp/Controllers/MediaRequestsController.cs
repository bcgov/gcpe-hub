using Gcpe.Hub.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gcpe.Hub.WebApp.Controllers
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class MediaRequestsController : BaseController
    {
        protected readonly IConfiguration Configuration;
        public MediaRequestsController(HubDbContext db, IConfiguration configuration) : base(db)
        {
            this.Configuration = configuration;
        }

        public IActionResult App()
        {
            ViewBag.baseHref = "/MediaRequests";
            ViewBag.UserMe = GetUserFromContext();
            ViewBag.FaviconImg = this.Configuration["FaviconImg"];

            return View();
        }
    }
}