using FlickrNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gcpe.Hub
{
    public class FlickrManager
    {
        private static readonly Flickr flickrClient = new Flickr();
        private static readonly string flickrPrivateBaseUrl = Gcpe.Hub.Configuration.App.Settings.FlickrPrivateBaseUri;

        public bool AuthenticateWithFlickr()
        {
            flickrClient.ApiKey = Gcpe.Hub.Configuration.App.Settings.FlickrApiKey;
            flickrClient.ApiSecret = Gcpe.Hub.Configuration.App.Settings.FlickrApiSecret;
            flickrClient.OAuthGetAccessToken(new OAuthRequestToken
            {
                Token = Gcpe.Hub.Configuration.App.Settings.FlickrApiToken,
                TokenSecret = Gcpe.Hub.Configuration.App.Settings.FlickrApiTokenSecret
            },
                Gcpe.Hub.Configuration.App.Settings.FlickrApiVerifier);
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