using System;
using System.Diagnostics;
using System.Collections.Generic;
using MediaRelationsDatabase;

namespace MediaRelationsLibrary
{
    public class CommonEventLogging
    {

        public enum EntityType
        {
            Outlet = 0,
            Company = 1,
            Contact = 2,
            Language = 3,
            Region = 4,
            ElectoralDistrict = 5,
            City = 6,
            MediaType = 7,
            Ethnicity = 8,
            ProvState = 9,
            Country = 10,
            Distribution = 11,
            MediaDesk = 12,
            MediaJobTitle = 13,
            MinisterialJobTitle = 14,
            PrintCategory = 15,
            PublicationDay = 16,
            PublicationFrequency = 17,
            Sector = 18,
            SpecialtyPublication = 19,
            Search = 20,
            Ministry = 21,
            Minister = 22,
            PublicReport = 23,
            Beat = 24
        }

        public enum ActivityType
        {
            Change_Submitted = 1, // to be deleted
            Record_Submitted = 2, // to be deleted
            Record_Edited = 3,
            Record_Created = 4,
            Record_Deleted = 5,
            Change_Modified = 6, // to be deleted
            Change_Rejected = 7, // to be deleted
            Change_Approved = 8, // to be deleted
            Outlet_Attached = 9,
            Outlet_Detached = 10,
            Record_Approved = 11, // to be deleted
            Record_Rejected = 12, // to be deleted
            Region_Added = 13,
            Region_Modified = 14,
            Region_Deleted = 15,
            Electoral_District_Added = 16,
            Electoral_District_Modified = 17,
            Electoral_District_Deleted = 18,
            City_Added = 19,
            City_Modified = 20,
            City_Deleted = 21,
            MediaType_Added = 22,
            MediaType_Modified,
            MediaType_Deleted = 23,
            Language_Added = 24,
            Language_Modified = 25,
            Language_Deleted = 26,
            Ethnicity_Added = 27,
            Ethnicity_Modified = 28,
            Ethnicity_Deleted = 29,
            ProvState_Added = 30,
            ProvState_Modified = 31,
            ProvState_Deleted = 32,
            Country_Added = 33,
            Country_Modified = 34,
            Country_Deleted = 35,
            Distribution_Added = 30,
            Distribution_Modified = 31,
            Distribution_Deleted = 32,
            MediaDesk_Added = 33,
            MediaDesk_Modified = 34,
            MediaDesk_Deleted = 35,
            Media_Job_Added = 36,
            Media_Job_Modified = 37,
            Media_Job_Deleted = 38,
            Ministerial_Job_Added = 39,
            Ministerial_Job_Modified = 40,
            Ministerial_Job_Deleted = 41,
            Print_Category_Added = 42,
            Print_Category_Modified = 43,
            Print_Category_Deleted = 44,
            Publication_Day_Added = 45,
            Publication_Day_Modified = 46,
            Publication_Day_Deleted = 47,
            Publication_Frequency_Added = 48,
            Publication_Frequency_Modified = 49,
            Publication_Frequency_Deleted = 50,
            Sector_Added = 51,
            Sector_Modified = 52,
            Sector_Deleted = 53,
            Specialty_Publication_Added = 54,
            Specialty_Publication_Modified = 55,
            Specialty_Publication_Deleted = 56,
            Share = 57,
            Export = 58,
            Email = 69,
            Print = 70,
            //Record_Locked = 71,
            //Record_Unlocked = 72,
            Ministry_Added = 73,
            Ministry_Modified = 74,
            Ministry_Deleted = 75,
            Ministry_Activated = 76,
            Ministry_Deactived = 77,
            Minister_Deleted = 78,
            Minister_Added = 79,
            Minister_Modified = 80,
            Minister_Activated = 81,
            Minister_Deactivated = 82,
            Record_Purged = 83,
            Beat_Added = 84,
            Beat_Modified = 85,
            Beat_Deleted = 86
        }

        public enum SystemErrorTypes
        {
            Exception = 900,
            MissingConfiguration = 901
        }

        protected readonly bool doWindowsEventLogging;
        protected readonly string loggingConfigurationString;

        protected EventLog eventLog = null;

        public CommonEventLogging()
        {
        }

        private static void WriteActivityLogEntry(int actionType, int entityType, Guid entityId, string entityData, Guid eventId, string eventData, string eventUser)
        {
            //database stuff here
            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                SysLog log = new SysLog();
                log.Action = actionType.ToString();
                log.EntityType = entityType.ToString();
                log.EntityId = entityId;
                log.EntityData = entityData;
                log.EventId = eventId;
                log.EventData = eventData;
                log.EventUser = eventUser;
                log.EventDate = DateTime.Now;

                ctx.SysLogs.Add(log);
                ctx.SaveChanges();
            }
        }

        public static void WriteActivityLogEntry(ActivityType activityType, EntityType entityType, Guid entityId,
            string entityData, Guid EventId, string eventData, string eventUser)
        {

            WriteActivityLogEntry((int)activityType, (int)entityType, entityId, entityData, EventId, eventData, eventUser);
        }

        public static void WriteActivityLogEntry(Contact contact, string eventData, ActivityType activityType = ActivityType.Record_Edited)
        {
            WriteActivityLogEntry(activityType, EntityType.Contact, contact.Id, contact.FirstName + " " + contact.LastName,
                        Guid.Empty, eventData, CommonMethods.GetLoggedInUser());
        }

        public static void WriteActivityLogEntry(Company company, string eventData, ActivityType activityType = ActivityType.Record_Edited)
        {
            WriteActivityLogEntry(activityType, company.IsOutlet ? EntityType.Outlet : EntityType.Company, company.Id, company.CompanyName,
                        Guid.Empty, eventData, CommonMethods.GetLoggedInUser());
        }

        public static bool Remove<T>(ICollection<T> items, T item, bool condition)
        {
            if (!condition) return false;
            items.Remove(item);
            return true;
        }
        public static bool Add<T>(ICollection<T> items, T item)
        {
            if (items.Contains(item)) return false;
            items.Add(item);
            return true;
        }

        #region exception logging

        public void WriteConfigurationException(Exception e, string methodName, string appKey)
        {
            if (!MediaRelationsLibrary.Configuration.App.Settings.DoExceptionLogging)
            {
                throw new Exception(e + " ||| " + methodName + " ||| " + appKey);
            }

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {

                string action = ((int)CommonEventLogging.SystemErrorTypes.MissingConfiguration).ToString();

                SysLog log = new SysLog()
                {
                    Action = action,
                    EntityType = "Configuration",
                    EntityId = Guid.Empty,
                    EntityData = e.Message + " ||| " + methodName + " ||| " + appKey,
                    EventId = Guid.Empty,
                    EventData = e.StackTrace,
                    EventUser = CommonMethods.MEDIA_RELATIONS_SYSTEM,
                    EventDate = DateTime.Now
                };

                ctx.SysLogs.Add(log);
                ctx.SaveChanges();
            }
        }

        public void WriteException(Exception e, string source)
        {
            if (!MediaRelationsLibrary.Configuration.App.Settings.DoExceptionLogging)
            {
                throw new Exception(e.Message + "\n\n" + e.StackTrace + "\n\n" + source);
            }

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {

                string action = ((int)CommonEventLogging.SystemErrorTypes.Exception).ToString();

                SysLog log = new SysLog()
                {
                    Action = action,
                    EntityType = "Exception",
                    EntityId = Guid.Empty,
                    EntityData = e.Message + " ||| " + source,
                    EventId = Guid.Empty,
                    EventData = e.StackTrace,
                    EventUser = CommonMethods.MEDIA_RELATIONS_SYSTEM,
                    EventDate = DateTime.Now
                };

                ctx.SysLogs.Add(log);
                ctx.SaveChanges();
            }
        }

        public virtual void WriteMissingConfigurationValue(string appKey)
        {
            throw new Exception("Missing configuration value - " + appKey);
        }

        #endregion

    }
}
