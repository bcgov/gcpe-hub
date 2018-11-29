using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gcpe.News.ReleaseManagement.Templates
{
    using System.Text.RegularExpressions;
    using System.Net;
    using HtmlAgilityPack;

    public static class Convert
    {
        /// <summary>
        /// match evaluator for regex to replace html entities
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        static string HtmlEntityMatchEvaluator(Match m)
        {
            if (m.Value == "&gt;" || m.Value == "&lt;" || m.Value == "&amp;" || m.Value == "&quot;" || m.Value == "&apos;") return m.Value;
            return WebUtility.HtmlDecode(m.Value);
        }

        /// <summary>
        /// Converts a string of html into plain text
        /// 
        /// The html is loaded as an xml document (so it must be xhtml-compliant) and then nodes are processed recursively
        /// </summary>
        /// <param name="html">xhtml-compliant html string</param>
        /// <returns>plain-text representation of html</returns>
        public static string HtmlToText(string html)
        {
            if (html == "")
                return "";

            var AssetRegex = new System.Text.RegularExpressions.Regex("<asset>(?<url>[^<]+)</asset>");

            html = AssetRegex.Replace(html, new System.Text.RegularExpressions.MatchEvaluator(match =>
            {
                //return "<div><a href=\"" + match.Groups["url"].Value + "\">" + match.Groups["url"].Value + "</a></div>";
                return "";
            }));

            html = html.Replace("<p>&nbsp;</p>", "");
            html = html.Replace("<p></p>", "");

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            StringBuilder result = new StringBuilder();

            foreach (HtmlNode node in doc.DocumentNode.ChildNodes)
            {
                result.Append(ProcessNode(node));
            }

            return result.ToString();
        }

        /// <summary>
        /// processes xhtml nodes into plain text
        /// 
        /// recognizes nodes based on node name (eg "<a href='#'>" is "a")
        /// </summary>
        /// <param name="node">the xml node currently being processed</param>
        /// <returns>a string representing plain text of the node's contents</returns>
        private static string ProcessNode(HtmlNode node)
        {
            if (node is HtmlTextNode)
                return System.Web.HttpUtility.HtmlDecode(node.InnerText.Trim('\r', '\n'));

            switch (node.Name.ToLower())
            {
                case "a":
                    return ProcessATag(node);
                case "p":
                    return ProcessPTag(node);
                case "ol":
                    return ProcessOLTag(node);
                case "ul":
                    return ProcessULTag(node);
                case "br":
                    return ProcessBRTag(node);
                case "div":
                    return ProcessDIVTag(node);
                case "b":
                    return ProcessBTag(node);
                case "strong":
                    return ProcessSTRONGTag(node);
                default:
                    //do not process
                    return ProcessOtherTag(node);
            }
        }

        /// <summary>
        /// processes the STRONG tag
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string ProcessSTRONGTag(HtmlNode node)
        {
            StringBuilder sb = new StringBuilder();
            foreach (HtmlNode cnode in node.ChildNodes)
            {
                sb.Append(ProcessNode(cnode));
            }
            return sb.ToString();
        }
        /// <summary>
        /// processes the B tag
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string ProcessBTag(HtmlNode node)
        {
            return ProcessSTRONGTag(node);
        }

        /// <summary>
        /// processes the BR tag
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string ProcessBRTag(HtmlNode node)
        {
            return Environment.NewLine;
        }
        /// <summary>
        /// processes the DIV tag
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string ProcessDIVTag(HtmlNode node)
        {
            return ProcessPTag(node);
        }
        /// <summary>
        /// processes any other tag by processing its child tags
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string ProcessOtherTag(HtmlNode node)
        {
            StringBuilder sb = new StringBuilder();
            foreach (HtmlNode cnode in node.ChildNodes)
            {
                sb.Append(ProcessNode(cnode));
            }
            return sb.ToString();
        }
        /// <summary>
        /// processes the A tag
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string ProcessATag(HtmlNode node)
        {
            string href = null;

            var hrefAttribute = node.Attributes.SingleOrDefault(e => e.Name == "href");

            //exclude anchors
            if (hrefAttribute != null && !hrefAttribute.Value.StartsWith("#"))
                href = hrefAttribute.Value;

            string text = "";

            StringBuilder sb = new StringBuilder();
            foreach (HtmlNode cnode in node.ChildNodes)
            {
                text += ProcessNode(cnode);
            }

            sb.Append(text);

            if (!String.IsNullOrWhiteSpace(href))
            {
                if (text.Replace("http://", "").TrimEnd('/') != href.Replace("http://", "").TrimEnd('/'))
                    sb.Append(" (" + href + ")");
            }

            return sb.ToString();
        }
        /// <summary>
        /// processes the P tag
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string ProcessPTag(HtmlNode node)
        {
            StringBuilder sb = new StringBuilder();
            foreach (HtmlNode cnode in node.ChildNodes)
            {
                sb.Append(ProcessNode(cnode));
            }
            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// processes the OL tag and its LI children (numbers the list items)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string ProcessOLTag(HtmlNode node)
        {
            //TODO use "Levels" for OLs

            StringBuilder sb = new StringBuilder();
            
            int liNumber = 1;
            foreach (HtmlNode cnode in node.ChildNodes)
            {
                if (cnode.Name == "li")
                    sb.Append((liNumber++) + ". ");

                //foreach (XmlNode ccnode in cnode)
                //    sb.AppendLine(ProcessNode(ccnode));

                foreach (HtmlNode ccnode in cnode.ChildNodes)
                {
                    sb.Append(ProcessNode(ccnode));
                }

                sb.AppendLine();
            }

            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// processes the UL tag and its LI children (adds * as bullets)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string ProcessULTag(HtmlNode node)
        {
            StringBuilder sb = new StringBuilder();

            foreach (HtmlNode cnode in node.ChildNodes)
            {
                if (cnode.Name == "li")
                    sb.Append("* ");

                foreach (HtmlNode ccnode in cnode.ChildNodes)
                {
                    sb.Append(ProcessNode(ccnode));
                }

                sb.AppendLine();
            }

            sb.AppendLine();

            return sb.ToString();
        }
    }
}
