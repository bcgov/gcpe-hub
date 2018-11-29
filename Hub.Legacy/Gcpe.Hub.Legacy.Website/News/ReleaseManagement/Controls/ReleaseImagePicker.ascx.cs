extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gcpe.Hub.News.ReleaseManagement.Controls
{
    public partial class ReleaseImagePicker : System.Web.UI.UserControl
    {
        public bool IsReadOnly { get; set; }

        public Guid? Value
        {
            get
            {
                return hdPageImage.Value == "" ? (Guid?)null : Guid.Parse(hdPageImage.Value);
            }
            set
            {
                hdPageImage.Value = value.ToString();
            }
        }

        public string SelectedImageUrl
        {
            get
            {
                return GetReleaseImageClientUrl(Value);
            }
        }
        public int LanguageId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                Value = string.IsNullOrEmpty(hdPageImage.Value) ? (Guid?)null : Guid.Parse(hdPageImage.Value);
            }
        }

        public Dictionary<Guid, string> Images
        {
            get
            {
                using (var db = new legacy::Gcpe.Hub.Data.Entity.HubEntities())
                {
                    var releaseImages = from ri in db.NewsReleaseImages
                                        where ri.Languages.Any(l => l.LanguageId == LanguageId) 
                                        //TODO: where ri.IsActive
                                        orderby ri.SortOrder, ri.Name
                                        select ri;
                    
                    return releaseImages.ToDictionary(ri => ri.Id, ri => ri.Name);
                }
            }
        }

        int imageHeight;

        public int ReleaseImageHeight
        {
            get
            {
                return imageHeight;
            }
            set
            {
                imageHeight = value;
            }
        }

        public string GetReleaseImageClientUrl(Guid? imageId = null)
        {
            return GetReleaseImageClientUrl(Page, ReleaseImageHeight, imageId);
        }

        //TODO: Make instance (not static) method
        public static string GetReleaseImageClientUrl(System.Web.UI.Page page, int imageHeight, Guid? imageId = null)
        {
            return page.ResolveUrl("~/News/ReleaseManagement/ReleaseImage.ashx") + "?Height=" + imageHeight.ToString() + (imageId.HasValue ? "&Id=" + imageId.Value.ToString() : "");
        }

        public string ValueClientID
        {
            get
            {
                return hdPageImage.ClientID;
            }
        }

        public string ImageClientID
        {
            get
            {
                return imgSelected.ClientID;
            }
        }
    }
}