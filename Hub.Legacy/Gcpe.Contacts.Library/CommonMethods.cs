using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

namespace MediaRelationsLibrary
{
    public class CommonMethods
    {
        public const string RECORD_EDITED_MESSAGE = "This record currently has pending changes and is locked for editing";
        public const string RECORD_EDITED_MESSAGE_ADMIN = "There are pending changes to this record. You must approve or reject the changes.";

        public const string DATE_FORMAT_STR = "yyyy/MM/dd HH:mm:ss";
        public const string FRIENDLY_DATE_FORMAT_STR = "MMM dd, yyyy @ h:mm tt";
        public const string FULL_DATE_STR = "dddd MMMM dd yyyy @ h:mm tt";

        public const string MEDIA_RELATIONS_SYSTEM = "MediaRelationsSystem";
        public const string PURGE_TASK_SYSTEM = "PurgeTask";

        public enum AddressType
        {
            Physical = 1,
            Mailing = 2
        }

        static object regionLockObj = new object();
        static object cityLockObj = new object();
        static object districtLockObj = new object();
        static object sectorLockObj = new object();
        static object mediaJobTitleLockObj = new object();
        static object ministryJobTitleLockObj = new object();
        static object provinceLockObj = new object();
        static object countryLockObj = new object();
        static object mediaDeskLockObj = new object();
        static object mediaTypeLockObj = new object();
        static object distributionLockObj = new object();
        static object publicationDaysLockObj = new object();
        static object publicationFrequencyLockObj = new object();
        static object printCategoryLockObj = new object();
        static object ministryLockObj = new object();
        static object ministerLockObj = new object();
        static object ministerialJobTitleLockObj = new object();
        static object ethnicityLockObj = new object();
        static object languageLockObj = new object();
        static object specialtyPublicationLockObj = new object();

        public static string GetResponsivePrintStyles(string filePath, string printPath)
        {
            StringBuilder result = new StringBuilder();
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filePath);
                string css = sr.ReadToEnd();
                int index1 = css.IndexOf("/**media609start**/");
                int index2 = css.IndexOf("/**media609end**/");
                if (index1 >= 0 && index2 > index1)
                {
                    result.Append(css.Substring(index1, (index2 - index1)) + "\n\n");
                }
            }
            catch { }
            finally
            {
                if (sr != null) sr.Close();
            }
            sr = null;
            try
            {
                sr = new StreamReader(printPath);
                string css = sr.ReadToEnd();

                result.Append(css + "\n\n");
            }
            catch { }
            finally
            {
                if (sr != null) sr.Close();
            }

            return result.ToString();
        }


        /// <summary>
        /// gets the currently logged in user based on request headers
        /// </summary>
        /// <returns>string of the domain and username of the logged in user (eg DOMAIN\accountname)</returns>
        public static string GetLoggedInUser()
        {
            string result = HttpContext.Current.Request.ServerVariables["REMOTE_USER"];
            

            if (HttpContext.Current.Session["MediaRelations_LoggedInUser"] == null)
            {
                HttpContext.Current.Session.Add("MediaRelations_LoggedInUser", result);
            }

            return result;
        }

        /// <summary>
        /// This method is used to send an email message
        /// </summary>
        /// <param name="fromEmail">The email address where the address is to come from</param>
        /// <param name="toEmail">The email address where the address is to be sent to</param>
        /// <param name="subject">The subject of the email</param>
        /// <param name="message">The message of the email</param>
        /// <param name="isHtml">Whether the mail is HTML (true) or plain text (false)</param>
        public static void SendEmail(string fromEmail, string toEmail, string subject, string message, bool isHtml)
        {
            string server = "";
            CommonEventLogging logger = new CommonEventLogging();

            // todo create this config
            server = MediaRelationsLibrary.Configuration.App.Settings.SMTPServer;
            if (string.IsNullOrEmpty(server))
            {
                logger.WriteMissingConfigurationValue("SMTPServer");
            }

            if (subject == null) subject = "";
            MailMessage mail = new MailMessage(fromEmail, toEmail, subject, message);
            mail.IsBodyHtml = isHtml;

            SmtpClient client = new SmtpClient(server);
            try
            {
                client.Send(mail);
            }
            catch (Exception e)
            {
                logger.WriteException(e, "CommonMethods SendEmail");
            }

        }

        /// <summary>
        /// returns a dictionary of query string so that the values can be edited and added to
        /// </summary>
        /// <returns>a copy of the querystring key/value pairs in a dictionary object</returns>
        public static Dictionary<String, string> GetEditableQueryString()
        {
            Dictionary<String, string> result = new Dictionary<String, string>();
            foreach (string key in HttpContext.Current.Request.QueryString.AllKeys)
            {
                result[key] = HttpContext.Current.Request.QueryString[key];
            }
            return result;
        }

        /// <summary>
        /// Gets a list of query string - value in a querystring that allows multiple of the same key
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetEditableQueryStringMultiple()
        {
            List<KeyValuePair<string, string>> qs = new List<KeyValuePair<string, string>>();

            foreach (string key in HttpContext.Current.Request.QueryString)
            {

                if (HttpContext.Current.Request.QueryString.GetValues(key).Length > 1)
                {
                    string[] values = HttpContext.Current.Request.QueryString.GetValues(key);
                    foreach (string val in values) qs.Add(new KeyValuePair<string, string>(key, val));
                }
                else
                {
                    qs.Add(new KeyValuePair<string, string>(key, HttpContext.Current.Request.QueryString[key]));
                }
            }

            return qs;
        }

        /// <summary>
        /// removes all instances of the key in the query string
        /// </summary>
        /// <param name="qs"></param>
        /// <param name="item"></param>
        public static void RemoveItemFromQueryString(List<KeyValuePair<string, string>> qs, string item)
        {
            for (int i = qs.Count - 1; i >= 0; i--)
            {
                if (qs[i].Key == item) qs.RemoveAt(i);
            }
        }

        /// <summary>
        /// gets the query string (? not included) of a list of query string parameters - allows multiple
        /// </summary>
        /// <param name="qs"></param>
        /// <returns></returns>
        public static string GetQueryStringMultiple(List<KeyValuePair<string, string>> qs)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < qs.Count; i++)
            {
                if (i != 0) sb.Append("&");
                sb.Append(qs[i].Key + "=" + HttpContext.Current.Server.UrlEncode(qs[i].Value));
            }

            return sb.ToString();
        }

        /// <summary>
        /// returns a string representation of a collection of name/value pairs in query string format
        /// e.g. param1=value1&param2=value2
        /// </summary>
        /// <param name="qs">a dictionary of key/value pairs representing querystring values</param>
        /// <returns>a string representation of a collection of name/value pairs in query string format</returns>
        public static string GetQueryString(Dictionary<String, string> qs)
        {
            StringBuilder sb = new StringBuilder();
            bool fst = false;
            foreach (string key in qs.Keys)
            {
                if (fst) sb.Append("&");
                fst = true;
                sb.Append(key + "=" + HttpContext.Current.Server.UrlEncode(qs[key]));
            }
            return sb.ToString();
        }


        public static string AddSpacesCamelCase(string str)
        {
            string result = str;

            Regex uppercase = new Regex("[A-Z]");
            result = uppercase.Replace(result, " $0");

            return result.Trim();
        }
    }

    public class PhoneNumberInfo
    {
        public string PhoneNumber = null;
        public string PhoneNumberExtension = null;

        public string PhoneNumberString
        {
            get
            {
                if (PhoneNumber == null) return null;

                if (string.IsNullOrWhiteSpace(PhoneNumberExtension))
                {
                    return PhoneNumber.Trim();
                }
                return PhoneNumber.Trim() + " ext." + PhoneNumberExtension.Trim();
            }
        }

        public PhoneNumberInfo(string phoneNumber, string phoneNumberExtension)
        {
            PhoneNumber = phoneNumber;
            PhoneNumberExtension = phoneNumberExtension;
        }

        public static PhoneNumberInfo GetPhoneNumberInfo(string phoneNumberStr)
        {
            string phoneNumber = null;
            string phoneNumberExtension = null;

            Match match = Regex.Match(phoneNumberStr, @"([\w\-\(\)\s\.]+)ext\.([\w\-\.]+)");
            if (match.Success)
            {
                phoneNumber = match.Groups[1].Value;
                phoneNumberExtension = match.Groups[2].Value;
            }
            else
            {
                phoneNumber = phoneNumberStr;
            }

            return new PhoneNumberInfo(phoneNumber, phoneNumberExtension);
        }
    }
}
