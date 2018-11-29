using System;
using System.Collections.Generic;
using System.Web;
using DDay.iCal;
using DDay.iCal.Serialization;

namespace Gcpe.Hub.News.ReleaseManagement
{
    /// <summary>
    /// Summary description for Forecast
    /// </summary>
    public class Forecast : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            iCalendar iCal = new iCalendar();

            iCal.ProductID = "-//Province of British Columbia//GCPE Hub//EN";

            Uri requestUrl = HttpContext.Current.Request.Url;
            string env = requestUrl.Host.Replace("hub.gcpe.gov.bc.ca", "").TrimEnd('.');
            string appRoot = string.Format("{0}://{1}{2}", requestUrl.Scheme, requestUrl.Authority,
                HttpContext.Current.Request.ApplicationPath.TrimEnd('/'));

            iCal.AddProperty("X-PUBLISHED-TTL", "PT1M");
            iCal.AddProperty("X-WR-CALNAME", "Release Forecast" + (env == "" ? "" : " (" + env.ToUpper() + ")"));

            ReleasesModel model = new ReleasesModel();
            List<ReleasesModel.Result> futureActivities = model.GetFutureActivitiesAndNR();

            foreach (ReleasesModel.Result activity in futureActivities)
            {
                if (activity.PublishDateTime.HasValue)
                {
                    // Create the event, and add it to the iCalendar
                    Event evt = iCal.Create<Event>();

                    // Set information about the event
                    evt.Start = new iCalDateTime(((DateTimeOffset)activity.PublishDateTime).DateTime);

                    if (!activity.IsCalActivity || !activity.EventEndDateTime.HasValue)
                    {
                        //Display as 5 minutes to match the convention follwed by Corporate Calendar users for NR-only activities.
                        evt.End = evt.Start.AddMinutes(5);
                    }
                    else
                    {
                        evt.End = new iCalDateTime(((DateTimeOffset)activity.EventEndDateTime).DateTime);
                    }

                    if (activity.IsCalActivity)
                    {
                        evt.Description = appRoot + "/Calendar/Activity.aspx?ActivityId=" + (activity.ActivityId ?? 0);
                    }
                    else
                    {
                        string status = activity.IsPublished ? "Published" : (activity.IsCommitted ? "Scheduled" : "Drafts");
                        string reference = activity.Reference == string.Empty ? HubModel.EncodeGuid((Guid)activity.Id) : activity.Reference;

                        evt.Description = appRoot + "/News/ReleaseManagement/" + status + "/" + reference;
                    }

                    evt.Location = activity.Location;

                    evt.Summary = activity.Headline;

                    evt.IsAllDay = activity.IsAllDay ?? false;

                    if (activity.IsAllDay != true && evt.Start.Hour == 8 && evt.Start.Minute == 0 && evt.End.Hour == 18 && evt.End.Minute == 0)
                        evt.IsAllDay = true;

                    if (activity.IsCalActivity && !(activity.IsConfirmed ?? false))
                        evt.Status = EventStatus.Tentative;
                    else
                        evt.Status = EventStatus.Confirmed;
                }
            }

            ISerializationContext ctx = new SerializationContext();
            ISerializerFactory factory = new DDay.iCal.Serialization.iCalendar.SerializerFactory();
            IStringSerializer serializer = factory.Build(iCal.GetType(), ctx) as IStringSerializer;

            string output = serializer.SerializeToString(iCal);
            var bytes = System.Text.Encoding.UTF8.GetBytes(output);
            context.Response.ContentType = "text/calendar";
            context.Response.AddHeader("Content-disposition", "attachment; filename=Forecast.ics");
            context.Response.BinaryWrite(bytes);
            return;
        }
    }
}