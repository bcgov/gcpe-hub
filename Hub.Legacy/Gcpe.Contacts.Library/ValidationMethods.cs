using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace MediaRelationsLibrary {
    /// <summary>
    /// This class contains methods that are used to validate certain peices 
    /// of data such as emails being proper email addresses.
    /// </summary>
    public class ValidationMethods {

        /// <summary>
        /// This method checks the inputted email address against the 
        /// .NET MailAddress and then some Regex to ensure validity
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>bool</returns>
        public static bool CheckEmailForValidity(string emailAddress) {
            emailAddress = emailAddress.Trim();
            if (string.IsNullOrEmpty(emailAddress)) {
                return false;
            }

            // first check is to use the built in mailaddress
            try {
                MailAddress ma = new MailAddress(emailAddress);
            } catch {
                return false;
            }

            // check against regular expression
            string pattern = "^[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,4}$";
            bool isValidEmail = Regex.IsMatch(emailAddress, pattern, RegexOptions.IgnoreCase);

            return isValidEmail;
        }
        
        /// <summary>
        /// This method is used to check if the website given is valid. This checks
        /// to make sure the website starts with http:// or https:// and ends with a 
        /// . and 2-4 characters.
        /// </summary>
        /// <param name="websiteUrl"></param>
        /// <returns>bool</returns>
        public static bool CheckWebsiteForValidity(string websiteUrl) {
            string pattern = "^https?\\://[\\w\\-]+\\.[a-z]{2,4}";

            if (!Regex.IsMatch(websiteUrl, pattern, RegexOptions.IgnoreCase)) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This method is used to check for a 'valid' phone number
        /// This just checks to see if there are at least 10 digits in the provided
        /// string
        /// </summary>
        /// <param name="phoneNumber">the phone number to check against</param>
        /// <returns>true if it found 10 or more digits</returns>
        public static bool CheckForValidPhoneNumber(string phoneNumber) {
            string phoneNumberBasic = Regex.Replace(phoneNumber, "[- .]","");
            if (phoneNumber.Length < 10) return false;
            return true;
        }

    }
}
