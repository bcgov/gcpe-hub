using System;
using FlickrNet;
using Gcpe.Hub.Properties;

namespace Gcpe.Hub
{
    public class FlickrManager
    {
        private static readonly Flickr flickrClient = new Flickr();
        private static readonly Uri flickrPrivateBaseUrl = Settings.Default.FlickrPrivateBaseUri;

        public bool AuthenticateWithFlickr()
        {
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
            try
            {
                if (AuthenticateWithFlickr() == true)
                {
                    flickrClient.PhotosGetInfo(photoId);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("invalid"))
                    return false;
            }

            return true;
        }

        public void SetFlickrAssetPermissionsToPrivate(string photoId)
        {
            if (AuthenticateWithFlickr() == true)
            {
                flickrClient.PhotosSetPerms(
                photoId,
                false,
                false,
                false,
                PermissionComment.Nobody,
                PermissionAddMeta.Owner);
            }
        }


        public void SetFlickrAssetPermissionsToPublic(string photoId)
        {
            if (AuthenticateWithFlickr() == true)
            {
                flickrClient.PhotosSetPerms(
                    photoId,
                    true,
                    false,
                    false,
                    PermissionComment.Nobody,
                    PermissionAddMeta.Owner);
            }
        }

        public string ConstructPrivateAssetUrl(string photoId)
        {
            return $"{flickrPrivateBaseUrl}{photoId}";
        }

    }
}