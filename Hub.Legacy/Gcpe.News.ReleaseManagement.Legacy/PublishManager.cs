extern alias legacy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using legacy::Gcpe.Hub.Data.Entity;

namespace Gcpe.News.ReleaseManagement
{

    public static class PublishManager
    {

        #region DeployToWeb Supporting Methods

        public static void DeployFiles(string publishPath, IEnumerable<string> deployPaths)
        {
            Console.WriteLine("DeployFiles Start");

            DirectoryInfo rootDirectory = new DirectoryInfo(publishPath);

            foreach (var file in GetCurrentCollectionFiles(rootDirectory))
            {
                Console.WriteLine("Checking " + file.FullName);

                if (FileAttributes.Archive == (file.Attributes & FileAttributes.Archive))
                {
                    string filePath = file.DirectoryName.Replace(rootDirectory.FullName.TrimEnd('\\'), "").TrimStart('\\');

                    Console.WriteLine("Deploying " + file.FullName + "...");

                    //int successful = 0;
                    foreach (string ProductionRoot in deployPaths)
                    {
                        var directory = new DirectoryInfo(ProductionRoot + filePath);
                        var destination = new System.IO.FileInfo(System.IO.Path.Combine(directory.FullName, file.Name));

                        if (file.Length == 0)
                        {
                            if (destination.Exists)
                                destination.Delete();
                        }
                        else
                        {
                            if (!directory.Exists)
                                directory.Create();

                            file.CopyTo(destination.FullName, true);
                        }

                        /*    successful++;
                        }
                        catch (IOException ioEx)
                        {
                            Console.WriteLine("Error Deploying to " + ProductionRoot + " (" + ioEx.Message + ")");
                        }*/
                    }

                    //if (deployPaths.Count() == successful)
                    file.Attributes ^= FileAttributes.Archive;
                }
            }

            Utils.LogMessage("DeployFiles End");
        }

        static IEnumerable<FileInfo> GetCurrentCollectionFiles(DirectoryInfo directory)
        {
            string currentCollectionName;
            using (HubEntities db = new HubEntities())
            {
                var currentCollection = (from collection in db.NewsReleaseCollections orderby collection.Name descending select collection).FirstOrDefault();
                if (currentCollection != null)
                    currentCollectionName = "news_releases_" + currentCollection.Name;
                else
                    currentCollectionName = "news_releases_2020-2024";
            }
            string path = Path.Combine(directory.FullName, currentCollectionName);

            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", "/c dir \"" + path + "\" /a:a /b");
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            processStartInfo.UseShellExecute = false;

            Process process = Process.Start(processStartInfo);

            string output;
            using (StreamReader streamReader = process.StandardOutput)
                output = streamReader.ReadToEnd();

            string error;
            using (StreamReader streamReader = process.StandardError)
                error = streamReader.ReadToEnd();

            foreach (string file in output.Replace("\r\n", "\n").Split('\n'))
            {
                if (file != "")
                    yield return new FileInfo(Path.Combine(path, file));
            }
        }

        static IEnumerable<FileInfo> GetFilesRecursive(DirectoryInfo directory)
        {
            string currentCollectionName;
            using (HubEntities db = new HubEntities())
            {
                var currentCollection = (from collection in db.NewsReleaseCollections orderby collection.Name descending select collection).FirstOrDefault();
                currentCollectionName = "news_releases_" + currentCollection.Name;
            }

            foreach (DirectoryInfo di in directory.GetDirectories())
            {
                foreach (FileInfo fi in GetFilesRecursive(di))
                {
                    if (!fi.FullName.Contains(currentCollectionName))
                        continue;

                    yield return fi;
                }
            }

            foreach (FileInfo fi in directory.GetFiles())
                yield return fi;
        }

        #endregion

        public static void PublishInMediaGallery(HubEntities db, NewsRelease nr, string publishLocation)
        {
            Console.WriteLine("Publishing Media Gallery...");

            //Do not add to RSS if body is empty
            if (nr.Documents.Any(d => d.Languages.Any(dl => dl.BodyHtml == string.Empty)))
                return;

            bool isMediaAdvisory = nr.ReleaseType == ReleaseType.Advisory;
            string logDescription = isMediaAdvisory ? "" : " BC Gov News";
            if (nr.IsPublishNewsOnDemand())
            {
                logDescription += " and News On Demand";
            }
            if (nr.IsPublishMediaContacts())
            {
                if (!string.IsNullOrEmpty(logDescription)) logDescription += " and";
                logDescription += " Media Distribution Lists";
            }
            Utils.LogMessage("Publishing NR " + nr.Key + " to" + logDescription);

            if (!nr.ReleaseDateTime.HasValue)
            {
                nr.ReleaseDateTime = nr.PublishDateTime.Value.ToLocalTime().DateTime;

                nr.Logs.Add(new NewsReleaseLog()
                {
                    DateTime = nr.PublishDateTime.Value,
                    Description = "Released for Publishing" /* NewsReleaseLog.Description */
                });

                nr.Logs.Add(new NewsReleaseLog()
                {
                    DateTime = DateTimeOffset.Now,
                    Description = "Published to" + logDescription
                });
            }
            else
            {
                nr.Logs.Add(new NewsReleaseLog()
                {
                    DateTime = DateTimeOffset.Now,
                    Description = "Republished to" + logDescription
                });
            }


            //. Create Files
            // Generate the publish files
            string publishFolder = Path.Combine(publishLocation, string.Format("news_releases_{0}", nr.Collection.Name));

            Console.WriteLine("Publishing NR " + nr.Key + " (Generating files)...");

            if (nr.ReleaseType == ReleaseType.Release)
            {
                ConvertNewsRelease.GenerateFiles(db, nr, publishFolder);
            }

            //TODO: Consider Logging to History

            // d.	On Success set FTP bit in NewsRelease table to true
            //Database.UpdateFileTransfer((int)nrRecord["ID"], true);

            //. Update Database

            string strNrNumber = nr.Key;

            DateTime dtRelease = nr.ReleaseDateTime.Value;

            bool bUpdateDatabase = false;

            //TODO: Check if all the news releases have been properly published
            bUpdateDatabase = true;

            if (bUpdateDatabase)
            {
                //NewsReleaseHistory history = new NewsReleaseHistory();
                //nr.History.Add(history);

                //Database.AddHistory(strNrNumber, "Published", "");
                //Common.LogMessage("    " + strNrNumber + ": UpdateDatabase");

                // Set the State to published and move the document to "Published" folder
                System.Diagnostics.Debug.Assert(!nr.IsPublished);
                nr.IsPublished = true;
                nr.Timestamp = DateTimeOffset.Now;
            }
        }

        public static void UnpublishMediaGallery(HubEntities db, string publishLocation)
        {
            Console.WriteLine("Unpublishing Media Gallery...");

            var inPublish = (from nr in db.NewsReleases where !nr.IsCommitted && nr.IsPublished select nr).ToArray();

            foreach (var nr in inPublish)
            {
                Utils.LogMessage("Unpublishing NR " + nr.Key + "...");

                string publishSubfolder = Path.Combine(publishLocation, string.Format("news_releases_{0}", nr.Collection.Name));

                Utils.UnpublishFiles(db, nr, publishSubfolder);

                //TODO: Check for unpublishing from News Feeds, Newsroom, and News On Demand

                NewsReleaseLog log = new NewsReleaseLog();
                log.DateTime = DateTimeOffset.Now;
                log.Description = "Unpublished from BC Gov News" + (nr.PublishOptions.HasFlag(PublishOptions.PublishNewsOnDemand) ? " and News On Demand" : ""); /* NewsReleaseLog.Description */
                nr.Logs.Add(log);

                nr.IsPublished = false;
                nr.Timestamp = DateTimeOffset.Now;
            }
            db.SaveChanges();
        }
    }
}