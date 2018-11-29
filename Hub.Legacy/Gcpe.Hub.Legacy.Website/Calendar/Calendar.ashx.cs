using DDay.iCal;
using DDay.iCal.Serialization;
using System;
using System.Linq;
using System.Text;
using System.Web;

namespace CorporateCalendar
{
    public class Calendar : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            //TODO: Review Security

            var customPrincipal = new Security.CustomPrincipal(context.User.Identity);

            bool isCurrentUserInOwnerList = customPrincipal.IsInApplicationOwnerOrganizations;

            DateTime? endDatetime = null;

            bool includeDeleted = false;

            var queryResults = ActivityDAO.GetAllActivitiesSecurely(customPrincipal.Id,
                isCurrentUserInOwnerList,
                endDatetime,
                new[] { "New","Changed","Reviewed"},
                new string[0],
                includeDeleted).Where(a => a.IsActive);


            iCalendar iCal = new iCalendar();

            iCal.ProductID = "-//Province of British Columbia//Corporate Calendar//EN";

            Uri requestUrl = HttpContext.Current.Request.Url;
            string env = requestUrl.Host.Replace("calendar.gcpe.gov.bc.ca", "").TrimEnd('.');
            string activityUrl = string.Format("{0}://{1}{2}", requestUrl.Scheme, requestUrl.Authority,
                    HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/Calendar/Activity.aspx?ActivityId=");

            iCal.AddProperty("X-PUBLISHED-TTL", "PT1M");
            iCal.AddProperty("X-WR-CALNAME", "Corporate Calendar" + (env == "" ? "" : " (" + env.ToUpper() + ")"));

            foreach (var activity in queryResults)
            {
                //if (activity.Title.ToUpper().Contains(" SLIDER: "))
                //    continue;

                if (activity.StartDateTime.HasValue && activity.EndDateTime.HasValue)
                {
                    bool isAllDay = activity.IsAllDay ||
                                    activity.StartDateTime.Value.TimeOfDay == new TimeSpan(8, 0, 0) && activity.EndDateTime.Value.TimeOfDay == new TimeSpan(18, 0, 0);

                    bool isMultiDay = activity.StartDateTime.Value.Date != activity.EndDateTime.Value.Date;

                    Event evt = iCal.Create<Event>();

                    if (isAllDay)
                    {
                        evt.Start = new iCalDateTime(activity.StartDateTime.Value.Date);
                        // Date truncates to the beginning of the day.
                        // So add a day so it runs to the end of the day.
                        evt.End = new iCalDateTime(activity.EndDateTime.Value.Date.AddDays(1));
                    }
                    else
                    {
                        evt.Start = new iCalDateTime(activity.StartDateTime.Value);
                        evt.End = new iCalDateTime(activity.EndDateTime.Value);
                    }

                    evt.Location = activity.City;

                    evt.Summary = activity.Title;

                    evt.IsAllDay = isAllDay;

                    if (!activity.IsConfirmed)
                        evt.Status = EventStatus.Tentative;
                    else
                        evt.Status = EventStatus.Confirmed;

                    if (isAllDay || isMultiDay)
                    {
                        evt.AddProperty("X-MICROSOFT-CDO-BUSYSTATUS", "FREE");
                    }

                    evt.Description = activityUrl + activity.Id;
                }
            }

            ISerializationContext ctx = new SerializationContext();
            ISerializerFactory factory = new DDay.iCal.Serialization.iCalendar.SerializerFactory();
            IStringSerializer serializer = factory.Build(iCal.GetType(), ctx) as IStringSerializer;

            string output = serializer.SerializeToString(iCal);
            var bytes = Encoding.UTF8.GetBytes(output);
            context.Response.ContentType = "text/calendar";
            context.Response.AddHeader("Content-disposition", "attachment; filename=CorporateCalendar.ics");
            context.Response.BinaryWrite(bytes);
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
