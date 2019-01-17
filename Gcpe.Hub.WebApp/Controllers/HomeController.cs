using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Gcpe.Hub.Data.Entity;
using Gcpe.Hub.Services.Legacy;
using Gcpe.Hub.Services.Legacy.Models;

namespace Gcpe.Hub.WebApp.Controllers
{
    public class HomeController : BaseController
    {
        private ISubscribe subscribe;
        public HomeController(HubDbContext db, ISubscribeClient subscribeClient) : base(db)
        {
            subscribe = subscribeClient.Subscribe;
        }

        public IActionResult Index()
        {
            ViewBag.UserMe = GetUserFromContext();
            return View();
        }

        public IActionResult Error()
        {
            ViewBag.UserMe = GetUserFromContext();
            return View();
        }

        public ActionResult SiteStatus(bool? showErrors)
        {
            List<string> model = new List<string>();
            DateTime start = DateTime.Now;
            model.Add(SiteStatusString("Subscribe API call: ", showErrors, ref start, () =>
            {
                IList<KeyValuePairStringString> tags = subscribe.SubscriptionItems("tags");
                return tags.Count() > 0 ? "OK" : "Failed";
            }));

            model.Add(SiteStatusString("Hub DB access, job titles: ", showErrors, ref start, () =>
            {
                return string.Join(", ", MediaContactsApiController.GetTitles(db).GetAwaiter().GetResult());
            }));

            return View("SiteStatus", model);
        }
        public static string SiteStatusString(string s, bool? showErrors, ref DateTime start, Func<string> func)
        {
            DateTime now = DateTime.Now;
            string value;
            try
            {
                value = func();
                s = "OK: " + s;
            }
            catch (Exception ex)
            {
                if (showErrors == false) throw ex;
                Exception inner = ex.InnerException;
                if (inner == null) value = ex.Message;
                else value = (inner.InnerException ?? inner).Message;
            }
            s += value + " (" + (int)(now - start).TotalMilliseconds + " ms)";
            start = now;
            return s;
        }
    }
}
