using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.News.ReleaseManagement.Controls
{
    using System.Text.RegularExpressions;

    //var tagRegex = /\<\/?([\w]+).*?\>/gi;
    //var html = args.get_value();
    ////strip all attributes from all tags except href from a
    ////html = html.replace(/\<[\s]*([\w]+).*?\>/gi, "\<$1\>");

    ////populate "clean" html (to do replacements without messing up regex matching index
    //var cleanHtml = html;
    ////remove illegal tags
    //var legalTags = ['a', 'p', 'ul', 'ol', 'li', 'div', 'b', 'strong', 'br'];
    //var tag = tagRegex.exec(html); //executes match from index of last match, returns null if no more matches
    //while (tag != null) {
            
    //    if (legalTags.indexOf(tag[1]) < 0) {
    //        cleanHtml = cleanHtml.replace(tag[0], '');
    //    } else {
    //        if (tag[1] != "a") {
    //            //strip attributes
    //            var attrRegex = new RegExp("<[\\s]*(" + tag[1] + ").*?>", "gi");
    //            console.log(attrRegex.exec(cleanHtml));
    //            cleanHtml = cleanHtml.replace(attrRegex, "\<$1\>");
    //        } else {
    //            //strip all except href
    //            var hrefRegex = /href=\"(.*?)\"/i;
    //            var mhref = hrefRegex.exec(tag[0]);
    //            if (mhref != null) {
    //                //it had an href, so replace tag[0] with <a href="$1">
    //                cleanHtml = cleanHtml.replace(tag[0], "<a href=\"" + mhref[1] + "\">");
    //            }
    //        }
    //    }
    //    tag = tagRegex.exec(html);

    public static class HtmlTagCleaner
    {
        public static string[] allowedTags = { "a", "p", "ul", "ol", "li", "b", "strong", "br", "div" };
        //public static string[] bulletChars = { "·", "o(?:\\&nbsp\\;)+", "§(?:\\&nbsp\\;)+" };

        public static string StripDisallowedTags(string html)
        {
            html = Regex.Replace(html, "<[\\s]*b[\\s]*>", "<strong>");
            html = Regex.Replace(html, "<\\/[\\s]*b[\\s]*>", "</strong>");

            string result = html;
            
            Regex tagRegex = new Regex(@"\<\/?([\w]+)(?:[\s]+.*?)?\>");
            Match m = tagRegex.Match(html.ToLower());
            while(m.Success)
            {

                if (!allowedTags.Contains(m.Groups[1].Value))
                {
                    //strip tag
                    result = result.Replace(m.Groups[0].Value, "");
                } else {
                    if (m.Groups[1].Value != "a") {
                        //strip attributes
                        var attrRegex = new Regex("<[\\s]*(" + m.Groups[1] + ").*?>", RegexOptions.IgnoreCase);
                        result = attrRegex.Replace(result, "<$1>");
                    } else {
                        //strip all except href
                        var hrefRegex = new Regex("href=\\\"(.*?)\\\"", RegexOptions.IgnoreCase);
                        var mhref = hrefRegex.Match(m.Groups[0].Value);
                        if (mhref.Success) {
                            //it had an href, so replace tag[0] with <a href="$1">
                            result = result.Replace(m.Groups[0].Value, "<a href=\"" + mhref.Groups[1].Value + "\">");
                        }
                    }
                }
                m = m.NextMatch();
            }

            //foreach (var bullet in bulletChars)
            //{
            //    result = Regex.Replace(result, @"<p>(?:\&nbsp\;)*" + bullet + @"(?:\&nbsp\;)*", "<p>");
            //}

            result = Regex.Replace(result, @"<p>[\s]*(?:<strong>)?[\s]*\&nbsp\;[\s]*(?:<\/strong>)?[\s]*<\/p>", "");
            return result; 
        }
    }
}
