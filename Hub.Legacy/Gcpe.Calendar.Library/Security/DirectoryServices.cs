using System;
using System.DirectoryServices;
using System.Collections.Generic;

namespace CorporateCalendar.Security
{
    /// <summary>
    /// Exposes methods to facilitate communication with the BC Government Active Directory domain controllers
    /// </summary>
    public class DirectoryServices
    {
        private static DirectoryEntry GetDirectoryEntry()
        {
            DirectoryEntry directoryEntry = new DirectoryEntry();
            directoryEntry.Path = CorporateCalendar.Configuration.App.Settings.LdapUrl;
            directoryEntry.AuthenticationType = AuthenticationTypes.Secure;
            string debugLdapPwd = Environment.GetEnvironmentVariable("DebugLdapPwd");
            if (!string.IsNullOrEmpty(debugLdapPwd))
            {
                // required for contractors not part of the configured AD domain
                directoryEntry.Username = Configuration.App.Settings.DebugLdapUser;
                directoryEntry.Password = debugLdapPwd;
            }

            return directoryEntry;
        }

        /// <summary>
        /// Tests to see if a given user exists in the Active Directory
        /// </summary>
        /// <param name="username">Username with or without the domain prefix</param>
        /// <returns>True or False</returns>
        public static bool UserExists(string username)
        {
            string account = Utility.User.GetUserNameWithoutDomain(username);
            DirectoryEntry directoryEntry = GetDirectoryEntry();
            DirectorySearcher directorySearcher = new DirectorySearcher();
            directorySearcher.SearchRoot = directoryEntry;
            directorySearcher.Filter = string.Format("(SAMAccountName={0})", account);
            SearchResultCollection searchResultCollection = directorySearcher.FindAll();

            return searchResultCollection.Count > 0;
        }

        /// <summary>
        /// Returns a given user's Exchange name for reference 
        /// </summary>
        /// <param name="username">Username with or without the domain prefix</param>
        /// <returns>The user's exchange name in the format Lastname, Firstname ORGANIZATION_NAME:EX </returns>
        public static string GetExchangeName(string username)
        {
            DirectoryEntry directoryEntry = GetDirectoryEntry();
            string account = Utility.User.GetUserNameWithoutDomain(username);
            try
            {
                DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry);
                directorySearcher.Filter = "(SAMAccountName=" + account + ")";
                directorySearcher.PropertiesToLoad.Add("displayName");
                SearchResult result = directorySearcher.FindOne();
                if (result != null)
                {
                    return result.Properties["displayname"][0].ToString();
                }
                else
                {
                    return "User not found.";
                }
            }
            catch (System.Exception ex)
            {
                var log = new CorporateCalendar.Logging.Log(ex.Message, CorporateCalendar.Logging.Log.LogType.Error);
                return string.Empty;
            }
        }


        /// <summary>
        /// Returns a given user's Exchange name for reference 
        /// </summary>
        /// <param name="username">Username with or without the domain prefix</param>
        /// <returns>The user's information from Active directory in key / value pairs</returns>
        public static Dictionary<string, string> GetUserInfo(string username)
        {
            DirectoryEntry directoryEntry = GetDirectoryEntry();
            string account = Utility.User.GetUserNameWithoutDomain(username);

            try
            {
                DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry);
                directorySearcher.Filter = "(SAMAccountName=" + account + ")";

                directorySearcher.PropertiesToLoad.Add("name"); // exchange name
                directorySearcher.PropertiesToLoad.Add("sn"); // last name
                directorySearcher.PropertiesToLoad.Add("givenName"); // first name
                directorySearcher.PropertiesToLoad.Add("telephoneNumber");
                directorySearcher.PropertiesToLoad.Add("mail");  // email
                directorySearcher.PropertiesToLoad.Add("streetAddress"); // street address
                directorySearcher.PropertiesToLoad.Add("l"); // city
                directorySearcher.PropertiesToLoad.Add("st"); // province
                directorySearcher.PropertiesToLoad.Add("postalCode"); // postal code
                directorySearcher.PropertiesToLoad.Add("Company"); // ministry/organization
                directorySearcher.PropertiesToLoad.Add("Department"); // department / branch
                directorySearcher.PropertiesToLoad.Add("Title"); // title

                SearchResult result = directorySearcher.FindOne();

                if (result != null)
                {
                    Dictionary<string, string> userInfoDictionary = new Dictionary<string, string>();

                    if (result.Properties["name"] != null && result.Properties["name"].Count > 0)
                    {
                        userInfoDictionary.Add("ExchangeName", result.Properties["name"][0].ToString());
                    }
                    if (result.Properties["sn"] != null && result.Properties["sn"].Count > 0)
                    {
                        userInfoDictionary.Add("LastName", result.Properties["sn"][0].ToString());
                    }
                    if (result.Properties["givenName"] != null && result.Properties["givenName"].Count > 0)
                    {
                        userInfoDictionary.Add("FirstName", result.Properties["givenName"][0].ToString());
                    }
                    if (result.Properties["telephoneNumber"] != null && result.Properties["telephoneNumber"].Count > 0)
                    {
                        userInfoDictionary.Add("PhoneNumber", result.Properties["telephoneNumber"][0].ToString());
                    }
                    if (result.Properties["mail"] != null && result.Properties["mail"].Count > 0)
                    {
                        userInfoDictionary.Add("Email", result.Properties["mail"][0].ToString());
                    }
                    if (result.Properties["streetAddress"] != null && result.Properties["streetAddress"].Count > 0)
                    {
                        userInfoDictionary.Add("StreetAddress", result.Properties["streetAddress"][0].ToString());
                    }
                    if (result.Properties["l"] != null && result.Properties["l"].Count > 0)
                    {
                        userInfoDictionary.Add("City", result.Properties["l"][0].ToString());
                    }
                    if (result.Properties["st"] != null && result.Properties["st"].Count > 0)
                    {
                        userInfoDictionary.Add("Province", result.Properties["st"][0].ToString());
                    }
                    if (result.Properties["postalCode"] != null && result.Properties["postalCode"].Count > 0)
                    {
                        userInfoDictionary.Add("PostalCode", result.Properties["postalCode"][0].ToString());
                    }
                    if (result.Properties["Company"] != null && result.Properties["Company"].Count > 0)
                    {
                        userInfoDictionary.Add("Organization", result.Properties["Company"][0].ToString());
                    }
                    if (result.Properties["Department"] != null && result.Properties["Department"].Count > 0)
                    {
                        userInfoDictionary.Add("Department", result.Properties["Department"][0].ToString());
                    }
                    if (result.Properties["Title"] != null && result.Properties["Title"].Count > 0)
                    {
                        userInfoDictionary.Add("Title", result.Properties["Title"][0].ToString());
                    }

                    return userInfoDictionary;
                }
                else
                {
                    var log = new Logging.Log(
                        string.Format("An Active Directory lookup was performed for user ({0}) and no results were found.", username),
                        Logging.Log.LogType.Warning);

                    return null;
                }
            }
            catch (System.Exception ex)
            {
                var log = new CorporateCalendar.Logging.Log(ex.Message, CorporateCalendar.Logging.Log.LogType.Message);

                return null;
            }
        }

    }
}
