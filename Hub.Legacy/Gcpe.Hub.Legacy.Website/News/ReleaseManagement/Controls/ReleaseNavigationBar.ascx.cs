extern alias legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gcpe.Hub.Properties;
using legacy::Gcpe.Hub.Data.Entity;

namespace Gcpe.Hub.News.ReleaseManagement.Controls
{
    public partial class ReleaseNavigationBar : System.Web.UI.UserControl
    {
        public ReleaseModel Model;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                string param = Request["__EVENTARGUMENT"]; // parameter
                if (param != null && Request["__EVENTTARGET"].IndexOf("DocumentLink") != -1)
                {
                    string[] indices = param.Split('-');
                    Model.ReorderDocuments(Int32.Parse(indices[0]) - 1, Int32.Parse(indices[1]) - 1);
                    Page.DataBind(); // to update the page with the new order
                }
            }
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(btnPrint);
        }

        protected void rptDocumentLanguage_DataBound(object sender, RepeaterItemEventArgs e)
        {
            var item = (ListItem)e.Item.DataItem;
            LinkButton lnk = (LinkButton)e.Item.FindControl("DocumentLink");
            lnk.Attributes["href"] = "#Documents-" + item.Value.Replace("/", @"-");
            lnk.Text = item.Text;
        }

        protected IEnumerable<System.Web.UI.WebControls.ListItem> DocumentLanguages(Guid documentId)
        {
            var documents = new List<System.Web.UI.WebControls.ListItem>();

            Model.AddDocumentLanguages(documentId, documents);

            return documents;
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            byte[] pdfData = Model.GetPortableDocument();

            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=New-" + DateTime.UtcNow.Ticks.ToString() + ".pdf");
            Response.BinaryWrite(pdfData);

            Context.ApplicationInstance.CompleteRequest();
            //Response.End();
        }

        public string GetReleaseUrl()
        {
            string folder = (string)Page.RouteData.Values["Folder"];
            string reference = (string)Page.RouteData.Values["Reference"];
            string id = (string)Page.RouteData.Values["Id"];

            return ResolveUrl("~/News/ReleaseManagement/" + folder + "/" + (reference ?? id));
        }

        public string GetNewDocumentUrl()
        {
            //TODO: Do not use .Count()
            return GetReleaseUrl() + "/" + (Model.DocumentCount + 1).ToString();
        }

        protected void BtnScheduleEmail_Click(object sender, EventArgs e)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

            message.From = new System.Net.Mail.MailAddress(Page.User.Identity.GetEmailAddress(), Page.User.Identity.GetDisplayName());
            message.To.Add(message.From);

            message.Subject = (string.IsNullOrEmpty(Model.Reference) || Model.LeadOrganization == "" ? "DRAFT: " : "") + Model.FirstHeadline;
            //message.Subject = "News Release Item for " + (string.IsNullOrEmpty(Model.Reference) ? "Draft" : Model.Reference);

            message.IsBodyHtml = true;

            //string fileName = (string.IsNullOrEmpty() ? "Draft_" + Model.DraftReference : Model.Reference) + "_" + DateTimeOffset.Now.ToString("yyyyMMddhhMMss") + ".pdf";
            //string fileName = (string.IsNullOrEmpty(Model.Reference) || Model.LeadOrganization == "" ? "DRAFT" : (Model.Reference == "" ? "NEWS-" + Model.ReleaseUri.AbsolutePath.Split('/').Last() : Model.Reference)) + ".pdf";
            string fileName = (string.IsNullOrEmpty(Model.Reference) || Model.LeadOrganization == "" ? "DRAFT" : (Model.ReleaseTypeId == ReleaseType.Release ? Model.Key : Model.Reference)) + ".pdf";

            string body = "";
            body += "Please refer to the files attached to this email. The following is the summary of the News Release" + "\r\n";
            body += "\r\n";

            bool isMediaAdvisory = Model.ReleaseTypeId == ReleaseType.Advisory;
            if (!isMediaAdvisory && !string.IsNullOrEmpty(Model.Reference))
            {
                body += "Permalink: https://" + Request.Url.Host.ToLower().Replace("hub.gcpe", "news").Replace("localhost", "dev.news.gov.bc.ca") + "/" + Model.Reference.Substring("NEWS-".Length) + "\r\n";
            }

            body += "\r\n";
            //body += "Document Name: " + fileName + "\r\n";

            bool isRelease = Model.ReleaseTypeId == ReleaseType.Release;
            if (string.IsNullOrEmpty(Model.Reference) || Model.LeadOrganization == null)
            {
                body += isRelease ? "NR Number: Not Approved" : "News ID: Not Approved";
            }
            else if (!isMediaAdvisory)
            {
                body += "News ID: " + Model.Reference;
                if (isRelease)
                {
                    body += "\r\nNR Number: " + Model.Key;
                }
            }

            body += "\r\n";
            body += "NR Type: " + Model.Documents.First().Text + "\r\n";
            body += "State: " + Model.ReleaseStatus + "\r\n";

            if (Model.ReleaseDate.HasValue)
            {
                //TODO: Use Release Date instead of Publish Date
                body += "Release Date: " + Model.ReleaseDate.Value.ToString("MMMM d, yyyy") + "\t\r\n"; // \t to prevent Outlook from ignoring the newline. Bruno
            }
            else
            {
                if (Model.PublishDateTime.HasValue)
                {
                    body += Model.IsCommitted ? "Release Date: " : "Planned Release Date: ";
                    body += Model.PublishDateTime.Value.ToString("MMMM d, yyyy") + " at " + Model.PublishDateTime.Value.ToString("h:mm tt").ToLower() + "\t\r\n";
                }
                else
                {
                    body += "No Release Date\r\n";
                }

                //body += Model.IsCommitted ? "Release Date: " : "Planned Release Date: ";
                //body += (Model.PublishDateTime.HasValue ? Model.PublishDateTime.Value.ToString("MMMM d, yyyy") + " at " + Model.PublishDateTime.Value.ToString("h:mm tt").ToLower() : "Not Scheduled") + "\r\n";
            }

            string mediaDistributionListStr = String.Empty;
            foreach (var item in Model.MediaDistributionLists)
            {
                if (item.Selected)
                {
                    if (!string.IsNullOrEmpty(mediaDistributionListStr))
                    {
                        mediaDistributionListStr += ", ";
                    }
                    mediaDistributionListStr += item.Text;
                }
            }
            if (!string.IsNullOrEmpty(mediaDistributionListStr))
            {
                body += "Media Distribution Lists: " + mediaDistributionListStr + "\t\r\n"; // http://stackoverflow.com/questions/247546/outlook-autocleaning-my-line-breaks-and-screwing-up-my-email-format
            }


            if (string.IsNullOrEmpty(Model.LeadOrganization))
            {
                body += "No Lead Organization" + "\r\n";
            }
            else
            {
                body += "Lead Organization: " + (string.IsNullOrEmpty(Model.LeadOrganization) ? "Not Approved" : Model.LeadOrganization) + "\r\n";
            }

            body += "Headline: " + Model.FirstHeadline + "\r\n";

            body += "" + "\r\n";
            body += "This email was auto-generated." + "\r\n";
            body += "" + "\r\n";

            var textDoc = System.Text.Encoding.UTF8.GetString(Model.GetTextDocument());
            // if we have a media advisory of type "event reminder", remove the duplicated title from the body
            if (Model.ReleaseTypeId == ReleaseType.Advisory && textDoc.IndexOf("MEDIA ADVISORY - EVENT REMINDER") > -1)
            {
                var idxOfMediaAdvisoryType = textDoc.IndexOf("MEDIA ADVISORY - EVENT REMINDER");
                var mediaAdvisoryType = textDoc.Substring(idxOfMediaAdvisoryType);
                var idxOfNewline = mediaAdvisoryType.IndexOf("\r\n\r\n");
                var haystack = mediaAdvisoryType.Substring(0, idxOfNewline);
                var needle = haystack.Substring(haystack.IndexOf("\r\n"));
                body += textDoc.Replace(needle, "");
            }
            else
            {
                body += textDoc;
            }

            message.Body = $"<pre><div style=\"font-family: \'BCSans\', \'Noto Sans\', Verdana, Arial, sans-serif; font-size:14px;\">{body}</div></pre>";
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            byte[] pdf = Model.GetPortableDocument();

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(pdf))
            {
                message.Attachments.Add(new System.Net.Mail.Attachment(stream, fileName));

                using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(Settings.Default.SMTPServer))
                    client.Send(message);
            }
        }

        protected void lbtnAdd_Click(object sender, EventArgs e)
        {
            // This will not be saved to the db yet. Just a place holder until the user fills it up and click Save
            Model.AddDocumentLanguagePlaceHolder(Model.DocumentCount, Language.enCA);
            Page.DataBind();
            // Scroll to the newly added document. Timeout is required for this to work correctly.
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "ScrollToNewDocument", "setTimeout(function() { ScrollToId('#Documents-" + Model.DocumentCount + "');}, 1000);", true);
            //ScriptManager.RegisterStartupScript(Page, this.GetType(), "ScrollToNewDocument", "ScrollToClass(document-" + Model.DocumentCount + ");", true);
        }

        //public string ReleaseStatusHtml
        //{
        //    get
        //    {
        //        string format = "<span class=\"state\">{0}</span> {1} {2} {3}";

        //        if (Model.PublishDateTime.HasValue)
        //        {
        //            if (Model.IsPublished)
        //                return string.Format(format, "Published", "on", Model.PublishDateTime.Value.ToString("MMM d, yyyy hh:mm"), Model.PublishDateTime.Value.ToString("tt").ToLower());
        //            else if (!Model.IsPublished && Model.IsCommitted)
        //                return string.Format(format, "Scheduled", "for", Model.PublishDateTime.Value.ToString("MMM d, yyyy hh:mm"), Model.PublishDateTime.Value.ToString("tt").ToLower());
        //            else if ((!Model.IsPublished && !Model.IsCommitted) && Model.PublishDateTime.HasValue)
        //                return string.Format(format, "Planned", "for", Model.PublishDateTime.Value.ToString("MMM d, yyyy hh:mm"), Model.PublishDateTime.Value.ToString("tt").ToLower());
        //            else
        //                throw new NotImplementedException();
        //        }
        //        else
        //        {
        //            return "No publish date";
        //        }
        //    }
        //}
    }
}
