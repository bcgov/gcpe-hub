using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.DirectoryServices.AccountManagement;
using MediaRelationsLibrary.Configuration;

namespace MediaRelationsLibrary
{
    public class Permissions
    {
        private static object lockObj = new Object();

        public enum SiteSection
        {
            None = 0,
            MediaRelationsCompany = 1, 
            MediaRelationsContact = 2, 
            MediaRelationsDataLists = 3,
            MediaRelationsCommonReports = 4,
            MediaRelationsUserReports = 5,
            MediaRelationsSearch = 6,
            MediaRelationsApprovals = 7
        };

        [FlagsAttribute]
        public enum SiteAction { None = 0, Read = 1, Create = 2, Update = 4, Delete = 8 };

        public static bool IsAdmin()
        {
            return IsAdmin(CommonMethods.GetLoggedInUser());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">AD Domain username of the user to check</param>
        /// <returns></returns>
        public static bool IsAdmin(string userId)
        {
            Dictionary<string, bool> userGroupPermissions = GetUsersGroupPermissions(userId);

            foreach (string group in App.Settings.AdminGroups.ToCollection())
            {
                if (!string.IsNullOrWhiteSpace(group))
                {
                    string groupName = group.Trim();
                    if (userGroupPermissions.ContainsKey(groupName) && userGroupPermissions[groupName] == true) return true;
                }
            }
            return false;
        }

        public static bool IsContributor() {
            return IsContributor(CommonMethods.GetLoggedInUser());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">AD Domain username of the user to check</param>
        /// <returns></returns>
        public static bool IsContributor(string userId)
        {
            if (IsAdmin(userId)) return false;

            Dictionary<string, bool> userGroupPermissions = GetUsersGroupPermissions(userId);

            foreach (string group in App.Settings.ContributorGroups.ToCollection())
            {
                if (!string.IsNullOrWhiteSpace(group))
                {
                    string groupName = group.Trim();
                    if (userGroupPermissions.ContainsKey(groupName) && userGroupPermissions[groupName] == true) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// contains the permissions for the news on demand and media advisory sites
        /// a dictionary with the site section as the key, and a key-value pair containing the active directory group name and permissions
        /// </summary>
        private static Dictionary<SiteSection, List<KeyValuePair<string, SiteAction>>> permissionMatrix = null;
        public static Dictionary<SiteSection, List<KeyValuePair<string, SiteAction>>> PermissionMatrix
        {
            get
            {
                lock (lockObj)
                {
                    if (permissionMatrix == null || permissionMatrix.Count == 0)
                    {
                        //create permission matrix
                        permissionMatrix = new Dictionary<SiteSection, List<KeyValuePair<string, SiteAction>>>();

                        //decode from config
                        foreach (string key in ConfigurationManager.AppSettings)
                        {
                            if (key.StartsWith("permissions_"))
                            {
                                string[] cPerm = ConfigurationManager.AppSettings[key].Split('/');
                                if (cPerm.Length == 3)
                                {
                                    SiteSection section = (SiteSection)Enum.Parse(typeof(SiteSection), cPerm[0].Trim());
                                    string adGroup = cPerm[1].Trim();
                                    string[] permission = cPerm[2].Split('|');
                                    SiteAction action = SiteAction.None;
                                    foreach (string perm in permission)
                                    {
                                        Permissions.SiteAction act;
                                        if (Enum.TryParse(perm.Trim(), out act))
                                            action = action | act;
                                    }

                                    KeyValuePair<string, SiteAction> kvp = new KeyValuePair<string, SiteAction>(adGroup, action);
                                    if (permissionMatrix.ContainsKey(section))
                                    {
                                        permissionMatrix[section].Add(kvp);
                                        //HttpContext.Current.Response.Write("added to list for "+section+"<br/>");
                                    }
                                    else
                                    {
                                        List<KeyValuePair<string, SiteAction>> plist = new List<KeyValuePair<string, SiteAction>>();
                                        plist.Add(kvp);
                                        permissionMatrix.Add(section, plist);
                                        //HttpContext.Current.Response.Write("added permission for " + section + "<br/>");
                                    }
                                }
                            }
                        }
                    }
                }
                return permissionMatrix;
            }
        }

        /// <summary>
        /// returns the permission level for the currently logged in user on the specified site section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static SiteAction GetUserPermissions(SiteSection section)
        {
            CommonEventLogging logger = new CommonEventLogging();
            return GetUserPermissions(CommonMethods.GetLoggedInUser(), section, logger);
        }

        /// <summary>
        /// returns the permission level for the currently logged in user on the specified site section
        /// </summary>
        /// <param name="section"></param>
        /// <param name="logger">instance of a logger to use</param>
        /// <returns></returns>
        public static SiteAction GetUserPermissions(SiteSection section, CommonEventLogging logger)
        {
            return GetUserPermissions(CommonMethods.GetLoggedInUser(), section, logger);
        }
        
        /// <summary>
        /// This gets a list of the sections in the site and tells whether the user has access to that 
        /// section or not. 
        /// 
        /// It first checks against the session to determine if the permissions have already been retreived
        /// during a users use of the site. 
        /// 
        /// It then gets the list of sections from the site, and checks the AD group for if the user is a 
        /// member of that group or not.
        /// </summary>
        /// <param name="userId">the user account of the user</param>
        /// <returns>A dictionary where the key is the ad group, and the value is if the user is in the ad group</returns>
        public static Dictionary<string, bool> GetUsersGroupPermissions(string userId)
        {
            // object representing the permissions of the user for a group.
            // key - the group name
            // value - boolean of if the user is a member of that group
            Dictionary<string, bool> adGroups = null;

            // get the item from session - will be null if no sessions set
            adGroups = HttpContext.Current.Session["BCNews_ClassLibrary.GetUsersGroupPermissions_" + userId] as Dictionary<string, bool>;

            // if the adGroups dictionary object is null or a count of 0, 
            // go to the active directory to set the session information of user permissions
            if (adGroups == null || adGroups.Count == 0)
            {
                adGroups = new Dictionary<string, bool>();

                bool debugMode = System.Diagnostics.Debugger.IsAttached ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] == "::1" : false;
                using (PrincipalContext ctx = debugMode ? null : new PrincipalContext(ContextType.Domain, MediaRelationsLibrary.Configuration.App.Settings.ActiveDirectoryDomain))
                {
                    UserPrincipal principal = debugMode ? null : UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, userId);
                    // get the list of siteSections from the permission matrix to
                    // set the list of sections that the user has access to or no access to
                    foreach (SiteSection siteSection in PermissionMatrix.Keys)
                    {
                        // go through each group in the section to add to the permissions
                        foreach (KeyValuePair<string, SiteAction> pair in PermissionMatrix[siteSection])
                        {
                            if (!adGroups.ContainsKey(pair.Key))
                            {
                                bool isAMember;
                                if (!debugMode)
                                {
                                    // get the AD group
                                    GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, IdentityType.Name, pair.Key);
                                    if (group == null) throw new Exception("Group does not exist: " + pair.Key);

                                    // search against the groups members for the users username
                                    isAMember = group.GetMembers(true).Any(m => m.SamAccountName == principal.SamAccountName);
                                }
                                else
                                {
                                    isAMember = pair.Key.EndsWith("_DVLPRS");
                                    //isAMember = pair.Key.EndsWith("_TSTRS");
                                }
                                // add the section to the users permission
                                adGroups.Add(pair.Key, isAMember);
                            }
                        }
                    }
                    HttpContext.Current.Session.Add("BCNews_ClassLibrary.GetUsersGroupPermissions_" + userId, adGroups);
                }
            }
            return adGroups;
        }

        /// <summary>
        /// returns the permission level for the specified user and section
        /// </summary>
        /// <param name="userId">AD Domain username of the user</param>
        /// <param name="section">site section to check permissions for</param>
        /// <param name="logger">instance of a logger to use</param>
        /// <returns>the permission level (create|read|update|delete)</returns>
        public static SiteAction GetUserPermissions(string userId, SiteSection section, CommonEventLogging logger)
        {
            //EventLogging log = new EventLogging();
            SiteAction returnval = SiteAction.None;

            //List<string> userGroups = null;
            if (userId != null)
            {
                try
                {
                    List<KeyValuePair<string, SiteAction>> perms = PermissionMatrix[section];
                    Dictionary<string, bool> adGroups = GetUsersGroupPermissions(userId);

                    foreach (KeyValuePair<string, SiteAction> kvp in perms)
                    {
                        bool isMemberOf;
                        if (adGroups.TryGetValue(kvp.Key, out isMemberOf) && isMemberOf && kvp.Value > returnval) {
                            returnval = kvp.Value;
                        }
                    }

                } catch (Exception ex) {
                    logger.WriteException(ex, "Permissions GetUserPermissions");
                }
            }

            return returnval;
        }

        /// <summary>
        /// gets the logged in
        /// </summary>
        /// <param name="logger">instance of a logger to use</param>
        /// <returns></returns>
        public static List<string> GetLoggedInUserGroups(CommonEventLogging logger)
        {
            string loggedInUser = CommonMethods.GetLoggedInUser();

            List<string> result = null;

            result = GetGroupsForUser(loggedInUser, logger);
            HttpContext.Current.Session["BCNews_UserGroups"] = result;

            return result;
        }

        /// <summary>
        /// returns the email address listed for the user in active directory
        /// </summary>
        /// <param name="loggedInUser">the active directory username</param>
        /// <param name="log">event logging</param>
        /// <returns>the email address, or null if not available</returns>
        public static string GetEmailForUser(string loggedInUser)
        {
            string result = null;
            try
            {
                // establish domain context
                PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain, MediaRelationsLibrary.Configuration.App.Settings.ActiveDirectoryDomain);

                // find your user
                UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, loggedInUser);

                result = user?.EmailAddress;

            }
            catch { }
            if (string.IsNullOrWhiteSpace(result)) result = null;
            return result;
        }

        /// <summary>
        /// gets a list of the string names of the groups the AD user is a member of
        /// </summary>
        /// <param name="loggedInUser">AD username of the user to look up</param>
        /// <param name="logger">instance of a logger to use</param>
        /// <returns>a list of the names of the groups</returns>
        public static List<string> GetGroupsForUser(string loggedInUser, CommonEventLogging logger)
        {
            //EventLogging log = new EventLogging();

            List<string> result = new List<string>();

            try
            {
                // establish domain context
                PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain, MediaRelationsLibrary.Configuration.App.Settings.ActiveDirectoryDomain);

                // find your user
                UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, loggedInUser);

                // if found - grab its groups
                if (user != null)
                {
                    PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();

                    // iterate over all groups
                    for (int i = 0; i < groups.Count(); i++)
                    {
                        try
                        {
                            Principal p = groups.ElementAt(i);
                            // make sure to add only group principals
                            if (p is GroupPrincipal)
                            {
                                result.Add(((GroupPrincipal)p).Name);
                                //HttpContext.Current.Response.Write(p.Name + "<br/>\n");
                            }
                        } catch { }
                    }
                }
            } catch (Exception ex) {
                logger.WriteException(ex, "Permissions GetGroupsForUser");
            }
            return result;
        }
    }
}
