extern alias legacy;

using System;
using System.Web;

namespace Gcpe.Hub.News.ReleaseManagement
{
    using legacy::Gcpe.Hub.Data.Entity;

    public class ReleaseHistoryFile : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string mimeType = (string)context.Request["MimeType"];
            Guid blobId = HubModel.DecodeGuid(context.Request["BlobId"]);

            using (HubEntities db = new HubEntities())
            {
                var blob = db.Blobs.Find(blobId);

                context.Response.ContentType = mimeType;
                context.Response.BinaryWrite(blob.Data);
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