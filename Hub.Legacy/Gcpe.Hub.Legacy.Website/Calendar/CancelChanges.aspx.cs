using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CancelChanges : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Uri url = Request.UrlReferrer;

        if (url != null) Response.Redirect(url.PathAndQuery);
    }
}