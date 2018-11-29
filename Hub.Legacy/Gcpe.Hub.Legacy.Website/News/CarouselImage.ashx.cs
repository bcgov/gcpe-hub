extern alias legacy;

using System;
using System.Linq;
using System.Web;
using legacy::Gcpe.Hub.Data.Entity;

namespace Gcpe.Hub.News
{
    /// <summary>
    /// Serves the browser a CarouselImage
    /// </summary>
    public class CarouselImage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid slideId;
            if (!Guid.TryParse(context.Request.QueryString["SlideId"], out slideId))
            {
                context.Response.StatusCode = 404;
                return;
            }
            using (var hubEntities = new HubEntities())
            {
                Slide slide = hubEntities.Slides.FirstOrDefault(s => s.Id == slideId);
                DateTime slideTimeStamp = slide.Timestamp.UtcDateTime.AddTicks(- (slide.Timestamp.Ticks % TimeSpan.TicksPerSecond)); // truncate to seconds
                context.Response.Cache.SetMaxAge(TimeSpan.Zero); // always check ifModifiedSince so that changing an image is reflected right away
                context.Response.Cache.SetLastModified(slideTimeStamp);

                DateTime ifModifiedSince;
                if (DateTime.TryParse(context.Request.Headers.Get("If-Modified-Since"), out ifModifiedSince)
                 && ifModifiedSince.ToUniversalTime() >= slideTimeStamp)
                {
                    // The requested image has not changed
                    context.Response.StatusCode = 304;
                    return;
                }

                context.Response.ContentType = slide.Image[0] == 0xFF ? "image/jpeg" : "image/png";
                context.Response.OutputStream.Write(slide.Image, 0, slide.Image.Length);
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}