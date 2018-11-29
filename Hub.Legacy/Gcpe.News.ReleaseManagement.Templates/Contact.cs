using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.News.ReleaseManagement.Templates
{
    public class Contact
    {
        public string PrimaryContactHtml { get; set; }

        public string PrimaryContactText
        {
            get { return Convert.HtmlToText(PrimaryContactHtml); }
        }

        public string SecondaryContactHtml { get; set; }

        public string SecondaryContactText
        {
            get { return SecondaryContactHtml == null ? "" : Convert.HtmlToText(SecondaryContactHtml); }
        }
    }
}