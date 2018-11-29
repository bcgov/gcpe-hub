using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.News.ReleaseManagement.Templates
{
    public class Document
    {
        public Release Release { get; set; }

        public int PageLayout { get; set; }

        public byte[] PageImage { get; set; }

        public string PageImageName { get; set; }

        public string PageImageAlternateName { get; set; }

        public string PageTitle { get; set; }

        public Guid DocumentId { get; set; }

        public string ReferenceNumber { get; set; }

        public string Organizations { get; set; }

        public string LanguageCode { get; set; }

        public string Headline { get; set; }

        public string SubheadlineHtml { get; set; }

        [Obsolete] 
        public string Byline { get { return BylineHtml; } }

        public string BylineHtml { get; set; }

        public string BodyHtml { get; set; }

        public string BodyWithoutLocationHtml { get; set; }

        public string BodyText
        {
            get { return Convert.HtmlToText(BodyHtml); }
        }

        //TODO: Remove Duplicate Code with DocumentModel.cs
        public string TopLeftHeader
        {
            get
            {
                if (PageLayout == 1)
                {
                    string text;

                    if (LanguageCode == "en-CA")
                    {
                        text = Release.IsReleased ? "For Immediate Release" : "";
                        text += (text == "" ? "" : "<br />") + ReferenceNumber;
                        text += (text == "" ? "" : "<br />") + Release.CPDateFormat(Release.ReleaseDateTime);
                    }
                    else if (LanguageCode == "fr-CA")
                    {
                        text = Release.IsReleased ? "Pour diffusion immédiate" : "";
                        text += (text == "" ? "" : "<br />") + ReferenceNumber;
                        text += (text == "" ? "" : "<br />") + Release.ReleaseDateTime.ToString("d MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo(LanguageCode));
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    return text;
                }
                else
                {
                    return null;
                }
            }
        }

        public string TopRightHeader
        {
            get
            {
                if (PageLayout == 1)
                {
                    return Organizations.Replace("\r", "").Replace("\n", "<br />");
                }
                else
                {
                    return null;
                }
            }
        }

        public string MiddleHeader
        {
            get
            {
                return "<b>" + Headline + "</b>" + (string.IsNullOrEmpty(SubheadlineHtml) ? "" : "<br />" + SubheadlineHtml);
            }
        }
                
        public int ContactsCount
        {
            get { return contactsCount; }
        }

        private readonly List<Contact> contacts;
        public IEnumerable<Contact> Contacts
        {
            get { return contacts.AsReadOnly(); }
        }

        int contactsCount = 0;

        public void AddContact(string contactHtml)
        {
            if (contactsCount % 2 == 0)
            {
                contacts.Add(new Contact()
                {
                    PrimaryContactHtml = contactHtml
                });
            }
            else
            {
                System.Diagnostics.Debug.Assert(contacts[contactsCount / 2].SecondaryContactHtml == null);

                contacts[contactsCount / 2].SecondaryContactHtml = contactHtml;
            }

            contactsCount++;
        }

        public Document()
        {
            contacts = new List<Contact>();
        }
    }
}
