<%@ WebHandler Language="C#" Class="LogEmailSearch" %>

using System;
using System.Web;
using System.Web.SessionState;
using MediaRelationsLibrary;

public class LogEmailSearch : IHttpHandler, IRequiresSessionState {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";

        string q = context.Request.QueryString["q"];
        if (q == null) q = "";        
        
        CommonEventLogging log = new CommonEventLogging();
        log.WriteActivityLogEntry(CommonEventLogging.ActivityType.Email, CommonEventLogging.EntityType.Search, 
            Guid.Empty, "", Guid.Empty, q, CommonMethods.GetLoggedInUser());        
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}