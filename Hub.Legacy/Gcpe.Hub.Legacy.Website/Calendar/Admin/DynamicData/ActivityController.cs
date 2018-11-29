using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CorporateCalendarAdmin.DynamicData
{
    public class ActivityController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public object Get(int id)
        {
            CorporateCalendar.Data.CorporateCalendarDataContext dc = new CorporateCalendar.Data.CorporateCalendarDataContext();

            //var userActivities = dc.Activities.Where(p => p.IsActive && p.CommunicationContactId == userID);
            //var selectedColomnsFromActivities = userActivities.Select(p => new { id = p.Id, commUserID = p.CommunicationContactId, title = p.Title, ministryID = p.ContactMinistryId, startDate = p.StartDateTime });

            // get list of activities belonging to user, but only pass back what is needed
            var selectedColomnsFromActivities = from acts in dc.ActiveActivities
                                                join comms in dc.ActiveCommunicationContacts
                                                  on acts.CommunicationContactId equals comms.Id
                                                where comms.SystemUserId == id
                                                select new { acts.Id, ActivityID = acts.Ministry + "-" + acts.Id, 
                                                             ministry = acts.Ministry, ministryID = acts.ContactMinistryId, 
                                                             commUserID = acts.CommunicationContactId, userID = comms.SystemUserId,
                                                             title = acts.Title, startDate = acts.StartDateTime
                                                };

            JavaScriptSerializer js = new JavaScriptSerializer();
            string strJSON = js.Serialize(selectedColomnsFromActivities);

            return selectedColomnsFromActivities; 

        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}