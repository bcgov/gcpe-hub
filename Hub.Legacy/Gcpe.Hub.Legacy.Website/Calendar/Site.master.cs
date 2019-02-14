using System;
using System.Configuration;
using System.Web;
using CorporateCalendar.Security;
using Gcpe.Hub.Properties;

public partial class SiteMaster : System.Web.UI.MasterPage
{
    public string GetVersion()
    {
        return ConfigurationManager.AppSettings["Version"];
    }
    //Security is determined based upon user Account Name. Here the user Identity gives the Users Account Name.
    CustomPrincipal _customPrincipal = null;
    public CorporateCalendar.Security.CustomPrincipal CustomPrincipal
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

    public string GetCalendarUrl()
    {
        return string.Format("{0}://{1}{2}", Request.Url.Scheme.Replace("http", "webcal"), Request.Url.Authority,
            Request.ApplicationPath.TrimEnd('/') + "/Calendar/Calendar.ashx");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Handle read-only users
        if (CustomPrincipal.IsInRole(CorporateCalendar.Security.SecurityRole.ReadOnly))
        {
            newActivity.Visible =  false;
        }

        // Handle users capable of administering data
        // RolesIds are:
        // 1 = ReadOnly
        // 2 = Editor
        // 3 = Advanced
        // 4 = Administrator
        // 5 = SysAdmin
        // For a description of each role, run: ~/Calendar/Admin/Roles/ListDetails.aspx
        if (CustomPrincipal.RoleId <= 3)
        {
            manage.Visible =  false;
        }
        else
        {
            manage.HRef = ResolveClientUrl("~/Calendar/Admin/");
        }

        sync.HRef = GetCalendarUrl();

        help.HRef = Settings.Default.HelpFileUri.ToString();

        favicon.Href = Settings.Default.FaviconImg;
    }
}