using System;
using System.Net.Mail;
using Gcpe.Hub.Properties;

namespace CorporateCalendar.Logging
{
    /// <summary>
    /// Log class for application instrumentation
    /// </summary>
    public class Log
    {

        /// <summary>
        /// Enumerates log types
        /// </summary>
        public enum LogType
        {
            /// <summary>
            /// Error message: Used to indicate a system error
            /// </summary>
            Error = 1,
            /// <summary>
            /// Warning message: Used to indicate a system warning
            /// </summary>
            Warning,
            /// <summary>
            /// General message: Used to indicate a user-defined message for information only
            /// </summary>
            Message,
            /// <summary>
            /// NewsFeed message: Used to indicate a message for the Corporate News Feed
            /// </summary>
            NewsFeed,
            /// <summary>
            /// Suggestion message: Used to indicate a suggestion made by the user
            /// </summary>
            Suggestion

        };

        public enum OperationType
        {
            /// <summary>
            /// Error message: Used to indicate a system error
            /// </summary>
            Insert = 1,
            /// <summary>
            /// Warning message: Used to indicate a system warning
            /// </summary>
            Update,
            /// <summary>
            /// General message: Used to indicate a user-defined message for information only
            /// </summary>
            Delete,
            /// <summary>
            /// NewsFeed message: Used to indicate a message for the Corporate News Feed
            /// </summary>
            Deactivate,
            /// <summary>
            /// NewsFeed message: Used to indicate a message for the Corporate News Feed
            /// </summary>
            Edit
        };


        /// <summary>
        /// Log constructor
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="logType">The type of message. Enumerated in the CorporateCalendar.Logging.Log.LogType enumeration</param>
        public Log(string message, LogType logType)
        {
            Message = message;
            System.Diagnostics.Trace.WriteLine(this.ToString(), logType.ToString());
        }

        public Log(int activityId, string message, LogType logType)
        {
            Message = message;
            System.Diagnostics.Trace.WriteLine(this.ToString(), logType.ToString());
        }

        //This is not being used as throwing a connection string error
        //public static void LogHistory(int activityId, LogType logType, string tableName, string fieldName,
        //    string oldValue, string newValue, OperationType operationType, string lastUpdatedBy)
        //{
        //    SqlConnection sqlConnection = new SqlConnection(CorporateCalendar.Configuration.Reader.GetConnectionString());
        //    using (sqlConnection)
        //    {
        //        CorporateCalendar.Data.CorporateCalendarDataContext dc =
        //            new Data.CorporateCalendarDataContext(sqlConnection);

        //        bool isActive = true;

        //        CorporateCalendar.Data.Log p = new CorporateCalendar.Data.Log
        //        {
        //            ActivityId = 1,
        //            LogType = (int)logType,
        //            TableName = tableName,
        //            FieldName = fieldName,
        //            OldValue = oldValue,
        //            NewValue = newValue,
        //            LastUpdatedDateTime = DateTime.Now,
        //            LastUpdatedBy = 1, // TO DO:
        //            CreatedDateTime = DateTime.Now,
        //            Operation = operationType.ToString(),
        //            IsActive = isActive
        //        };

        //        dc.GetTable<CorporateCalendar.Data.Log>().InsertOnSubmit(p);
        //        dc.SubmitChanges();
        //    }
        //}

        /// <summary>
        /// Log constructor
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="logType">The type of message. Enumerated in the CorporateCalendar.Logging.Log.LogType enumeration</param>
        /// <param name="sendEmail">If set to true will send an email to the distribution Corporate Calendar distribution list</param>
        public Log(string message, LogType logType, bool sendEmail)
        {
            Message = message;
            System.Diagnostics.Trace.WriteLine(this.ToString(), logType.ToString());

            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress(Settings.Default.LogMailFrom);
            mailMessage.To.Add(new MailAddress(Settings.Default.LogMailTo));
            mailMessage.Subject = string.Format("Corporate Calendar logged an event of type: {0}", logType.ToString());
            mailMessage.Body = Message;

            SmtpClient client = new SmtpClient(Settings.Default.SMTPServer);

            try
            {
                client.Send(mailMessage);
            }
            catch (SmtpException e)
            {
                CorporateCalendar.Logging.Log log = new CorporateCalendar.Logging.Log(
                    string.Format("{0}", e.ToString()), CorporateCalendar.Logging.Log.LogType.Error);
            }

        }
        /// <summary>
        /// Log Message - a simple string.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Timestamp to be associated with log message.
        /// 
        /// The Sortable ("s") Format Specifier
        /// The "s" standard format specifier represents a custom date and time format string 
        /// that is defined by the DateTimeFormatInfo.SortableDateTimePattern property. The 
        /// pattern reflects a defined standard (ISO 8601), and the property is read-only. 
        /// Therefore, it is always the same, regardless of the culture used or the format 
        /// provider supplied. The custom format string is "yyyy'-'MM'-'dd'T'HH':'mm':'ss".
        /// When this standard format specifier is used, the formatting or parsing operation 
        /// always uses the invariant culture. 
        /// </summary>
        public string SortableDateTime
        {
            get
            {
                return DateTime.Now.ToString("s");
            }
        }

        /// <summary>
        /// Override to return log message for TraceWriter
        /// </summary>
        /// <returns>A formatted string consisting of a SortableDateTime and  Message</returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", SortableDateTime, Message);
        }

    }
}