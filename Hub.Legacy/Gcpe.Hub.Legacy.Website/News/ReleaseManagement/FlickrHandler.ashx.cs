using Gcpe.News.ReleaseManagement;
using System;
using System.Web;

namespace Gcpe.Hub
{
    /// <summary>
    /// Summary description for FlickrHandler
    /// </summary>
    public class FlickrHandler : IHttpHandler
    {
        FlickrManager flickrManager = new FlickrManager();

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string photoId = context.Request.Form["photoId"];
                if (!string.IsNullOrWhiteSpace(photoId) && flickrManager.FlickrAssetExists(photoId))
                    flickrManager.SetFlickrAssetPermissionsToPrivate(photoId);
            }
            catch (Exception e)
            {
                Utils.LogError("An error occured setting a Flickr asset to private", e);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}