using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using MediaRelationsDatabase;

namespace MediaRelationsLibrary
{
    /// <summary>
    /// This class is used to contain methods for the site navigation with the navigation.xml file
    /// </summary>
    public class AdminNavigationLib
    {
        readonly string currentPath;
        readonly string xmlDocument;
        readonly string[] urlPaths;

        public AdminNavigationLib()
        {
            Uri url = HttpContext.Current.Request.Url;
            currentPath = url.AbsolutePath.ToLower();

            // get the individual parts of the url
            urlPaths = currentPath.Substring(1).Split('/');

            if (url.Query.StartsWith("?guid", StringComparison.Ordinal))
                currentPath += "?guid";

            if (currentPath.StartsWith("/legacy"))
                currentPath = currentPath.Substring("/legacy".Length);

            currentPath = currentPath.Replace(".aspx", "");

            string xmlDocumentPath = HttpContext.Current.Server.MapPath("~/Contacts/navigation.xml");

            xmlDocument = System.IO.File.ReadAllText(xmlDocumentPath);
            xmlDocument = xmlDocument.Replace("~/", (HttpContext.Current.Handler as System.Web.UI.Page).ResolveUrl("~/"));
        }

        /// <summary>
        /// This method is used to get the list of items to display on the dashboard of the 
        /// site
        /// </summary>
        /// <param name="logger">instance of a logging class to use</param>
        /// <returns>list of string arrays where 0 - name, 1 - url, 3 = css class</returns>
        public List<string[]> GetDashboardItems(CommonEventLogging logger)
        {
            var items = new List<string[]>();

            var list = new List<KeyValuePair<string, string>>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(new System.IO.StringReader(xmlDocument));

                XmlNodeList nodes = doc.SelectNodes("//navigation/item");

                foreach (XmlNode node in nodes)
                {
                    string name = "";
                    string url = "";
                    string cssClass = "";

                    bool displayOnDashboard = displayOnDashboard = Convert.ToBoolean(node.Attributes["DisplayOnDashboard"]?.Value);

                    if (displayOnDashboard)
                    {
                        //check user permissions
                        bool userHasPermission = UserHasPermission(node, logger);

                        if (userHasPermission)
                        {
                            if (node.Attributes["name"] != null) name = node.Attributes["name"].Value;
                            if (node.Attributes["url"] != null) url = node.Attributes["url"].Value;
                            if (node.Attributes["DashboardCssClass"] != null) cssClass = node.Attributes["DashboardCssClass"].Value;

                            string[] item = { name, url, cssClass };
                            items.Add(item);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.WriteException(exc, "AdminNavigationLib GetDashboardItems");
            }

            return items;
        }

        /// <summary>
        /// checks a user's permissions against the site section and access level specified in the xml node attributes
        /// </summary>
        /// <param name="node">xml node from the navigation file</param>
        /// <param name="logger">instance of a logger to use</param>
        /// <returns>true if the user has the required access, false otherwise</returns>
        private bool UserHasPermission(XmlNode node, CommonEventLogging logger)
        {
            bool userHasPermission = true;

            if (node.Attributes["SiteSection"] != null)
            {
                Permissions.SiteSection section;
                if (!Enum.TryParse(node.Attributes["SiteSection"].Value, out section))
                    section = Permissions.SiteSection.None;

                Permissions.SiteAction accessLevel;
                if (node.Attributes["AccessLevel"] == null || !Enum.TryParse(node.Attributes["AccessLevel"].Value, out accessLevel))
                    accessLevel = Permissions.SiteAction.None;

                if (section == Permissions.SiteSection.None) userHasPermission = true;
                else if (accessLevel == Permissions.SiteAction.None) userHasPermission = true;
                else
                {
                    Permissions.SiteAction userPermission = Permissions.GetUserPermissions(section);
                    userHasPermission = ((userPermission & accessLevel) != 0);
                }
            }
            return userHasPermission;
        }

        /// <summary>
        /// This method is used to get the list of top level navigation items on the site that
        /// is used to build the page header with
        /// </summary>
        /// <param name="selectedIndex">defaulted to -1 if not found, otherwise will be set as the
        /// index where the current page was found. To be used to help the calling function know
        /// which page is active</param>
        /// <param name="logger">an instance of an event logger</param>
        /// <returns>List of pairs (url, name) of pages that are top level in the site</returns>
        public List<KeyValuePair<string, string>> GetNavigationItems(string rootPath, out int selectedIndex, CommonEventLogging logger)
        {
            selectedIndex = -1;
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(new System.IO.StringReader(xmlDocument));

                XmlNode node = GetCurrentNode();
                if (node != null)
                {
                    XmlNode parentNode = node;
                    while (parentNode.ParentNode.Name == "item") parentNode = parentNode.ParentNode;
                    string parentUrl = parentNode.Attributes["url"].Value.ToLower();

                    XmlNodeList nodeList = doc.SelectNodes("//navigation/item");
                    if (nodeList.Count > 0)
                    {
                        int itemCount = 0;
                        foreach (XmlNode item in nodeList)
                        {
                            bool userHasPermission = UserHasPermission(item, logger);
                            bool isNavigationItem = true;
                            if (item.Attributes["Navigation"] != null)
                            {
                                if (!bool.TryParse(item.Attributes["Navigation"].Value, out isNavigationItem))
                                    isNavigationItem = true;
                            }

                            if (userHasPermission && isNavigationItem)
                            {
                                var url = (HttpContext.Current.Handler as System.Web.UI.Page).ResolveUrl("~/") + item.Attributes["url"].Value.TrimStart('/');
                                var pair = new KeyValuePair<string, string>(url, item.Attributes["name"].Value);
                                list.Add(pair);
                                // determine if the current item is part of this item
                                if (url.ToLower().Equals(parentUrl))
                                {
                                    selectedIndex = itemCount;
                                }

                                itemCount++;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.WriteException(exc, "AdminNavigationLib GetNavigationItems");
            }
            return list;
        }

        /// <summary>
        /// This method is called to get the breadcrumb of the current page. This finds where you are on the site in the xml
        /// and goes up by parent node to get to the first item for a full bread crumb.
        /// </summary>
        /// <returns>A string containing the breadcrumb (with anchors)</returns>
        public string GetBreadcrumb(bool includeHtml = true)
        {
            XmlNode node = GetCurrentNode();
            if (node != null)
            {
                string breadCrumb = "";

                breadCrumb += (includeHtml ? "<b>" : "")
                    + node.Attributes["name"].Value + (includeHtml ? "</b>" : "");

                XmlNode parentNode = null;
                parentNode = node.ParentNode;

                bool showBreadcrumb = true;
                if (node.Attributes["breadcrumb"] != null)
                {
                    try
                    {
                        showBreadcrumb = Convert.ToBoolean(node.Attributes[ "breadcrumb"].Value);
                    }
                    catch
                    {
                        //EventLogging log = new EventLogging();
                        //log.WriteException(exc, "AdminNavigationLib GetBreadCrumb");
                    }
                }

                if (showBreadcrumb)
                {
                    while (parentNode != null && parentNode.Name.Equals("item"))
                    {
                        breadCrumb = (includeHtml ? "<a href='" + (HttpContext.Current.Handler as System.Web.UI.Page).ResolveUrl("~/") + parentNode.Attributes["url"].Value.TrimStart('/') + "'>" : "") + parentNode.Attributes["name"].Value + (includeHtml ? "</a>" : "")
                            + " - " + breadCrumb;
                        parentNode = parentNode.ParentNode;
                    }

                    return breadCrumb;
                }
            }

            return null;
        }

        /// <summary>
        /// This method gets the current node in the XML of the page that the user is on. Will return null if not found.
        /// </summary>
        /// <returns>xml node of the xml where the user is at</returns>
        public XmlNode GetCurrentNode()
        {
            string searchPattern = "//item[translate(@url, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='" + currentPath + "']";

            XmlDocument doc = new XmlDocument();
            doc.Load(new System.IO.StringReader(xmlDocument));

            XmlNode node = doc.SelectSingleNode(searchPattern);

            return node;
        }


        #region mobile nav

        public List<MobileNavItem> MobileNavigationItems(out int selectedIndex, CommonEventLogging logger)
        {
            List<MobileNavItem> navigationItems = new List<MobileNavItem>();

            selectedIndex = -1;

            XmlDocument doc = new XmlDocument();
            doc.Load(new System.IO.StringReader(xmlDocument));

            XmlNode node = GetCurrentNode();

            XmlNode parentNode = node;
            while (parentNode.ParentNode.Name == "item") parentNode = parentNode.ParentNode;
            string parentUrl = parentNode.Attributes["url"].Value.ToLower();
            XmlNodeList nodeList = doc.SelectNodes("//navigation/item[@MobileNav=\"true\"]");

            int count = 0;
            foreach (XmlNode item in nodeList)
            {
                string name = item.Attributes["name"].Value;
                string sectionName = (item.Attributes["SiteSection"] != null ? item.Attributes["SiteSection"].Value : null);
                string url = item.Attributes["url"].Value;

                bool userHasPermission = UserHasPermission(item, logger);

                if (userHasPermission)
                {
                    Nullable<Permissions.SiteSection> section = null;
                    if (!string.IsNullOrEmpty(sectionName))
                    {
                        section = (Permissions.SiteSection)Enum.Parse(typeof(Permissions.SiteSection), sectionName);
                    }
                    var navItem = new MobileNavItem(section, name, url);
                    navigationItems.Add(navItem);

                    // get the selected item
                    if (node != null)
                    {
                        if (node.Attributes["url"].Value.ToLower().Equals(url.ToLower())) selectedIndex = count;
                    }
                    count++;
                }
            }

            return navigationItems;
        }

        #endregion

    }

    public struct MobileNavItem
    {
        public Nullable<Permissions.SiteSection> Section;
        public string Name;
        public string Url;

        public MobileNavItem(Nullable<Permissions.SiteSection> section, string name, string url)
        {
            Section = section;
            Name = name;
            Url = url;
        }
    }
}
