<%@ WebHandler Language="C#" Class="LogPrintContact" %>

using System;
using System.Web;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Linq;


public class LogPrintContact : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        Guid guid;
        if (Guid.TryParse(context.Request.QueryString["guid"], out guid))
        {
            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                Contact contact = (from c in ctx.Contacts where c.ContactId == guid select c).FirstOrDefault();

                if (contact != null)
                {
                    CommonEventLogging log = new CommonEventLogging();

                    log.WriteActivityLogEntry(CommonEventLogging.ActivityType.Print, CommonEventLogging.EntityType.Contact,
                    guid, contact.FirstName + " " + contact.LastName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
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