using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gcpe.Hub
{
    public partial class Admin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public Uri GetAzureFilesContainer()
        {
            return Global.ReadContainerWithSharedAccessSignature("files", DateTimeOffset.Now.Date.AddYears(1));
        }

        public Uri GetAzureAssetsContainer()
        {
            return Global.ListContainerWithSharedAccessSignature("assets", DateTimeOffset.Now.Date.AddYears(1));
        }
    }
}