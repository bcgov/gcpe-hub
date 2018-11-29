using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using CorporateCalendar.Data;
using CorporateCalendar.Security;


public class MinistryHandler : IHttpHandler
{

    CustomPrincipal _customPrincipal = null;
    public CustomPrincipal CustomPrincipal
    {
        get
        {
            if (_customPrincipal == null)
            {
                _customPrincipal = new CustomPrincipal(HttpContext.Current.User.Identity);
            }
            return _customPrincipal;
        }
    }


    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

        // Ministries

        using (var dc = new CorporateCalendarDataContext())
        {
            var ddm = new DropDownListManager(dc);
            // Communication contacts
            if (context.Request.QueryString["Op"] == "GetMinistryCommunicationContacts")
            {
                string ministryCode = context.Request.QueryString["ministryCode"];
                string sort = context.Request.QueryString["sortOrder"]; //RoleThenName
                DropDownListManager.CommunicationSortOrder sortOrder;

                if (sort == "FirstName")
                    sortOrder = DropDownListManager.CommunicationSortOrder.FirstName;
                else
                    sortOrder = DropDownListManager.CommunicationSortOrder.RoleThenName;

                IQueryable<ActiveDistinctCommunicationContact> activeMinistryCommunicationContacts;

                if (ministryCode != "Select Ministry")
                {
                    activeMinistryCommunicationContacts =
                        ddm.GetCommunicationContactByMinistryShortName(ministryCode, sortOrder);
                }
                else
                {
                    activeMinistryCommunicationContacts =
                        ddm.GetCommunicationContactsByCurrentUser(sortOrder, CustomPrincipal);
                }

                context.Response.Clear();
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.Write("[" + serializer.Serialize(activeMinistryCommunicationContacts.ToList()) + "]");
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