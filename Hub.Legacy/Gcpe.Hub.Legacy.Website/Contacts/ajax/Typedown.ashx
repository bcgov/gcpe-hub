<%@ WebHandler Language="C#" Class="Typedown" %>

using System;
using System.Web;
using MediaRelationsLibrary;

public class Typedown : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        SearchLib lib = new SearchLib();
        context.Response.ContentType = "text/plain";
        context.Response.Write(lib.GetTypedownResults(context.Request.QueryString["q"]));
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}