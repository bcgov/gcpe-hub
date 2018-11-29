extern alias legacy;
using System;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using legacy::Gcpe.Hub.Data.Entity;

namespace Gcpe.News.ReleaseManagement
{
    public static class Utils
    {
        public static void LogError(string error, string description)
        {
            try
            {
                Utils.LogMessage(String.Format("Error: {0}\r\n{1}", error, description));
                // TODO: Log to Errors list
            }
            catch
            {
                // Do nothing
            }
        }

        public static void LogError(Exception ex)
        {
            Utils.LogError(ex.Message, ex.StackTrace);
        }

        public static void LogError(string message, Exception ex)
        {
            Utils.LogError(message + ": " + ex.Message, ex.StackTrace);
        }

        public static void LogMessage(string strMessage)
        {
            Debug.WriteLine(strMessage);
            try
            {
                string path = HostingEnvironment.MapPath("~") + @"\..\Log Files\";
                if (Directory.Exists(path))
                { 
                    File.AppendAllText(path + "QueueBackgroundWorkItem.log", DateTime.Now.ToString("MMM d HH:mm:ss") + ": " + strMessage + "\r\n");
                }
            }
            catch
            {
                // Do nothing
            }
        }

        public static void UnpublishFiles(HubEntities db, NewsRelease nr, string publishSubfolder)
        {
            //string strDestDir = Common.GetAppSetting(Constants.PublishLocation);
            
            //if (!String.IsNullOrEmpty(strDestSubDir))
            //{
            //    strDestDir += "\\" + strDestSubDir;
            //}

            string strDestDir = publishSubfolder;

            string strNrNumber = nr.Key;

            if (File.Exists(strDestDir + "\\" + strNrNumber + ".htm"))
            {
                File.WriteAllText(strDestDir + "\\" + strNrNumber + ".htm", "");
                File.SetAttributes(strDestDir + "\\" + strNrNumber + ".htm", File.GetAttributes(strDestDir + "\\" + strNrNumber + ".htm") | FileAttributes.Archive);
            }
            if (File.Exists(strDestDir + "\\" + strNrNumber + ".pdf"))
            {
                File.WriteAllText(strDestDir + "\\" + strNrNumber + ".pdf", "");
                File.SetAttributes(strDestDir + "\\" + strNrNumber + ".pdf", File.GetAttributes(strDestDir + "\\" + strNrNumber + ".pdf") | FileAttributes.Archive);
            }
            if (File.Exists(strDestDir + "\\" + strNrNumber + ".txt"))
            {
                File.WriteAllText(strDestDir + "\\" + strNrNumber + ".txt", "");
                File.SetAttributes(strDestDir + "\\" + strNrNumber + ".txt", File.GetAttributes(strDestDir + "\\" + strNrNumber + ".txt") | FileAttributes.Archive);
            }

            // Check if there are subdirs and add files in 
            if (Directory.Exists(strDestDir + "\\" + strNrNumber + "_files"))
            {
                foreach (string file in Directory.GetFiles(strDestDir + "\\" + strNrNumber + "_files"))
                {
                    if (File.Exists(file))          // Avoids file ending with . etc.
                    {
                        Utils.LogMessage(String.Format("UnPublishFiles: {0}", file));

                        File.WriteAllText(file, "");
                        File.SetAttributes(file, File.GetAttributes(file) | FileAttributes.Archive);
                    }
                }
            }

            // Run the deploy exe right away 
            //PublishManager.DeployToWeb();
        }

        public static string TrimSummary(string summaryText, int length)
        {
            if (summaryText.Length > length)
            {
                summaryText = summaryText.Substring(0, length - 3);

                while (!summaryText.EndsWith(" ") && !summaryText.EndsWith(".") && !summaryText.EndsWith("!") && !summaryText.EndsWith("?"))
                    summaryText = summaryText.Substring(0, summaryText.Length - 1);

                summaryText = summaryText.Trim();

                if (!summaryText.EndsWith(".") && !summaryText.EndsWith("!") && !summaryText.EndsWith("?"))
                {
                    if (Char.IsPunctuation(summaryText[summaryText.Length - 1]))
                        summaryText = summaryText.Substring(0, summaryText.Length - 1);

                    summaryText = summaryText.Trim();

                    summaryText += "...";
                }
            }

            return summaryText;
        }
    }
}