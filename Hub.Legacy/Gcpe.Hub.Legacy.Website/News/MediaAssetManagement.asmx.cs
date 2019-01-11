using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Gcpe.Hub.News
{
    /// <summary>
    /// Summary description for MediaAssetManagement
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    [System.ComponentModel.ToolboxItem(false)]
    public class MediaAssetManagement : WebService
    {
        const string FileAttributesSession = "FileClientAttributes";
        const string StaticFileKey = "staticFiles";
        const int BytesPerKb = 1024;

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object PrepareMetadata(string key, string path, int blocksCount, string fileName, long fileSize, long chunkSize)
        {
            if (key == StaticFileKey)
            {
#if !LOCAL_MEDIA_STORAGE
                return PrepareMetadataResponse("files", null, blocksCount, fileName, fileSize, chunkSize, true);
#else
                try
                {
                    string directory = Properties.Settings.Default.NewsFileFolder;

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                }
                catch (Exception e)
                {
                    return new { error = true, message = "Error occurred when preparing uploading a file." + e.Message };
                }

                string uploadFolder = Properties.Settings.Default.NewsFileFolder;
                string folder = null;

                return PrepareMetadataResponse(uploadFolder, folder, blocksCount, fileName, fileSize, chunkSize, false);
#endif
            }
            else
            {
                string folder = path + "\\" + key;
                return PrepareMetadataResponse("assets", folder, blocksCount, fileName, fileSize, chunkSize, true);
            }
        }

        private object PrepareMetadataResponse(string uploadFolder, string folder, int blocksCount, string fileName, long fileSize, long chuckSize, bool useBlobStorage)
        {
            var fileToUpload = new FileUploadModel()
            {
                UseBlobStorage = useBlobStorage,
                FolderName = folder,
                BlockCount = blocksCount,
                FileName = fileName,
                FileSize = fileSize,
                UploadFolder = uploadFolder,
                StartTime = DateTime.Now,
                IsUploadCompleted = false,
                ChunkSize = chuckSize,
                UploadStatusMessage = string.Empty
            };

            HttpContext.Current.Session.Add(FileAttributesSession, fileToUpload);
            HttpContext.Current.Response.AddHeader("X-UA-Compatible", "IE=edge");

            return new { error = false };
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UploadBlock()
        {
            string stringid = HttpContext.Current.Request.Params.Get("id");
            int id = int.Parse(stringid);

            Context.Response.AddHeader("X-UA-Compatible", "IE=edge");
            Context.Response.ContentType = "application/json";

            JavaScriptSerializer json = new JavaScriptSerializer();

            dynamic response;

            if (HttpContext.Current.Session[FileAttributesSession] == null)
            {
                response = new { error = true, isLastBlock = false, message = "File could not be uploaded because session has expired. Please upload the file again." };
            }
            else
            {
                HttpPostedFile uploadedData = Context.Request.Files["Slice"];

                var model = (FileUploadModel)Session[FileAttributesSession];

                if (model.UseBlobStorage)
                {
                    response = UploadBlockToBlobStorage(id, uploadedData, model);
                }
                else
                {
                    response = UploadBlockToFileSystem(id, uploadedData, model);
                }
            }

            if (response.error == true)
            {
                HttpContext.Current.Session[FileAttributesSession] = null;
            }

            if (response.isLastBlock == true)
            {
                HttpContext.Current.Session.Clear();
            }

            Context.Response.Write(json.Serialize(response));
        }

        private object UploadBlockToFileSystem(int id, HttpPostedFile uploadedData, FileUploadModel model)
        {
            System.Diagnostics.Debug.Assert(model.FolderName == null);

            string filePath = Path.Combine(model.UploadFolder, model.FileName);
            string tempfileFolderPath = Path.Combine(model.UploadFolder, Path.GetFileNameWithoutExtension(filePath));

            var tempfilePath = string.Format("{0}/{1}.part{2}", tempfileFolderPath, "temp", id.ToString("0000"));
            bool saveFileError = false;
            string errorMsg = "";

            try
            {
                if (!Directory.Exists(tempfileFolderPath))
                    Directory.CreateDirectory(tempfileFolderPath);

                uploadedData.SaveAs(tempfilePath);
            }
            catch (PathTooLongException)
            {
                saveFileError = true;
                errorMsg = "File could not be uploaded because server could not save the file. Please use a shorter file name and try again.";
            }
            catch (Exception e)
            {
                saveFileError = true;
                errorMsg = "File could not be uploaded. " + e.Message;
            }

            if (saveFileError)
            {
                try
                {
                    Directory.Delete(tempfileFolderPath);
                }
                catch (Exception)
                {
                    //do nothing
                }

                return new { error = true, isLastBlock = false, message = errorMsg };
            }

            if (id == model.BlockCount)
            {
                //Check if all chunks are ready and save file
                var files = Directory.GetFiles(tempfileFolderPath);

                float fileSizeInKb = model.FileSize / BytesPerKb;
                bool errorInOperation = false;

                if ((files.Length + 1) * model.ChunkSize >= fileSizeInKb)
                {
                    try
                    {
                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            foreach (string file in files.OrderBy(x => x))
                            {
                                var buffer = File.ReadAllBytes(file);
                                fs.Write(buffer, 0, buffer.Length);
                                File.Delete(file);
                            }
                        }

                        model.IsUploadCompleted = true;
                        model.UploadStatusMessage = model.FileName + " has been uploaded successfully.";
                    }
                    catch (IOException ioe)
                    {
                        model.UploadStatusMessage = "Error occurred when saving the file. \n\r " + ioe.Message;
                        errorInOperation = true;
                    }

                    try
                    {
                        if (!Directory.GetFiles(tempfileFolderPath).Any())
                            Directory.Delete(tempfileFolderPath);
                    }
                    catch (Exception)
                    {
                        //do nothing
                    }

                    //return all files
                    string directory = model.UploadFolder;

                    files = Directory.GetFiles(directory);
                    List<object> fileList = new List<object>();

                    foreach (var file in files)
                    {
                        string fname = Path.GetFileName(file);
                        fileList.Add(new { fileName = fname });
                    }

                    return new { error = errorInOperation, isLastBlock = model.IsUploadCompleted, message = model.UploadStatusMessage, files = fileList };
                }
            }

            return new { error = false, isLastBlock = false, message = string.Empty };
        }

        private object UploadBlockToBlobStorage(int id, HttpPostedFile uploadedData, FileUploadModel model)
        {
            if (uploadedData.ContentLength > 4 * 1024 * 1024)
                throw new ArgumentOutOfRangeException(nameof(uploadedData));

#if !LOCAL_MEDIA_STORAGE
            var containerUri = Global.ModifyContainerWithSharedAccessSignature(model.UploadFolder);

            var container = new CloudBlobContainer(containerUri);

            string folderName = string.IsNullOrEmpty(model.FolderName) ? "" : model.FolderName.Replace("\\", "/").TrimEnd('/') + "/";

            CloudBlockBlob blob = container.GetBlockBlobReference((folderName + model.FileName).ToLower());

            //Upload chunk to block
            {                
                //TODO: Consider validating MD5 of file on the client.
                //http://stackoverflow.com/questions/768268/how-to-calculate-md5-hash-of-a-file-using-javascript

                blob.PutBlock($"{id:X8}", uploadedData.InputStream, null);

                if (id < model.BlockCount)
                {
                    //Return to the client; there are more chunks to process
                    return new { error = false, isLastBlock = false, message = string.Empty };
                }
            }
            //Assembly chunks in to single file
            {
                var blockListIds = new List<string>();

                for (int i = 1; i <= model.BlockCount; i++)
                {
                    blockListIds.Add($"{i:X8}");
                }

                blob.PutBlockList(blockListIds);
                blob.Metadata["filename"] = model.FileName;
                blob.SetMetadata();

                model.UploadStatusMessage = model.FileName + " has been uploaded successfully.";
                return new { error = false, isLastBlock = true, message = model.UploadStatusMessage };
            }
#else
            string filePath = Path.Combine(model.UploadFolder, model.FolderName, model.FileName);
            string tempfileFolderPath = Path.Combine(model.UploadFolder, model.FolderName, Path.GetFileNameWithoutExtension(filePath));

            var tempfilePath = string.Format("{0}/{1}.part{2}", tempfileFolderPath, "temp", id.ToString("0000"));
            bool saveFileError = false;
            string errorMsg = "";

            try
            {
                if (!Directory.Exists(tempfileFolderPath))
                    Directory.CreateDirectory(tempfileFolderPath);

                uploadedData.SaveAs(tempfilePath);
            }
            catch (PathTooLongException)
            {
                saveFileError = true;
                errorMsg = "File could not be uploaded because server could not save the file. Please use a shorter file name and try again.";
            }
            catch (Exception e)
            {
                saveFileError = true;
                errorMsg = "File could not be uploaded. " + e.Message;
            }

            if (saveFileError)
            {
                try
                {
                    Directory.Delete(tempfileFolderPath);
                }
                catch (Exception)
                {
                    //do nothing
                }

                return new { error = true, isLastBlock = false, message = errorMsg };
            }

            if (id == model.BlockCount)
            {
                //Check if all chunks are ready and save file
                var files = Directory.GetFiles(tempfileFolderPath);

                float fileSizeInKb = model.FileSize / BytesPerKb;
                bool errorInOperation = false;

                if ((files.Length + 1) * model.ChunkSize >= fileSizeInKb)
                {
                    try
                    {
                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            foreach (string file in files.OrderBy(x => x))
                            {
                                var buffer = File.ReadAllBytes(file);
                                fs.Write(buffer, 0, buffer.Length);
                                File.Delete(file);
                            }
                        }

                        model.IsUploadCompleted = true;

                        Global.QueueBackgroundWorkItemWithRetry(() =>
                        {
                            foreach (string folder in Properties.Settings.Default.DeployFolders)
                            {
                                string deployDirectory = Path.Combine(folder, model.FolderName);

                                if (!Directory.Exists(deployDirectory))
                                    Directory.CreateDirectory(deployDirectory);

                                File.Copy(filePath, Path.Combine(deployDirectory, model.FileName), true);
                            }
                        });

                        model.UploadStatusMessage = model.FileName + " has been uploaded successfully.";

                    }
                    catch (IOException ioe)
                    {
                        model.UploadStatusMessage = "Error occurred when saving the file. \n\r " + ioe.Message;
                        errorInOperation = true;
                    }

                    try
                    {
                        if (!Directory.GetFiles(tempfileFolderPath).Any())
                            Directory.Delete(tempfileFolderPath);
                    }
                    catch (Exception)
                    {
                        //do nothing
                    }

                    //return all files
                    string directory = Path.Combine(model.UploadFolder, model.FolderName);

                    files = Directory.GetFiles(directory);
                    List<object> fileList = new List<object>();

                    foreach (var file in files)
                    {
                        string fname = Path.GetFileName(file);
                        fileList.Add(new { fileName = fname });
                    }

                    return new { error = errorInOperation, isLastBlock = model.IsUploadCompleted, message = model.UploadStatusMessage, files = fileList };
                }
            }

            return new { error = false, isLastBlock = false, message = string.Empty };
#endif
        }
    }
}