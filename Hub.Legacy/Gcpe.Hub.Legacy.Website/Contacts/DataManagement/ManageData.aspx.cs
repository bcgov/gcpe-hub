using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using System.Xml;
using System.Text;

public partial class DataManagement_ManageData : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminNavigationLib anl = new AdminNavigationLib();

        XmlNode node = anl.GetCurrentNode();

        XmlNodeList list = node.SelectNodes("item");

        StringBuilder sb = new StringBuilder();

        foreach (XmlNode nodeItem in list) {
            sb.Append("<div>\n");
            sb.Append("<a href='" + ResolveUrl("~/") + nodeItem.Attributes["url"].Value.TrimStart('/') + "'>" + nodeItem.Attributes["name"].Value + "</a>\n");
            sb.Append("</div>\n");
        }

        itemsLit.Text = sb.ToString();
    }
}