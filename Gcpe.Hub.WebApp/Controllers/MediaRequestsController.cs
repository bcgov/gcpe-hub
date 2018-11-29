using Gcpe.Hub.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gcpe.Hub.WebApp.Controllers
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class MediaRequestsController : BaseController
    {
        public MediaRequestsController(HubDbContext db, IConfiguration configuration) : base(db, configuration)
        {
        }

        public IActionResult App()
        {
            ViewBag.baseHref = "/MediaRequests";
            ViewBag.UserMe = GetUserFromContext();

            return View();
        }
    }
}