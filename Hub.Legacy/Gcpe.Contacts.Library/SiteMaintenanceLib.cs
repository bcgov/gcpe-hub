using System;
using System.Collections.Generic;
using System.Linq;
using MediaRelationsDatabase;
using System.Web;
using System.Web.SessionState;

namespace MediaRelationsLibrary
{
    public class SiteMaintenanceLib
    {

        private static object purgeLockObj = new object();

        public SiteMaintenanceLib() { }

        /// <summary>
        /// Called on session start to determine whether to run the site purge or not
        /// This also calls the lock cleaner if it determines to run the purge
        /// </summary>
        public void RunPurge()
        {
            bool doPurge = false;
            lock (purgeLockObj)
            {

                if (HttpContext.Current.Application["MediaRelations_PurgeLockCheck"] != null)
                {

                    int frequencyHours = MediaRelationsLibrary.Configuration.App.Settings.PurgeTaskFrequencyHours;
                    DateTime dateToCheck = DateTime.Now.AddHours(frequencyHours * -1);
                    DateTime lastRunDate = DateTime.MinValue;
                    try
                    {
                        lastRunDate = (DateTime)HttpContext.Current.Application["MediaRelations_PurgeLockCheck"];
                    }
                    catch (Exception) { }

                    if (dateToCheck > lastRunDate || lastRunDate == DateTime.MinValue)
                    {
                        doPurge = true;
                    }
                }
                else
                {
                    doPurge = true;
                }

                if (doPurge)
                {
                    HttpContext.Current.Application.UnLock();

                    DateTime currentDate = DateTime.Now;
                    if (HttpContext.Current.Application["MediaRelations_PurgeLockCheck"] == null)
                    {
                        HttpContext.Current.Application.Add("MediaRelations_PurgeLockCheck", currentDate);
                    }
                    else
                    {
                        HttpContext.Current.Application["MediaRelations_PurgeLockCheck"] = currentDate;
                    }

                    HttpContext.Current.Application.Lock();

                    RunSitePurge();
                }
            }
        }

        /// <summary>
        /// This method runs the purge against the database to remove deleted
        /// records that are a certain amount of days
        /// </summary>
        private void RunSitePurge()
        {
            int companyPurgeDays = MediaRelationsLibrary.Configuration.App.Settings.CompanyPurgeDays;
            int contactPurgeDays = MediaRelationsLibrary.Configuration.App.Settings.ContactPurgeDays;
            int lookBackdays = MediaRelationsLibrary.Configuration.App.Settings.LookbackDays;


            ContactAdminLib contactAdminLib = new ContactAdminLib();
            CompanyAdministrationLib companyAdminLib = new CompanyAdministrationLib();

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {

                string deletedAction = ((int)CommonEventLogging.ActivityType.Record_Deleted).ToString();

                var baseLogsSearch = (from c in ctx.SysLogs select c);

                #region contact purge

                string contactEntityType = ((int)CommonEventLogging.EntityType.Contact).ToString();

                var logs = baseLogsSearch.Where(c => c.EntityType == contactEntityType);

                DateTime earliestDate = DateTime.Now.AddDays(contactPurgeDays * -1);
                DateTime latestDate = earliestDate.AddDays(lookBackdays * -1);

                logs = logs.Where(c => c.EventDate < earliestDate && c.EventDate > latestDate);

                List<SysLog> logList = logs.ToList();

                foreach (SysLog log in logList)
                {
                    Guid entityId = (Guid)log.EntityId;

                    Contact contact = (from con in ctx.Contacts where con.Id == entityId where !con.IsActive select con).FirstOrDefault();
                    if (contact != null)
                    {
                        contactAdminLib.PermanentlyDeleteContact(contact, ctx, CommonMethods.PURGE_TASK_SYSTEM);
                    }
                }


                #endregion

                #region company purge

                string companyEntityType = ((int)CommonEventLogging.EntityType.Company).ToString();

                logs = baseLogsSearch.Where(c => c.EntityType == companyEntityType);

                earliestDate = DateTime.Now.AddDays(companyPurgeDays * -1);
                latestDate = earliestDate.AddDays(lookBackdays * -1);

                logs = logs.Where(c => c.EventDate < earliestDate && c.EventDate > latestDate);

                logList = logs.ToList();

                foreach (SysLog log in logList)
                {
                    Guid entityId = (Guid)log.EntityId;

                    Company company = (from cmp in ctx.Companies where cmp.Id == entityId where !cmp.IsActive select cmp).FirstOrDefault();
                    if (company != null)
                    {
                        companyAdminLib.PermanentlyDeleteCompany(company, ctx, CommonMethods.PURGE_TASK_SYSTEM);
                    }
                }

                #endregion

            }
        }
    }
}
