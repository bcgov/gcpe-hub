using System;
using System.Security.Principal;
using System.Web;
using System.DirectoryServices.AccountManagement;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Configuration;
using System.Web.Configuration;
using Gcpe.Hub.Properties;

namespace Gcpe.Hub
{
    public static class IIdentityExtensions
    {
        /// <summary>
        /// keys to use as a base for session vars to hold these values so we don't have to go back to AD server often
        /// </summary>
        const string guidSessionKey = "GCPE_SESSION_GUID";
        const string emailSessionKey = "GCPE_SESSION_EMAIL";
        const string displayNameSessionKey = "GCPE_SESSION_DISPLAYNAME";
        const string groupsSessionKey = "GCPE_SESSION_GROUPS";

        /// <summary>
        /// gets the user name for the identity
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetUserName(this IIdentity identity)
        {
            return identity.Name;
        }

        /// <summary>
        /// gets the guid (not sid) for the AD user identified by the identity.name
        /// stores in a session object based on the guid session key and the user name
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static Guid GetGuid(this IIdentity identity)
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DebugUsername")))
                return Guid.Empty;

            var section = ConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;
            int cacheExpiryMinutes = (int)section.Timeout.TotalMinutes;

            string userGuidSessionKey = guidSessionKey + identity.Name;

            Guid result;

            if (HttpContext.Current.Cache == null || HttpContext.Current.Cache[userGuidSessionKey] == null)
            {
                PrincipalContext context = new PrincipalContext(ContextType.Domain, Settings.Default.ActiveDirectoryDomain);
                UserPrincipal user = UserPrincipal.FindByIdentity(context, identity.Name);

                result = user.Guid.Value;

                if (HttpContext.Current.Cache != null)
                    HttpContext.Current.Cache.Add(userGuidSessionKey, result, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, cacheExpiryMinutes, 0), CacheItemPriority.Normal, null);
            }
            else
            {
                result = (Guid)HttpContext.Current.Cache[userGuidSessionKey];
            }

            return result;
        }

        /// <summary>
        /// gets the email for the AD user identified by the identity.name
        /// stores in a session object based on the email session key and the user name
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetEmailAddress(this IIdentity identity)
        {
            var section = ConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;
            int cacheExpiryMinutes = (int)section.Timeout.TotalMinutes;

            string userEmailSessionKey = emailSessionKey + identity.Name;
            string result = null;
            if (HttpContext.Current.Cache == null || HttpContext.Current.Cache[userEmailSessionKey] == null)
            {
                PrincipalContext context = new PrincipalContext(ContextType.Domain, Settings.Default.ActiveDirectoryDomain);
                UserPrincipal user = UserPrincipal.FindByIdentity(context, identity.Name);

                result = user?.EmailAddress ?? "";

                if (HttpContext.Current.Cache != null) HttpContext.Current.Cache.Add(userEmailSessionKey, result, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, cacheExpiryMinutes, 0), CacheItemPriority.Normal, null);
            }
            else
            {
                result = HttpContext.Current.Cache[userEmailSessionKey] as string;
            }
            return result;
        }

        /// <summary>
        /// gets the display name for the AD user identified by the identity.name
        /// stores in a session object based on the display name session key and the user name
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetDisplayName(this IIdentity identity)
        {
            string debugUsername = Environment.GetEnvironmentVariable("DebugUsername");
            if (!string.IsNullOrEmpty(debugUsername))
                return "Developer " + debugUsername;

            var section = ConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;
            int cacheExpiryMinutes = (int)section.Timeout.TotalMinutes;

            string userDisplayNameSessionKey = displayNameSessionKey + identity.Name;
            string result = null;
            if (HttpContext.Current.Cache == null || HttpContext.Current.Cache[userDisplayNameSessionKey] == null)
            {
                PrincipalContext context = new PrincipalContext(ContextType.Domain, Settings.Default.ActiveDirectoryDomain);
                UserPrincipal user = UserPrincipal.FindByIdentity(context, identity.Name);

                result = user.DisplayName.ToString();

                if (HttpContext.Current.Cache != null) HttpContext.Current.Cache.Add(userDisplayNameSessionKey, result, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, cacheExpiryMinutes, 0), CacheItemPriority.Normal, null);
            }
            else
            {
                result = HttpContext.Current.Cache[userDisplayNameSessionKey] as string;
            }
            return result;
        }

        /// <summary>
        /// gets the groups for the AD user identified by the identity.name
        /// stores in a session object based on the groups session key and the user name
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static List<string> GetGroups(this IIdentity identity)
        {
            return GetGroups(identity.Name);
        }

        //This method does not work in the new domain, use IsMember method instead
        public static List<String> GetGroups(string username)
        {
            var section = ConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;
            int cacheExpiryMinutes = (int)section.Timeout.TotalMinutes;

            string userGroupsSessionKey = groupsSessionKey + username;

#if DEBUG
            if (HttpContext.Current.Request.QueryString["clearGroups"] != null)
                HttpContext.Current.Cache.Remove(userGroupsSessionKey);
#endif

            List<String> result = new List<string>();
            if (HttpContext.Current.Cache == null || HttpContext.Current.Cache[userGroupsSessionKey] == null)
            {
                PrincipalContext context = new PrincipalContext(ContextType.Domain, Settings.Default.ActiveDirectoryDomain);
                UserPrincipal user = UserPrincipal.FindByIdentity(context, username);

                var authGroups = user.GetAuthorizationGroups(); //this is recursive on the AD server, much faster than trying to perform the recursion on our side with multiple calls to the AD server
                if (authGroups != null)
                {
                    var x = authGroups.ElementAt(0);
                    for (var i = 0; i < authGroups.Count(); i++)
                    {
                        //this try-catch block was required because some users had a NoMatchingPrincipalException for one or more groups
                        //the root cause is likely unmapped SIDs in the active directory
                        try
                        {
                            var group = authGroups.ElementAtOrDefault(i);
                            if (group != null && !String.IsNullOrWhiteSpace(group.Name) && !result.Contains(group.Name.Trim())) result.Add(group.Name.Trim());
                        }
                        catch (Exception)
                        {
                            //ignore groups that cause an exception - caused by invalid.orphaned SIDs
                        }
                    }

                }


                if (HttpContext.Current.Cache != null) HttpContext.Current.Cache.Add(userGroupsSessionKey, result, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, cacheExpiryMinutes, 0), CacheItemPriority.Normal, null);
            }
            else
            {
                result = HttpContext.Current.Cache[userGroupsSessionKey] as List<string>;
            }
            return result;
        }

        public static bool IsMember(this IIdentity identity, List<string> groups)
        {
            string username = identity.Name;
            string userGroupsSessionKey = groupsSessionKey + username;
#if DEBUG
            if (HttpContext.Current.Request.QueryString["clearGroups"] != null)
                HttpContext.Current.Cache.Remove(userGroupsSessionKey);
#endif

            if (HttpContext.Current.Cache == null || HttpContext.Current.Cache[userGroupsSessionKey] == null)
            {
                PrincipalContext context = new PrincipalContext(ContextType.Domain, Settings.Default.ActiveDirectoryDomain);
                UserPrincipal user = UserPrincipal.FindByIdentity(context, identity.Name);
                if (user != null)
                {
                    foreach (string groupName in groups)
                    {
                        var group = GroupPrincipal.FindByIdentity(context, groupName);
                        if (group != null && group.GetMembers(true).Any(member => user.SamAccountName.ToLower() == member.SamAccountName.ToLower()))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}