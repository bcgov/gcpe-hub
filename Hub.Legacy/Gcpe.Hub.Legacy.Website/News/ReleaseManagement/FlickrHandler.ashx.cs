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
            string photoId = context.Request.Form["photoId"];
            if(!string.IsNullOrWhiteSpace(photoId) && flickrManager.FlickrAssetExists(photoId))
                flickrManager.SetFlickrAssetPermissionsToPrivate(photoId);
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