<%@ WebHandler Language="C#" Class="LogEmailCompany" %>

using System;
using System.Web;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Linq;

public class LogEmailCompany : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        Guid guid;
        if (Guid.TryParse(context.Request.QueryString["guid"], out guid))
        {
            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                Company company = (from c in ctx.Companies where c.CompanyId == guid select c).FirstOrDefault();

                if (company != null)
                {
                    CommonEventLogging log = new CommonEventLogging();

                    log.WriteActivityLogEntry(CommonEventLogging.ActivityType.Email, CommonEventLogging.EntityType.Company,
                    guid, company.CompanyName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
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