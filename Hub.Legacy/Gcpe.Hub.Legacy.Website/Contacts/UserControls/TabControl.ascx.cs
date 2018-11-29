using System;
using System.Collections.Generic;
using System.Text;
using MediaRelationsLibrary;

public partial class UserControls_TabControl : System.Web.UI.UserControl
{
    public Dictionary<string, string> Tabs { get; set; }

    public string CssClass { get; set; }

    public string SelectedTab { get; set; }

    public string OnTabClickEvent { get; set; }

    private bool forPhone = false;
    public bool ForPhone
    {
        get { return forPhone; }
        set { forPhone = value; }
    }

    private bool isAddScreen = false;
    public bool IsAddScreen
    {
        get { return isAddScreen; }
        set { isAddScreen = value; }
    }

    public void Refresh()
    {
        SetHtml();
    }

    private void SetHtml()
    {
        if (CssClass != null)
        {
            CssClassDiv.Attributes["class"] = CssClass;
        }

        if (ForPhone)
        {
            tabsContainerDiv.Attributes.Add("class", "common-page-tabs");
        }

        StringBuilder tsb = new StringBuilder();
        StringBuilder jsb = new StringBuilder();

        jsb.Append("<script type='text/javascript'>\n");
        jsb.Append("var tabControlCurrentTab = \"" + SelectedTab + "\";\n");
        jsb.Append("var onClickMethod = \"" + OnTabClickEvent + "\";\n");
        jsb.Append("var tabControlTabs = new Array();\n");

        if (Tabs == null) return;

        int count = 0;
        foreach (string key in Tabs.Keys)
        {
            string additionalClass = "num" + Tabs.Count;
            if (count == 0) additionalClass += " first";
            if (count == Tabs.Count - 1) additionalClass += " last";


            string tabUrl = Tabs[key];
            if (!isAddScreen)
            {
                if (key.Equals(SelectedTab))
                {
                    tsb.Append("<div class='selected bluegradient " + additionalClass + "'>" + CommonMethods.AddSpacesCamelCase(key) + "</div>\n");
                }
                else
                {
                    tsb.Append("<a href='" + tabUrl + "' class='gradient " + (key.Equals(SelectedTab) ? "selected" : "") + " " + additionalClass +
                    "' onclick=\"" + (!string.IsNullOrWhiteSpace(OnTabClickEvent) ? "return " + OnTabClickEvent + "('" + key + "', '" + SelectedTab + "');" : "") + "\">" + CommonMethods.AddSpacesCamelCase(key) + "</a>\n");
                }

            }
            else
            {
                tsb.Append("<div class='" + (key.Equals(SelectedTab) ? "bluegradient selected" : "gradient") + " " + additionalClass + "'>" + CommonMethods.AddSpacesCamelCase(key) + "</div>\n");
            }


            jsb.Append("tabControlTabs[" + count + "] = [\"" + key + "\", \"" + tabUrl + "\"];\n");

            count++;

        }

        jsb.Append("</script>\n");
        tabsScriptLit.Text = jsb.ToString();

        tabLit.Text = tsb.ToString();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        SetHtml();
    }
}