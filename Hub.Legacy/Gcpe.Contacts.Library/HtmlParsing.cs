using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MediaRelationsLibrary {
    /// <summary>
    /// This class contains methods that are used to parse and handle
    /// html from within C# code
    /// </summary>
    public class HtmlParsing {

        private static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
        
        /// <summary>
        /// Returns plain text string from a given HTML string
        /// </summary>
        /// <param name="htmlString"></param>
        /// <returns></returns>
        public static string GetPlainTextFromHTMLString(string htmlString) {
            return _htmlRegex.Replace(htmlString, string.Empty);
        }

        /// <summary>
        /// Returns the text string that was inputted with <script..></script>
        /// tags removed from the text
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string RemoveScriptTagsFromHTMLString(string inputString) {
            inputString = Regex.Replace(inputString, "<script.*?>.*?</script>", "",
                                        RegexOptions.IgnoreCase | RegexOptions.Singleline);

            return inputString;
        }

    }
}
