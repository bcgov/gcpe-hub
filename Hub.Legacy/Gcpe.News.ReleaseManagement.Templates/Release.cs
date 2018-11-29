extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Gcpe.News.ReleaseManagement.Templates
{
    using Microsoft.Reporting.WinForms;
    using legacy::Gcpe.Hub.Data.Entity;
    using HtmlAgilityPack;
    using System.Web;

    public class Release
    {
        const string LocationSeparator = " – ";

        private readonly List<Document> documents;
        public List<Document> Documents
        {
            get { return documents; }
        }

        public bool IsReleased { get; set; }

        public DateTime ReleaseDateTime { get; set; }

        private string MetaLocation { get; set; }

        private string MetaSectors { get; set; }

        private string MetaOrganization { get; set; }

        public Release()
        {
            documents = new List<Document>();
        }

        public static Release FromEntity(legacy::Gcpe.Hub.Data.Entity.NewsRelease nr)
        {
            Release release = new Release();

            //TODO: Determine if we should use nr.IsPublished or nr.PublishDateTime.HasValue to determine IsReleased value
            release.IsReleased = !string.IsNullOrEmpty(nr.Reference);

            //TODO: Do not use DateTime.Today: Change to System.DateTime

            release.ReleaseDateTime = nr.ReleaseDateTime ?? (nr.PublishDateTime.HasValue ? nr.PublishDateTime.Value.DateTime : (DateTime?)null) ?? DateTime.Today;
            //Meta content values are comma separated strings. A specific single value cannot contain commas
            //TODO: Strip commas from the query string used in the News.Gov search
            release.MetaLocation = nr.English().Location.Replace(",", string.Empty);

            release.MetaSectors = string.Join(",", nr.Sectors.Select(s => s.Id));

            release.MetaOrganization = nr.Ministry == null ? "" : nr.Ministry.Id.ToString();

            foreach (var nrLanguage in nr.Languages.OrderBy(e => e.Language.SortOrder).ThenBy(e => e.Language.Name))
            {
                foreach (var nd in nr.Documents.OrderBy(e => e.SortIndex))
                {
                    foreach (var ndLanguage in nd.Languages.Where(e => e.Language.Id == nrLanguage.Language.Id))
                    {
                        var document = new Templates.Document();

                        //TODO: document Guid
                        //document.DocumentId = nd.Id;
                        document.DocumentId = Guid.NewGuid();

                        document.PageLayout = (int)nd.PageLayout;

                        if (nrLanguage.Language.Id == 4105)
                            document.LanguageCode = "en-CA";
                        else if (nrLanguage.Language.Id == 3084)
                            document.LanguageCode = "fr-CA";
                        else
                            throw new NotImplementedException();

                        if (nr.ReleaseType != ReleaseType.Advisory)
                        {
                            if (nr.Reference == "" || nr.Ministry == null)
                                document.ReferenceNumber = "Not Approved"; //release.ReleaseDateTime.ToString("yyyy") + "DRAFT";
                            else
                                document.ReferenceNumber = nr.ReleaseType == ReleaseType.Release ? nr.Key : nr.Reference;
                        }

                        document.PageTitle = nd.Language(nrLanguage.Language.Id).PageTitle;
                        document.Headline = nd.Language(nrLanguage.Language.Id).Headline;

                        document.SubheadlineHtml = nd.Language(nrLanguage.Language.Id).Subheadline;

                        if (document.LanguageCode == "en-CA" && nd.Languages.Count > 1)
                        {
                            document.SubheadlineHtml += (document.SubheadlineHtml == "" ? "" : "<br />") + "(disponible en français en bas de page)";
                        }

                        document.BylineHtml = nd.Language(nrLanguage.Language.Id).Byline.Replace("\r", "").Replace("\n", "<br />");

                        string location;

                        if (nd.SortIndex == 0)
                            location = string.IsNullOrEmpty(nr.Language(ndLanguage.Language.Id).Location) ? nr.English().Location : nr.Language(ndLanguage.Language.Id).Location;
                        else
                            location = "";

                        string bodyHtml = nd.Language(nrLanguage.Language.Id).BodyHtml;

                        document.BodyHtml = MergeLocationBody(location, bodyHtml);

                        var AssetRegex = new System.Text.RegularExpressions.Regex("<asset>(?<url>[^<]+)</asset>");

                        document.BodyHtml = AssetRegex.Replace(document.BodyHtml, new System.Text.RegularExpressions.MatchEvaluator(match =>
                        {
                            //return "<div><a href=\"" + match.Groups["url"].Value + "\">" + match.Groups["url"].Value + "</a></div>";
                            return "";
                        }));

                        document.BodyHtml = document.BodyHtml.Replace("<p>&nbsp;</p>", "");
                        document.BodyHtml = document.BodyHtml.Replace("<p></p>", "");

                        document.BodyWithoutLocationHtml = bodyHtml;

                        if (nd.PageLayout == PageLayout.Informal)
                        {
                            string byline = "&nbsp;<br />" + (string.IsNullOrEmpty(ndLanguage.Byline) ? "" : "<b>" + ndLanguage.Byline.Replace("\r", "").Replace("\n", "<br />") + "</b>");

                            //Removing the dates from getting added with they ByLine. This date shows up twice.
                            if (ndLanguage.LanguageId == 4105)
                                byline += (byline == "" ? "" : "<br /><br />");// + CPDateFormat(release.ReleaseDateTime);
                            else if (ndLanguage.LanguageId == 3084)
                                byline += (byline == "" ? "" : "<br /><br />");// + release.ReleaseDateTime.ToString("d MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo(3084)) + "<br /><br />";
                            else
                                throw new NotImplementedException();

                            document.BodyHtml = byline + "<br /><br />" + document.BodyHtml;
                        }

                        document.Organizations = nd.Language(nrLanguage.Language.Id).Organizations;

                        //Find the logo image which the language is same as document or bilingual
                        if (ndLanguage.PageImage != null)
                        {
                            NewsReleaseImageLanguage imageLanguage = ndLanguage.PageImage.Languages.Single(e => e.Language.Id == ndLanguage.Language.Id);

                            document.PageImage = ndLanguage.PageImage.Blob.Data;
                            document.PageImageName = ndLanguage.PageImage.Name;
                            document.PageImageAlternateName = imageLanguage.AlternateName;
                        }

                        foreach (var contact in ndLanguage.Contacts.OrderBy(e => e.SortIndex))
                        {
                            //TODO: Replace other HTML escaped characters.
                            document.AddContact(HttpUtility.HtmlEncode(contact.Information).Replace("\r", "").Replace("\n", "<br />"));
                        }

                        document.Release = release;
                        release.Documents.Add(document);
                    }
                }
            }

            return release;
        }

        public static string CPDateFormat(DateTimeOffset dateTime)
        {
            return dateTime.ToString("MMM. d, yyyy").Replace("Mar.", "March").Replace("Apr.", "April").Replace("May.", "May").Replace("Jun.", "June").Replace("Jul.", "July").Replace("Sep.", "Sept.");
        }

        public static string CPDateTimeFormat(DateTimeOffset dateTime)
        {
            return dateTime.ToString("MMM. d, yyyy h:mm tt").Replace("Mar.", "March").Replace("Apr.", "April").Replace("May.", "May").Replace("Jun.", "June").Replace("Jul.", "July").Replace("Sep.", "Sept.")
                .Replace("AM", "a.m.").Replace("PM", "p.m.");
        }

        public static string MergeLocationBody(string location, string bodyHtml)
        {
            if (string.IsNullOrWhiteSpace(location))
                return bodyHtml;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(bodyHtml);

            foreach (var node in doc.DocumentNode.ChildNodes)
            {
                if (node.Name == "p")
                {
                    foreach (var child in node.ChildNodes)
                    {
                        if (child.Name == "asset")
                            continue;

                        HtmlNode locationNode = HtmlTextNode.CreateNode(location.ToUpper() + LocationSeparator);
                        node.InsertBefore(locationNode, child);
                        return doc.DocumentNode.OuterHtml;

                        //if (child.Name == "#text")
                        //{
                        //    if (child.InnerText.Replace("&nbsp;", " ").Trim() != "")
                        //    {
                        //        HtmlNode locationNode = HtmlTextNode.CreateNode(location.ToUpper() + LocationSeparator);

                        //        node.InsertBefore(locationNode, child);

                        //        return doc.DocumentNode.OuterHtml;
                        //    }
                        //}
                    }
                }
                else if ((node.Name == "ul" || node.Name == "ol") && node.InnerText.Replace("&nbsp;", " ").Trim() != "")
                {
                    return bodyHtml;
                }
            }

            return bodyHtml;
        }

        public byte[] ToTextDocument()
        {
            string package = ToTextDocumentAsString() + "\r\n\r\n\r\nConnect with the Province of B.C. at: http://news.gov.bc.ca/connect";
            return System.Text.Encoding.UTF8.GetBytes(package);
        }

        public string ToTextDocumentAsString()
        {
            string releaseTemplate, documentTemplate;

            using (System.IO.StreamReader reader = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Gcpe.News.ReleaseManagement.Templates.Resources.ReleaseText.txt")))
                releaseTemplate = reader.ReadToEnd();

            using (System.IO.StreamReader reader = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Gcpe.News.ReleaseManagement.Templates.Resources.DocumentText.txt")))
                documentTemplate = reader.ReadToEnd();

            string formattedDocuments = "";
            foreach (Document document in Documents)
            {
                string formattedContacts = "";
                if (document.ContactsCount > 0)
                {
                    if (document.LanguageCode == "en-CA")
                    {
                        formattedContacts += document.ContactsCount == 1 ? "Contact:" : "Contacts:"; /*Comm. Contacts*/
                    }
                    else if (document.LanguageCode == "fr-CA")
                    {
                        formattedContacts += "Renseignements additionnels:";
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    foreach (var contact in document.Contacts)
                    {
                        formattedContacts += "\r\n\r\n" + contact.PrimaryContactText;
                        string secondaryContactText = contact.SecondaryContactText;
                        if (!string.IsNullOrEmpty(secondaryContactText)) formattedContacts += "\r\n\r\n" + contact.SecondaryContactText;
                    }
                }

                //Some funky manipulation to for a special formatting case for Media Advisory's only.
                string headline = document.PageTitle;
                headline = (headline == "Media Advisory") ? headline.ToUpper() : headline.ToUpper() + "\r\n" + document.Headline;
                formattedDocuments += "\r\n\r\n" + documentTemplate
                    .Replace("@Document.Headline", headline)
                    .Replace("@Document.Subheadline", document.SubheadlineHtml.Replace("<br />", "\r\n")) //*BodyText already includes byline* + (string.IsNullOrEmpty(document.BylineHtml) ? "" : "\r\n\r\n" + document.BylineHtml.Replace("<br />", "\r\n")))
                    .Replace("Format(@Document.BodyText)", document.BodyText)
                    .Replace("Format(@Contacts)", formattedContacts);
                //.Replace("Format(@Document.BodyText)", string.Join("\r\n  ", System.Text.RegularExpressions.Regex.Split(document.BodyText, "\r\n")))
                //.Replace("Format(@Contacts)", string.Join("\r\n  ", System.Text.RegularExpressions.Regex.Split(formattedContacts, "\r\n")));
            }

            string package = releaseTemplate
                .Replace("@Documents.First().ReferenceNumber", (IsReleased ? "For Immediate Release" + "\r\n" : "") + documents.First().ReferenceNumber)
                .Replace("@Release.ReleaseDateTime.ToString(\"MMMM d, yyyy\")", CPDateFormat(ReleaseDateTime))
                .Replace("@Documents.First().Organizations", documents.First().Organizations)
                .Replace("Format(@Documents)", formattedDocuments);

            while (package.Contains("\r\n\r\n\r\n") || package.Contains("\r\n  \r\n"))
                package = package.Replace("\r\n\r\n\r\n", "\r\n\r\n").Replace("\r\n  \r\n", "\r\n\r\n");

            // Replace: smart single quotes and apostrophe, smart double quotes, ellipsis, dashes, circumflex, open and closed angle brackets, and spaces
            package = System.Text.RegularExpressions.Regex.Replace(package, "[\u2018\u2019\u201A]", "'");
            package = System.Text.RegularExpressions.Regex.Replace(package, "[\u201C\u201D\u201E]", "\"");
            package = System.Text.RegularExpressions.Regex.Replace(package, "[\u2026]", "...");

            //Common dashes (https://en.wikipedia.org/wiki/Dash)
            package = System.Text.RegularExpressions.Regex.Replace(package, "[\u2012\u2013\u2014\u2015]", "-");
            package = System.Text.RegularExpressions.Regex.Replace(package, "[\u2053]", "~");

            //Similar Unicode characters
            package = System.Text.RegularExpressions.Regex.Replace(package, "[\u005F\u02CD]", "_");
            package = System.Text.RegularExpressions.Regex.Replace(package, "[\u002D\u00AD\u00AF\u02C9\u02D7\u2010\u2011\u203E\u2043\u207B\u208B\u2212\u23AF\u23E4\u2500\u2796\u2E3A\u2E3B\u1019]", "-");
            package = System.Text.RegularExpressions.Regex.Replace(package, "[\u007E\u02DC\u223C]", "~");

            package = System.Text.RegularExpressions.Regex.Replace(package, "[\u02C6]", "^");
            package = System.Text.RegularExpressions.Regex.Replace(package, "[\u2039]", "<");
            package = System.Text.RegularExpressions.Regex.Replace(package, "[\u203A]", ">");
            package = System.Text.RegularExpressions.Regex.Replace(package, "[\u02DC\u00A0]", " ");

            return package;
        }

        public Dictionary<string, byte[]> ToHtmlDocuments()
        {
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

            string releaseTemplate, documentTemplate;

            using (System.IO.StreamReader reader = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Gcpe.News.ReleaseManagement.Templates.Resources.ReleaseHtml.html")))
                releaseTemplate = reader.ReadToEnd();

            using (System.IO.StreamReader reader = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Gcpe.News.ReleaseManagement.Templates.Resources.DocumentHtml.html")))
                documentTemplate = reader.ReadToEnd();

            string formattedDocuments = "";
            foreach (Document document in Documents)
            {
                string formattedContacts = "";
                if (document.ContactsCount > 0)
                {
                    //style=""display: none;""
                    formattedContacts += @"<tr><td colspan=""2"">&nbsp;<br /><b>";

                    if (document.LanguageCode == "en-CA")
                        formattedContacts += document.ContactsCount == 1 ? "Contact:" : "Contacts:"; /*Comm. Contacts*/
                    else if (document.LanguageCode == "fr-CA")
                        formattedContacts += "Renseignements additionnels:";
                    else
                        throw new NotImplementedException();

                    formattedContacts += "</b><br />&nbsp;</td></tr>";

                    foreach (var contact in document.Contacts)
                        formattedContacts += "<tr><td style=\"width:50%; vertical-align:top;\">" + contact.PrimaryContactHtml + "<br />&nbsp;</td><td style=\"width:50%; vertical-align:top;\">" + contact.SecondaryContactHtml + "<br />&nbsp;</td></tr>";
                }

                formattedDocuments += documentTemplate
                    .Replace("@PageTitle", document.PageTitle.ToUpper())
                    .Replace("@TopLeftHeader", document.TopLeftHeader)
                    .Replace("@TopRightHeader", document.TopRightHeader)
                    .Replace("@MiddleHeader", document.MiddleHeader)
                    //.Replace("@BottomHeader", document.BottomHeader)
                    //.Replace("@Document.PageTitle.ToUpper()", document.PageTitle.ToUpper())
                    //.Replace("@Document.Headline", document.Headline)
                    //.Replace("@Document.Subheadline", document.Subheadline)
                    .Replace("@Body", document.BodyHtml)
                    .Replace("Format(@MediaContacts)", formattedContacts);

                if (string.IsNullOrEmpty(document.PageImageName))
                {
                    formattedDocuments = formattedDocuments.Replace("Format(@PageImage)", "");
                }
                else
                {
                    formattedDocuments = formattedDocuments.Replace("Format(@PageImage)", "<img src='" + document.PageImageName + ".jpg' alt='" + document.PageImageAlternateName + "' height='70' width='700' />");
                }
            }

            string package = releaseTemplate
                .Replace("@Documents.First().Reference", documents.First().ReferenceNumber)
                .Replace("@Documents.First().Headline", documents.First().Headline)

                //Use meta tags with dates in the ISO-8601 format (YYYY-MM-DD) to avoid the confusion caused by multiple dates and multiple formats in the title or text of the documents.
                //https://developers.google.com/search-appliance/documentation/50/help_gsa/crawl_sort
                .Replace("@MetaDcDate", ReleaseDateTime.ToString("yyyy-MM-dd"))

                .Replace("@MetaOrganization", MetaOrganization)
                .Replace("@MetaSectors", MetaSectors)
                .Replace("@MetaLocation", MetaLocation)
                .Replace("Format(@Documents)", formattedDocuments);

            //while (package.Contains("\r\n\r\n\r\n") || package.Contains("\r\n  \r\n"))
            //    package = package.Replace("\r\n\r\n\r\n", "\r\n\r\n").Replace("\r\n  \r\n", "\r\n\r\n");
            //return System.Text.Encoding.UTF8.GetBytes(package);

            files.Add(Documents.First().ReferenceNumber + ".htm", System.Text.Encoding.UTF8.GetBytes(package));
            return files;
        }

        public string ToNewsroomHtml()
        {
            string html = "";

            int index = 0;
            foreach (var document in documents)
            {
                //TODO: Consult Business Area re: Formatting of additional documents

                if (index > 0)
                {
                    html += "<p>";
                    html += "<b>" + HttpUtility.HtmlEncode(document.PageTitle.ToUpper()) + "</b>";
                    html += "</p>";
                }

                if (index > 0 || document.LanguageCode != "en-CA")
                {
                    if (document.Headline != "")
                    {
                        html += "<p>";

                        html += "<b>" + HttpUtility.HtmlEncode(document.Headline) + "</b>";

                        if (document.SubheadlineHtml != "")
                            html += "<br />" + HttpUtility.HtmlEncode(document.SubheadlineHtml);

                        html += "</p>";
                    }
                }

                string body = "";

                if (document.BylineHtml != "")
                    body += "<b>" + document.BylineHtml.Replace("\r\n", "\n").Replace("\n", "<br />") + "</b>";

                body += document.BodyWithoutLocationHtml.Trim();

                if (body.Contains("<p>"))
                {
                    if (!body.StartsWith("<p>"))
                        body = "<p>" + body.Substring(0, body.IndexOf("<p>")) + "</p>" + body.Substring(body.IndexOf("<p>"));
                }
                else
                {
                    body = "<p>" + body + "</p>";
                }

                html += body;

                if (document.ContactsCount > 0)
                {
                    html += "<p><b>Contacts</b>:</p>"; /*Comm. Contacts*/

                    foreach (var contact in document.Contacts)
                    {

                        html += "<p>" + contact.PrimaryContactHtml + "</p>";

                        if (!string.IsNullOrWhiteSpace(contact.SecondaryContactText))
                            html += "<p>" + contact.SecondaryContactHtml + "</p>";
                    }
                }

                index++;
            }

            //html += "<p></p>";

            return html;
        }

        public byte[] ToPortableDocument()
        {
            using (ReportViewer viewer = new ReportViewer())
            {
                viewer.SetDisplayMode(DisplayMode.PrintLayout);

                viewer.ProcessingMode = ProcessingMode.Local;

                viewer.LocalReport.ReportEmbeddedResource = "Gcpe.News.ReleaseManagement.Templates.Resources.ReleaseReport.rdlc";

                viewer.LocalReport.DataSources.Add(new ReportDataSource("Release", new Release[] { this }));

                viewer.LocalReport.DataSources.Add(new ReportDataSource("Documents", Documents));

                viewer.LocalReport.SubreportProcessing += (sender, e) =>
                {
                    foreach (string dsn in e.DataSourceNames)
                    {
                        if (dsn == "Release")
                        {
                            e.DataSources.Add(new ReportDataSource(dsn, new Release[] { this }));
                        }
                        else if (dsn == "Documents")
                        {
                            e.DataSources.Add(new ReportDataSource(dsn, Documents));
                        }
                        else if (dsn == "Document")
                        {
                            Guid documentId = new Guid(e.Parameters["DocumentId"].Values.Single());
                            var document = Documents.Single(d => d.DocumentId == documentId);
                            document.BodyHtml = document.BodyHtml.Replace("&minus;", "" + (char)0x2013);

                            e.DataSources.Add(new ReportDataSource(dsn, new Document[] { document }));
                        }
                        else if (dsn == "Contacts")
                        {
                            Guid documentId = new Guid(e.Parameters["DocumentId"].Values.Single());
                            var document = Documents.Single(d => d.DocumentId == documentId);

                            e.DataSources.Add(new ReportDataSource(dsn, document.Contacts));
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                };

                string mimeType, encoding, extension;
                string[] streamIds;
                Warning[] warnings;

                byte[] newsReleaseData = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                if (warnings.Length > 0)
                    throw new ApplicationException(string.Join(Environment.NewLine, warnings.Skip(warnings.Length > 1 ? 1 : 0).Select(e => e.Message)));

                return newsReleaseData;
            }
        }
    }
}