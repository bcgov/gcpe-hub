using System;
using System.Collections.Generic;
using System.Text;
using MediaRelationsLibrary;

public partial class MasterPage_MediaRelations : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bool print;
        print = bool.TryParse(Request.QueryString["print"], out print);

        if (print)
        {
            printStyleArea.Text = "<style type='text/css'>\n" + CommonMethods.GetResponsivePrintStyles(Server.MapPath("~/Contacts/css/MediaRelationsResponsive.css"), Server.MapPath("~/Contacts/css/print.css")) + "</style>\n";
            printButtonPanel.Visible = true;
        }
        else
        {
            printButtonPanel.Visible = false;
        }
        if (!IsPostBack)
        {
            SetupPageNavigation();
            SetupPageBreadcrumb();
        }

        if (Request.QueryString["message"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["message"]))
        {
            string message = Request.QueryString["message"].Trim();
            message = message.Replace("\"", "\\\"");
            message = message.Replace("\n", "\\n");

            StringBuilder mb = new StringBuilder();
            mb.Append("$(document).ready(function() {\n");
            mb.Append(" alert(\"" + message + "\");\n");
            //if (print) mb.Append("self.print();\n");
            mb.Append("});\n");

            ScriptLiteral.Text = mb.ToString();
        }
        else
        {
            /*StringBuilder mb = new StringBuilder();
            mb.Append("$(document).ready(function() {\n");
            if (print) mb.Append("self.print();\n");
            mb.Append("});\n");

            ScriptLiteral.Text = mb.ToString();*/
        }

        DesktopLogo.Src = Gcpe.Hub.Configuration.App.Settings.ContactsHeaderImg;
        PhoneLogo.Src = Gcpe.Hub.Configuration.App.Settings.ContactsHeaderImg;
    }

    private void SetupPageNavigation()
    {
        CommonEventLogging logger = new CommonEventLogging();

        AdminNavigationLib anl = new AdminNavigationLib();
        int selectedIndex = -1;
        List<KeyValuePair<string, string>> navigationItems = anl.GetNavigationItems(ResolveUrl("~/Contacts/"), out selectedIndex, logger);

        int index = 0;
        foreach (KeyValuePair<string, string> pair in navigationItems)
        {
            string classString = " ";

            if (index == 0) classString = "first ";
            if (selectedIndex == index) classString += "selected ";
            classString = classString.Trim();

            navigationLiteral.Text += "<a href='" + pair.Key + "' class='" + classString + "'>" + pair.Value + "</a>";
            index++;
        }
    }

    private void SetupPageBreadcrumb()
    {
        AdminNavigationLib anl = new AdminNavigationLib();
        string breadCrumb = anl.GetBreadcrumb();

        string titleString = "Media Relations Database";

        if (!string.IsNullOrEmpty(breadCrumb))
        {
            breadcrumbLiteral.Text = anl.GetBreadcrumb();
            titleString = anl.GetBreadcrumb(false) + " > " + titleString;
        }

        titleTag.Text = titleString;
    }
}
