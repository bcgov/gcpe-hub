using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Gcpe.Hub.News.ReleaseManagement
{
    public static class AssetEmbedManager
    {
        private static readonly Regex AssetRegex = new Regex("<asset>(?<url>[^<]+)</asset>");
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly FlickrManager flickrManager = new FlickrManager();
        private const int TIMEOUT = 3000; //in ms

        enum AssetType { Flickr, SoundCloud, Youtube }

        private class AssetInfo
        {
            public string Url { get; private set; }

            public string NormalTag { get; set; }

            public AssetInfo(string url)
            {
                this.Url = url;
            }

            public string GetAssetOriginalTag()
            {
                return string.Format("<asset>{0}</asset>", Url);
            }
        }

        public static Uri NormalizeYoutubeUri(Uri uri)
        {
            if (uri.Host == "www.youtube.com")
            {
                //TODO: Clean up YouTube default URL format
                return new Uri(uri.ToString().Replace("http://", "https://"));
            }
            else if (uri.Host.Contains("youtu.be"))
            {
                string videoId = uri.AbsolutePath.TrimStart('/');

                return new Uri("https://www.youtube.com/watch?v=" + videoId);
            }

            throw new NotImplementedException();
        }

        public static Uri NormalizeFlickrUri(Uri uri)
        {
            if (uri.Host.Contains("staticflickr.com"))
            {
                //TODO: Clean up default Flickr format
                return new Uri(uri.ToString().Replace("http://", "https://"));
            }
            else if (uri.Host.Contains("flickr.com") || uri.Host == "flic.kr")
            {
                using (var client = new System.Net.WebClient())
                {
                    var photoId = uri.Segments[3].TrimEnd('/');
                    flickrManager.SetFlickrAssetPermissionsToPublic(photoId);

                    try
                    {
                        var result = client.DownloadString(string.Format("http://www.flickr.com/services/oembed?url={0}", uri));

                        using (var xmlReader = XmlReader.Create(new StringReader(result)))
                        {
                            while (xmlReader.ReadToFollowing("url"))
                            {
                                return new Uri(xmlReader.ReadElementContentAsString());
                            }
                        }
                    }
                    catch (System.Net.WebException wEx)
                    {
                        if (wEx.Message == "Response status code does not indicate success: 404 (Not Found).")
                        {
                            return null;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            throw new NotImplementedException();
        }

        private static async Task<AssetInfo> NormalizeAssetUrlAsync(AssetInfo assetInfo)
        {
            var FormatAssetTag = new Func<Uri, string>(normalUri => { return "<asset>" + normalUri.ToString() + "</asset>"; });

            Uri uri;
            if (Uri.TryCreate(assetInfo.Url, UriKind.Absolute, out uri))
            {
                if (uri.Host.Contains("staticflickr.com"))
                {
                    //TODO: Clean up YouTube default Flickr format
                    assetInfo.NormalTag = FormatAssetTag(uri);
                }
                else if (uri.Host.Contains("flickr.com") || uri.Host == "flic.kr")
                {
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        try
                        {
                            var result = await client.GetStreamAsync(string.Format("http://www.flickr.com/services/oembed?url={0}", uri)).ConfigureAwait(false);

                            using (var xmlReader = XmlReader.Create(new StreamReader(result), new XmlReaderSettings() { Async = true }))
                            {
                                while (xmlReader.ReadToFollowing("url"))
                                {
                                    assetInfo.NormalTag = FormatAssetTag(new Uri(await xmlReader.ReadElementContentAsStringAsync().ConfigureAwait(false)));
                                }
                            }

                        }
                        catch (System.Net.Http.HttpRequestException hrEx)
                        {
                            if (hrEx.Message == "Response status code does not indicate success: 404 (Not Found).")
                            {
                                //Asset is not found in Flickr oembed service
                                assetInfo.NormalTag = "<a href=\"" + uri + "\">" + uri + "</a>";
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
                else if (uri.Host.Contains("w.soundcloud.com"))
                {
                    //TODO: Clean up YouTube default SoundCloud format
                    assetInfo.NormalTag = FormatAssetTag(uri);
                }
                else if (uri.Host.Contains("soundcloud.com"))
                {
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        var result = await client.GetStreamAsync(string.Format("http://soundcloud.com/oembed?url={0}", uri)).ConfigureAwait(false);

                        using (var xmlReader = XmlReader.Create(new StreamReader(result), new XmlReaderSettings() { Async = true }))
                        {
                            while (xmlReader.ReadToFollowing("html"))
                            {
                                var src = await xmlReader.ReadElementContentAsStringAsync().ConfigureAwait(false);

                                assetInfo.NormalTag = FormatAssetTag(new Uri(HttpUtility.HtmlEncode(Regex.Match(src, "(?<=src=\")[^\"]+", RegexOptions.IgnoreCase).Value.Replace("visual=true&", ""))));
                            }
                        }
                    }
                }
                else if (uri.Host == "www.youtube.com")
                {
                    //TODO: Clean up YouTube default URL format
                    assetInfo.NormalTag = FormatAssetTag(uri);
                }
                else if (uri.Host.Contains("youtu.be"))
                {
                    string videoId = uri.AbsolutePath.TrimStart('/');

                    assetInfo.NormalTag = FormatAssetTag(new Uri("https://www.youtube.com/watch?v=" + videoId));
                }
                else if (uri.Host.Contains("facebook.com"))
                {
                    assetInfo.NormalTag = FormatAssetTag(uri);
                }
                else
                {
                    //Asset is an unrecognized format; convert to HTML link.
                    assetInfo.NormalTag = "<a href=\"" + uri + "\">" + uri + "</a>";
                }
            }
            else
            {
                //assetInfo.NormalTag = "<!--" + assetInfo.Url + "-->";
                assetInfo.NormalTag = "<a href=\"" + uri + "\">" + uri + "</a>";
            }

            return assetInfo;
        }

        public static string NormalizeAssetTags(string tagHtml)
        {
            var urlInfos = new List<AssetInfo>();

            var matches = AssetRegex.Matches(tagHtml);

            foreach (Match m in matches)
            {
                string url = m.Groups["url"].Value;
                urlInfos.Add(new AssetInfo(url));
            }

            var readyInfoTasks = urlInfos.Select(i => NormalizeAssetUrlAsync(i)).ToArray();

            Task.WaitAll(readyInfoTasks); //This is the sync point of all async tasks

            foreach (var t in readyInfoTasks.Where(t => t.IsCompleted))
                tagHtml = tagHtml.Replace(t.Result.GetAssetOriginalTag(), t.Result.NormalTag);

            return tagHtml;
        }


        public static string RenderAssetsInHtml(string bodyHtml)
        {
            var newhtml = AssetRegex.Replace(bodyHtml, new MatchEvaluator(match =>
            {
                string url = match.Groups["url"].Value;

                try
                {
                    Uri uri = new Uri(url);

                    if (uri.Host == "www.youtube.com")
                    {
                        string videoId = null;
                        if (uri.Host.Contains("youtube"))
                        {
                            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

                            videoId = query["v"];
                        }

                        var youtubeIframeUrl = string.Format("//www.youtube.com/embed/{0}?rel=0&amp;modestbranding=1", videoId);

                        return string.Format(
                                                "<div class=\"asset-preview\">" +
                                                "<iframe src=\"{0}\" frameborder=\"0\" allowfullscreen ></iframe>" +
                                                "</div>"
                                                , youtubeIframeUrl);

                    }
                    else if (uri.Host.EndsWith("staticflickr.com"))
                    {
                        return string.Format(
                                                "<div class=\"asset-preview\">" +
                                                "<img src=\"{0}\"/>" +
                                                "</div>"
                                                , url);
                    }
                    else if (uri.Host == "w.soundcloud.com")
                    {
                        //var url1=  HttpUtility.HtmlDecode(url);
                        return string.Format(
                                               "<div class=\"asset-soundcloud-preview\">" +
                                               "<iframe src=\"{0}\" frameborder=\"0\" allowfullscreen ></iframe>" +
                                               "</div>"
                                               , HttpUtility.HtmlDecode(url));
                    }
                    else if (uri.Host == "www.facebook.com")
                    {
                        return string.Format(
                                              "<div class=\"asset-facebook-preview\">" +
                                                 "<div class=\"fb-post\" data-href=\"{0}\"></div>" +
                                              "</div>"
                                              , url);
                    }
                }
                catch (UriFormatException)
                {
                    return "<!--" + url + "-->";
                }

                return "<a href=\"" + url + "\">" + url + "</a>";
            }));

            return newhtml;
        }
    }
}