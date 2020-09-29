extern alias legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gcpe.Hub.News.ReleaseManagement;
using Gcpe.Hub.Properties;
using Gcpe.Hub.Services.Legacy;
using Gcpe.News.ReleaseManagement;
using legacy::Gcpe.Hub.Data.Entity;

namespace Gcpe.Hub
{
    static public class ReleasePublisher
    {
        private static FlickrManager flickrManager = new FlickrManager();
        private static DateTimeOffset NextNewsReleasePublishDate = DateTime.MinValue;
        public static void UpdateNextNewsReleasePublishDate(NewsRelease release)
        {
            var publishDateTime = MaxReleaseDate(release);
            if (publishDateTime.HasValue && NextNewsReleasePublishDate > publishDateTime.Value)
            {
                // Wake up the NewsReleasePublish Task. It will handle retries if the NOD api is down
                NextNewsReleasePublishDate = publishDateTime.Value;
            }
        }

        // return Max of PublishDateTime and ReleaseDateTime
        public static DateTimeOffset? MaxReleaseDate(NewsRelease nr)
        {
            return nr.ReleaseDateTime > nr.PublishDateTime ? nr.ReleaseDateTime : nr.PublishDateTime;
        }

        private static void AppendNewPost(NewsRelease post, List<Hub.Services.Legacy.Models.Article> posts)
        {
            NewsReleaseDocumentLanguage firstEnglish = post.Documents.OrderBy(d => d.SortIndex).First().English();

            try
            {
                string postUrl = ReleaseModel.GetPermanentUri(post);

                if (post.IsPublishNewsOnDemand() || post.IsPublishMediaContacts())
                {
                    List<string> distributionLists = post.IsPublishNewsOnDemand() ? ReleaseManagementModel.SelectedNodDistributionLists(post) : new List<string>();
                    string textContent = null;
                    if (post.IsPublishMediaContacts())
                    {
                        textContent = Gcpe.News.ReleaseManagement.Templates.Release.FromEntity(post).ToTextDocumentAsString();
                        textContent = "<p>" + textContent.Replace("\r", "").Replace("\n\n", "</p><p>").Replace("\n", "<br/>") + "</p>";
                        distributionLists.AddRange(ReleaseManagementModel.SelectedMediaDistributionLists(post));
                    }

                    posts.Add(new Hub.Services.Legacy.Models.Article
                    {
                        DistributionLists = distributionLists,
                        Title = firstEnglish.Headline,
                        Uri = postUrl,
                        Id = post.GetAtomId(),
                        Published = post.ReleaseDateTime ?? post.PublishDateTime.Value,
                        HtmlContent = post.English().Summary,
                        TextContent = textContent
                    });
                }
            }
            catch (Exception ex)
            {
                Utils.LogError(ex);
            }
        }

        public static void NewsReleasePublishTask(CancellationToken applicationShutdownToken)
        {
            while (true)
            {
                try
                {
                    using (var client = new SubscribeClient(Settings.Default.SubscribeBaseUri))
                    {
                        if (NextNewsReleasePublishDate <= DateTimeOffset.Now)
                        {
                            NextNewsReleasePublishDate = PublishNewsReleases(client, applicationShutdownToken);
                        }
                        bool cancelled = applicationShutdownToken.WaitHandle.WaitOne(60000); // sleep for 60 seconds
                        if (cancelled) return;
                        client.Distribution.TryRunningDailyDigest(); // every minute
                    }
                }
                catch (Exception ex)
                {
                    Utils.LogError(ex);
                    bool cancelled = applicationShutdownToken.WaitHandle.WaitOne(60000); // sleep for 60 seconds
                    if (cancelled) return;
                }
            }
        }

        private static DateTimeOffset PublishNewsReleases(ISubscribeClient client, CancellationToken applicationShutdownToken)
        {
            var articles = new List<Hub.Services.Legacy.Models.Article>();
            DateTimeOffset nextCheckTime = DateTimeOffset.MaxValue;
            using (HubEntities db = new HubEntities())
            {
                IList<NewsRelease> releasesToBePublished = db.NewsReleases.Where(nr => nr.IsActive && nr.IsCommitted && !nr.IsPublished)
                                                                         .OrderBy(r => r.PublishDateTime).ToList();

                //Global.LogError("scheduledReleasesOrForPublishing: " + scheduledReleasesOrForPublishing.Count().ToString());

                var now = DateTimeOffset.Now;
                IList<string> releaseUris = new List<string>();
                foreach (NewsRelease post in releasesToBePublished)
                {
                    if (!string.IsNullOrWhiteSpace(post.AssetUrl))
                    {
                        var postUri = new Uri(post.AssetUrl);
                        try
                        {
                            var publishDate = post.PublishDateTime.Value;
                            
                            if (postUri.Host.Contains("staticflickr.com"))
                            {
                                var assetUri = AssetEmbedManager.NormalizeFlickrUri(new Uri(post.AssetUrl));
                                post.AssetUrl = assetUri.ToString();
                            }
                            else if (publishDateIsNow(publishDate, now) && (post.AssetUrl.Contains("flickr") || post.AssetUrl.Contains("flic.kr")))
                            {
                                var photoId = new Uri(post.AssetUrl).Segments[3].TrimEnd('/');
                                if (flickrManager.FlickrAssetExists(photoId) == true)
                                {
                                    if (!flickrManager.IsAssetPublic(photoId))
                                        flickrManager.SetFlickrAssetPermissionsToPublic(photoId);

                                    var assetUri = AssetEmbedManager.NormalizeFlickrUri(new Uri(post.AssetUrl));
                                    post.AssetUrl = assetUri.ToString();
                                }
                                else
                                {
                                    // the flickr asset has been deleted so we will no longer store a reference to it
                                    post.AssetUrl = string.Empty;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Utils.LogError("An error occured when publishing a post with a Flickr asset", e);
                            flickrManager.SendErrorNotification(post.Reference, post.AssetUrl);
                            post.AssetUrl = string.Empty;
                        }
                    }

                    var postDateTime = MaxReleaseDate(post);
                    if (postDateTime > now)
                    {
                        if (nextCheckTime > postDateTime)
                        {
                            nextCheckTime = postDateTime.Value;
                        }
                        break;
                    }
                    PublishManager.PublishInMediaGallery(db, post, Settings.Default.PublishLocation); // because of new ReportViewer() in ToPortableDocument(), trying to do any await call in this file will causes a deadlock!
                    // we're going to tell Facebook to scrape these afterwards
                    releaseUris.Add(new Uri(Settings.Default.NewsHostUri, post.ReleasePathName() + "/" + post.Key).ToString());
                    if (!articles.Any(e => e.Id == post.AtomId))
                    {
                        AppendNewPost(post, articles);
                    }
                }
                if (releaseUris.Count != 0)
                {
                    db.SaveChanges();
                    Utils.LogMessage("Published");
                    client.Distribution.AddNewsOnDemandPostsAsync(articles, applicationShutdownToken).GetAwaiter().GetResult();
                    PublishManager.DeployFiles(Settings.Default.PublishLocation, Settings.Default.DeployLocations.Split('|')); // this will throw if a deployPath is down
                }
            }
            return nextCheckTime;
        }

        private static bool publishDateIsNow(DateTimeOffset publishDate, DateTimeOffset now)
        {
            if (publishDate == null)
                return false;

            // remove the seconds
            now = now.AddSeconds(-now.Second);
            publishDate = publishDate.AddSeconds(-now.Second);

            // remove the milliseconds
            now = now.AddMilliseconds(-now.Millisecond);
            publishDate = publishDate.AddMilliseconds(-now.Millisecond);

            return (publishDate.Date == now.Date) && (publishDate.Hour == now.Hour && publishDate.Minute == now.Minute);
        }

        public static void UnpublishNewsRelease(string permanentUri)
        {
            using (var client = new SubscribeClient(Settings.Default.SubscribeBaseUri))
            {
                client.Distribution.RemoveNewsOnDemandEntry(permanentUri);
                using (HubEntities db = new HubEntities())
                {
                    PublishManager.UnpublishMediaGallery(db, Settings.Default.PublishLocation);
                }
            }
        }

        public static int? NewsReleaseSubscriberCount(IList<string> distributionLists)
        {
            if (distributionLists.Count == 0) return null;
            using (var client = new SubscribeClient(Settings.Default.SubscribeBaseUri))
            {
                return client.Distribution.NewsOnDemandSubscriberCount(distributionLists);
            }
        }
    }
}