using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorporateCalendar.Utility
{
    public class GridHelper
    {
        public static string Linkify(string stringToLinkify, string url) {
            if (string.IsNullOrEmpty(stringToLinkify)) { throw new ArgumentException("StringToLinkify cannot be nothing."); }
            if (string.IsNullOrEmpty(url)) { throw new ArgumentException("Url cannot be nothing."); }
            string linkifiedString = string.Format("<a href=\"{1}\" target=\"_blank\" style=\"line-height:1.3em;\">{0}</a>", stringToLinkify, url);
            return linkifiedString;
        }
    }
}
