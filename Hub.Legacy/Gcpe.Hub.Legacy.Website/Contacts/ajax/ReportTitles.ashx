<%@ WebHandler Language="C#" Class="ReportTitles" %>

using System;
using System.Web;
using System.Web.SessionState;
using MediaRelationsLibrary;

public class ReportTitles : IHttpHandler, IRequiresSessionState {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";

        string reportTitle = context.Request.QueryString["reportTitle"];

        bool isPublic = false;
        if (context.Request.QueryString["reportType"] != null) {
            if (context.Request.QueryString["reportType"].ToLower().Equals("public")) isPublic = true;
        }

        ReportsLib lib = new ReportsLib();
        bool uniqueName = lib.GetIsUniqueReportName(reportTitle, isPublic);

        if (uniqueName) context.Response.Write("1");
        else context.Response.Write("0");
        
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}