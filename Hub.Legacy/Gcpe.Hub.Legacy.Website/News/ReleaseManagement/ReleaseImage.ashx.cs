extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gcpe.Hub.News.ReleaseManagement
{
    using legacy::Gcpe.Hub.Data.Entity;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    /// <summary>
    /// Summary description for ReleaseImage
    /// </summary>
    public class ReleaseImage : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            Guid id;
            if (Guid.TryParse(context.Request["Id"], out id))
            {
                using (HubEntities db = new HubEntities())
                {
                    NewsReleaseImage newsImage = db.NewsReleaseImages.Find(id);

                    int height;
                    if (int.TryParse(context.Request["Height"], out height))
                    {
                        using (System.IO.MemoryStream dataStream = new System.IO.MemoryStream(newsImage.Blob.Data))
                        {
                            using (var image = Bitmap.FromStream(dataStream))
                            {
                                using (Bitmap scaleImage = new Bitmap(image.Width * height / image.Height, height))
                                {
                                    using (Graphics g = Graphics.FromImage(scaleImage))
                                    {
                                        g.SmoothingMode = SmoothingMode.HighQuality;
                                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                                        g.DrawImage(image, new Rectangle(0, 0, scaleImage.Width, scaleImage.Height));
                                    }

                                    using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                                    {
                                        scaleImage.Save(stream, image.RawFormat);

                                        context.Response.ContentType = newsImage.MimeType;
                                        context.Response.BinaryWrite(stream.ToArray());
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        context.Response.ContentType = newsImage.MimeType;
                        context.Response.BinaryWrite(newsImage.Blob.Data);
                    }
                }
            }
            else
            {
                context.Response.ContentType = "image/png";
                context.Response.WriteFile(context.Server.MapPath("~/News/Images/noimage.png"));
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