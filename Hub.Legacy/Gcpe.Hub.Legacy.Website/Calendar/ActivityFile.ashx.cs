using CorporateCalendar.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gcpe.Hub.Calendar
{
    public class ActivityFile : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var id = int.Parse(context.Request.QueryString["id"]);

            using (var dc = new CorporateCalendarDataContext())
            {
                var activityFile = dc.ActivityFiles.SingleOrDefault(f => f.Id == id);
                if (activityFile != null)
                {
                    var data = activityFile.Data.ToArray();
                    context.Response.BinaryWrite(data);
                    context.Response.ContentType = activityFile.FileType;
                    context.Response.AppendHeader("Content-Disposition",  string.Format("attachment; filename={0}", activityFile.FileName));
                }
            }
       }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}