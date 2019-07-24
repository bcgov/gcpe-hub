using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FlickrNet;
using Gcpe.Hub.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gcpe.Hub
{
    public class FlickrManager
    {
        private static readonly Flickr flickrClient = new Flickr();
        private static readonly Uri flickrPrivateBaseUrl = Settings.Default.FlickrPrivateBaseUri;
        private static HttpClient client = new HttpClient();

        public bool AuthenticateWithFlickr()
        {
            if (ShouldCancelRequestDueToServiceOutage())
                throw new InvalidOperationException("The Flickr services that NRMS depends on are experiencing technical issues. " +
                    "Please check back later when Flickr services are online. You can check Flickr status at https://status.flickr.net/");

            flickrClient.ApiKey = Settings.Default.FlickrApiKey;
            flickrClient.ApiSecret = Settings.Default.FlickrApiSecret;
            flickrClient.OAuthGetAccessToken(new OAuthRequestToken
            {
                Token = Settings.Default.FlickrApiToken,
                TokenSecret = Settings.Default.FlickrApiTokenSecret
            },
                Settings.Default.FlickrApiVerifier);
            flickrClient.InstanceCacheDisabled = true;

            var foundUser = flickrClient.TestLogin();
            return foundUser != null;
        }

        public bool FlickrAssetExists(string photoId)
        {
            if (ShouldCancelRequestDueToServiceOutage())
                throw new InvalidOperationException("The Flickr services that NRMS depends on are experiencing technical issues. " +
                    "Please check back later when Flickr services are online. You can check Flickr status at https://status.flickr.net/");

            if (AuthenticateWithFlickr() != true)
                return false;

            var photoInfo = flickrClient.PhotosGetInfo(photoId);
            return photoInfo != null;
        }

        public void SetFlickrAssetPermissionsToPrivate(string photoId)
        {
            if (ShouldCancelRequestDueToServiceOutage())
                throw new InvalidOperationException("The Flickr services that NRMS depends on are experiencing technical issues. " +
                    "Please check back later when Flickr services are online. You can check Flickr status at https://status.flickr.net/");

            if (AuthenticateWithFlickr() != true)
                return;

            flickrClient.PhotosSetPerms(
                photoId,
                false,
                false,
                false,
                PermissionComment.Nobody,
                PermissionAddMeta.Owner);
        }


        public void SetFlickrAssetPermissionsToPublic(string photoId)
        {
            if (ShouldCancelRequestDueToServiceOutage())
                throw new InvalidOperationException("The Flickr services that NRMS depends on are experiencing technical issues. " +
                    "Please check back later when Flickr services are online. You can check Flickr status at https://status.flickr.net/");

            if (AuthenticateWithFlickr() != true)
                return;

            flickrClient.PhotosSetPerms(
                    photoId,
                    true,
                    false,
                    false,
                    PermissionComment.Nobody,
                    PermissionAddMeta.Owner);
        }

        public string ConstructPrivateAssetUrl(string photoId)
        {
            return $"{flickrPrivateBaseUrl}{photoId}";
        }

        public bool ShouldCancelRequestDueToServiceOutage()
        {
            HttpResponseMessage response = client.GetAsync("https://9htz5wc2q8lk.statuspage.io/api/v2/components.json").Result;
            if (response.IsSuccessStatusCode)
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                var deserializedResult = JsonConvert.DeserializeObject<Root>(jsonString);

                // we're interested in the API, Login functionality and uploads
                // if any of these components are in a state other than "operational" (i.e. degraded_performance, partial_outage, or major_outage) we cancel any requests to Flickr
                // in order not to interfere with the publishing of releases
                return deserializedResult.Components
                    .Any(c => c.Name == "API" && c.Status != "operational"
                            || c.Name == "Login" && c.Status != "operational"
                            || c.Name == "Uploads" && c.Status != "operational"
                            || c.Name == "Photo/Video Serving" && c.Status != "operational");
            }

            return false;
        }

        // data structure for the root json object returned from the Flickr status API
        private class Root
        {
            public IEnumerable<Component> Components { get; set; }
        }

        // data structure for the services (components) provided by Flickr
        private class Component
        {
            public string Name { get; set; }
            public string Status { get; set; }
        }

        public void SendErrorNotification(string reference, string assetUrl)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.From = new System.Net.Mail.MailAddress(Settings.Default.FlickrErrorNotificationContactEmail);
            message.To.Add(message.From);

            message.Subject = "A BC Gov News entry needs your attention when Flickr services are back online:";
            message.IsBodyHtml = true;

            // figure out which environment we're in depending on which news app we're publishing to
            var env = new string(Settings.Default.NewsHostUri.Authority.Take(3).ToArray());
            string baseUrl = "https://hub.gcpe.gov.bc.ca/Legacy/News/ReleaseManagement/Published/";

            if (env == "dev" || env == "test")
            {
                baseUrl = $"https://{env}.hub.gcpe.gov.bc.ca/Legacy/News/ReleaseManagement/Published/";
            }

            string body = $"<p>Please add the following asset -- {assetUrl} -- to <a href='{baseUrl}{reference}'>{reference}</a>.</p>";
            body += "<p>You can check Flickr's status at <a href='https://status.flickr.net'>https://status.flickr.net</a></p>";
            message.Body = body;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(Settings.Default.SMTPServer))
            {
                client.Send(message);
            }

        }

    }
}