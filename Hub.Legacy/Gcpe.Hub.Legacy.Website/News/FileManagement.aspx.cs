using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace Gcpe.Hub.News
{
    public partial class FileManagement : Hub.News.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Hub.News.Site site = Master;
            site.MenuText = "BC Gov News";
            site.MenuNavigateUrl = "~/News/FileManagement";
            site.AddAppItem("BC Gov Corporate Calendar", "~/Calendar");
            site.AddAppItem("News Release Management", "~/News/ReleaseManagement/Drafts");

            site.AddNavigationItem("Files", "~/News/FileManagement");
            site.AddNavigationItem("Carousel", "~/News/Carousel");
            site.AddNavigationItem("Live Feed", "~/News/LiveFeedManagement");
            site.AddNavigationItem("Project Granville", "~/News/ProjectGranvilleManagement");
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!GetStaticFiles().Any())
            {
                rptAssetList.Visible = false;
                NoFileMessage.Visible = true;
                btnDelete.Visible = false;
            }
        }

        protected void btnSaveFiles_Click(object sender, EventArgs e)
        {
            try
            {
#if !LOCAL_MEDIA_STORAGE
                var container = new CloudBlobContainer(Global.ModifyContainerWithSharedAccessSignature("files"));

                foreach (RepeaterItem item in rptAssetList.Items)
                {
                    Label label = (Label)item.FindControl("file");
                    HiddenField valTopOrFeature = (HiddenField)item.FindControl("valDeleted");
                    if (valTopOrFeature.Value.ToLower() == "true")
                    {
                        var file = container.GetBlobReference(label.Text.ToLower());
                        if (!file.Exists())
                            file = container.GetBlobReference(label.Text); // for backwards compatibility

                        if (file.Exists())
                            file.Delete();
                    }
                }
#else
                string directory = Settings.Default.NewsFileFolder;
                if (Directory.Exists(directory))
                {
               
                        foreach (RepeaterItem item in rptAssetList.Items)
                        {
                            Label label = (Label)item.FindControl("file");
                            HiddenField valTopOrFeature = (HiddenField)item.FindControl("valDeleted");
                            if (valTopOrFeature.Value.ToLower() == "true")
                            {
                                if (File.Exists(Path.Combine(directory, label.Text)))
                                {
                                    File.Delete(Path.Combine(directory, label.Text));
                                }

                            }
                        }
                }
#endif
            }
            catch (Exception fde)
            {
                List<string> errorMessages = new List<string>();
                errorMessages.Add(fde.Message);
                DisplayErrors(errorMessages);

            }
            rptAssetList.DataBind();
        }


        public IEnumerable<string> GetStaticFiles()
        {
            List<string> mediaAssets = new List<string>();

#if !LOCAL_MEDIA_STORAGE
            var container = new CloudBlobContainer(Global.ModifyContainerWithSharedAccessSignature("files"));

            foreach (CloudBlockBlob blob in container.ListBlobs(null, false, BlobListingDetails.Metadata, null, null).OfType<CloudBlockBlob>())
            {
                string fileName;
                mediaAssets.Add(blob.Metadata.TryGetValue("filename", out fileName) ? fileName : Path.GetFileName(blob.Name));
            }
#else
            if (!String.IsNullOrEmpty(Settings.Default.NewsFileFolder))
            {
                string directory = Settings.Default.NewsFileFolder;

                if (Directory.Exists(directory))
                {
                    foreach (string file in Directory.GetFiles(directory))
                    {
                        mediaAssets.Add(Path.GetFileName(file));
                    }
                }
            }
#endif
            return mediaAssets;
        }

        private void DisplayErrors(IEnumerable<string> errorMessages)
        {
            pnlErrors.Visible = true;
            rptErrors.DataSource = errorMessages;
            rptErrors.DataBind();
        }
    }
}