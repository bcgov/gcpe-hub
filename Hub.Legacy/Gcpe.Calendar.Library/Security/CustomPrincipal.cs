using System;
using System.Security.Principal;
using System.Linq;
using System.Collections.Generic;

using CorporateCalendar.Data;
using Gcpe.Hub.Properties;

namespace CorporateCalendar.Security
{
    /// <summary>
    /// The Role Id MUST match the id of the Role in the Role Table in the database
    /// </summary>
    public enum SecurityRole { ReadOnly = 1, Editor, Advanced, Administrator, SysAdmin };

    /// <summary>
    /// Comments TBD
    /// </summary>
    public class CustomPrincipal : IPrincipal
    {
        /// <summary>
        /// Identity
        /// </summary>
        public IIdentity Identity { get; set; }

        public int Id { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string ExchangeName { get; set; }
        public string PhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Organization { get; set; }
        public string Department { get; set; }
        public bool IsAuthorized { get; set; }
        public string SessionId { get; set; }
        public bool IsInApplicationOwnerOrganizations { get; set; }
        public string GCPEHQ_Ministry { get; set; }
        public bool IsGCPEHQ { get; set; }
        public List<Guid> SystemUserMinistryIds { get; set; }
        public string SingleMinistryName { get; set; }

        /// <summary>
        /// Get Domain name
        /// </summary>
        public string Domain => Utility.User.GetDomainWithoutUserName(Identity);

        /// <summary>
        /// Get User Name 
        /// </summary>
        public string Username
        {
            get
            {
                string debugUsername = Environment.GetEnvironmentVariable("DebugUsername");
                return Utility.User.GetUserNameWithoutDomain(string.IsNullOrEmpty(debugUsername) ? Identity.Name : debugUsername);
            }
        }

        private string DbUsername { get; set; }

        public int? FilterDisplayValue { get; private set; }
        public string HiddenColumns { get; private set; }

        /// <summary>
        /// Populate the Custom Principal from user identity
        /// </summary>
        /// <param name="identity">user identity</param>
        public CustomPrincipal(IIdentity identity)
        {
            GCPEHQ_Ministry = Settings.Default.HQAdmin;
            if (identity == null) { throw new ArgumentException("Parameter identity cannot be null"); }

            Identity = identity;

            //Get user details from Active Directory
            Dictionary<string, string> exchangeUserInfo = DirectoryServices.GetUserInfo(Username);
            if (exchangeUserInfo == null) return; // not Authorized

            string temp;
            LastName = (exchangeUserInfo.TryGetValue("LastName", out temp)) ? temp : string.Empty;
            FirstName = (exchangeUserInfo.TryGetValue("FirstName", out temp)) ? temp : string.Empty;
            Email = (exchangeUserInfo.TryGetValue("Email", out temp)) ? temp : string.Empty;
            ExchangeName = (exchangeUserInfo.TryGetValue("ExchangeName", out temp)) ? temp : string.Empty;
            PhoneNumber = (exchangeUserInfo.TryGetValue("PhoneNumber", out temp)) ? temp : string.Empty;
            StreetAddress = (exchangeUserInfo.TryGetValue("StreetAddress", out temp)) ? temp : string.Empty;
            City = (exchangeUserInfo.TryGetValue("City", out temp)) ? temp : string.Empty;
            Province = (exchangeUserInfo.TryGetValue("Province", out temp)) ? temp : string.Empty;
            PostalCode = (exchangeUserInfo.TryGetValue("PostalCode", out temp)) ? temp : string.Empty;
            Organization = (exchangeUserInfo.TryGetValue("Organization", out temp)) ? temp : string.Empty;
            Department = (exchangeUserInfo.TryGetValue("Department", out temp)) ? temp : string.Empty;
            Title = (exchangeUserInfo.TryGetValue("Title", out temp)) ? temp : string.Empty;

            using (var dc = new CorporateCalendarDataContext())
            {
                // get the user
                var systemUser = dc.SystemUsers.Where(u => u.Username.ToUpper() == Username.ToUpper() && u.IsActive == true).SingleOrDefault();

                if (systemUser != null)
                {
                    Id = systemUser.Id;
                    RoleId = systemUser.RoleId;
                    IsAuthorized = true;
                    DbUsername = systemUser.Username;
                    FilterDisplayValue = systemUser.FilterDisplayValue;
                    HiddenColumns = systemUser.HiddenColumns;
                    RoleName = ((SecurityRole)RoleId).ToString();
                }
                var systemUserMinistryList = dc.SystemUserMinistries.Where(um => um.SystemUserId == Id && um.Ministry.IsActive && um.IsActive)
                    .Select(um => new { um.MinistryId, um.Ministry.Abbreviation }).ToList();
                SystemUserMinistryIds = systemUserMinistryList.Select(m => m.MinistryId??Guid.Empty).ToList();
                string[] applicationOwners = ApplicationOwners;
                IsInApplicationOwnerOrganizations = systemUserMinistryList.Any(m => applicationOwners.Contains(m.Abbreviation));
                IsGCPEHQ = systemUserMinistryList.Any(m => m.Abbreviation == GCPEHQ_Ministry);
                if (systemUserMinistryList.Count == 1)
                    SingleMinistryName = systemUserMinistryList[0].Abbreviation;
                else
                    SingleMinistryName = string.Empty;
            }

            SessionId = string.Empty;
        }

        /// <summary>
        /// Check user role
        /// </summary>
        /// <param name="securityRole">role</param>
        /// <returns>whether user is in role or not</returns>
        public bool IsInRole(string securityRole)
        {
            switch (securityRole)
            {
                case "ReadOnly":
                    return true;
                case "Editor":
                    return true;
                case "Advanced":
                    return true;
                case "Administrator":
                    return true;
                case "SysAdmin":
                    return true;
                default:
                    string errorMessage = string.Format("Security Role {0} does not exist.", securityRole);
                    var log = new Logging.Log(errorMessage, Logging.Log.LogType.Error);
                    return false;
            }
        }
        /// <summary>
        /// Does security role match role id.
        /// </summary>
        /// <param name="securityRole"></param>
        /// <returns></returns>
        public bool IsInRole(SecurityRole securityRole)
        {
            return (RoleId == Convert.ToInt32(securityRole));
        }

        public bool IsInRoleOrGreater(SecurityRole securityRole)
        {
            return (RoleId >= Convert.ToInt32(securityRole));
        }

        /// <summary>
        /// Gets the list of ministries which own CorpCal
        /// </summary>
        /// <returns></returns>

        private string[] ApplicationOwners
        {
            get
            {
                string applicationOwnerOrganizationsCSVs = Settings.Default.ApplicationOwnerOrganizations;
                if (string.IsNullOrEmpty(applicationOwnerOrganizationsCSVs))
                    throw new System.Exception("owner id's not set");


                return applicationOwnerOrganizationsCSVs.Split(','); // get individual values
            }
        }
    }
}