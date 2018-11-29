using System;
using System.Security.Principal;

namespace CorporateCalendar.Utility
{
    public class User
    {
        public static string GetUserNameWithoutDomain(string identityName) {
            if (identityName == null) { throw new ArgumentException("Parameter (identityName) cannot be null."); }
            return identityName.Substring(identityName.IndexOf(@"\") + 1);
        }

        public static string GetDomainWithoutUserName(IIdentity identity) {
            if (identity == null) { throw new ArgumentException("Parameter (identity) cannot be null."); }
            return identity.Name.Split('\\')[0];
        }
    }
}
