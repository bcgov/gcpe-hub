extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gcpe.Hub.News
{
    using legacy::Gcpe.Hub.Data.Entity;

    public class Page : System.Web.UI.Page
    {
        public Page()
        {
            utcPage = DateTimeOffset.UtcNow;
        }

        private HubEntities dbContext;
        protected HubEntities DbContext
        {
            get
            {
                if (dbContext == null)
                {
                    lock (this)
                    {
                        if (dbContext == null)
                            dbContext = new HubEntities();
                    }
                }

                return dbContext;
            }
        }

        private DateTimeOffset utcPage;
        protected DateTimeOffset UtcPage
        {
            get
            {
                return utcPage;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void Redirect(string url, bool endResponse = false)
        {
            if (endResponse || !ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
            {
                Response.Redirect(url, endResponse);

                if (!endResponse)
                    Context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                //if (endResponse)
                //    throw new InvalidOperationException();

                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "window.location.replace('" + ResolveClientUrl(url) + "');", true);
            }
        }

        public static string LongDateFormat
        {
            get { return "MMMM d, yyyy h:mm tt"; }
        }

        public static string ShortDateEnterFormat
        {
            get { return "yyyy-MM-d"; }
        }

        public static string FormatDateTime(DateTimeOffset date)
        {
            string text;

            System.Globalization.Calendar invariantCalendar = System.Globalization.CultureInfo.InvariantCulture.Calendar;

            if (date.Date.Date == DateTime.Today.AddDays(-1d))
                text = "Yesterday";
            else if (date.Date.Date == DateTime.Today)
                text = "Today";
            else if (date.Date.Date == DateTime.Today.AddDays(1d))
                text = "Tomorrow";
            //else if (date.Date.Date > DateTime.Today && date.Date.Date < DateTime.Today.AddDays(7d))
            //    text = date.ToString("dddd") + ", " + date.ToString("MMMM d, yyyy");
            else if (date.Date.Date > DateTime.Today && invariantCalendar.GetWeekOfYear(date.Date, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday) == invariantCalendar.GetWeekOfYear(DateTime.Today, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday))
                text = date.ToString("dddd") + ", " + date.ToString("MMMM d, yyyy");
            //else if (date.Date.Date > DateTime.Today && invariantCalendar.GetWeekOfYear(date.Date, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday) == 1 + invariantCalendar.GetWeekOfYear(DateTime.Today, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday))
            //    text = "Next " + date.ToString("dddd") + ", " + date.ToString("MMMM d, yyyy");
            else
                text = date.ToString("MMMM d, yyyy");

            if (date.TimeOfDay != TimeSpan.Zero)
            {
                text += " at " + date.ToString("h:mm tt").ToLower();
            }

            return text;
        }
    }
}