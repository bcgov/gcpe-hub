extern alias legacy;

using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Xml;

namespace Gcpe.News.ReleaseManagement
{
    using legacy::Gcpe.Hub.Data.Entity;

    static class ConvertNewsRelease
    {

        internal static Dictionary<string, byte[]> GenerateFiles(HubEntities db, NewsRelease nr, string htmlFileName, string pdfFileName, string textFileName)
        {
            Templates.Release release = Templates.Release.FromEntity(nr);
            
            Dictionary<string, byte[]> documents = new Dictionary<string, byte[]>();
            byte[] bytesPdf = null, bytesText = null;

            // First get the HTML (initializes documents)
            if (!String.IsNullOrEmpty(htmlFileName))
            {
                documents = release.ToHtmlDocuments();
            }

            if (!String.IsNullOrEmpty(pdfFileName))
            {
                bytesPdf = release.ToPortableDocument();
            }

            if (!String.IsNullOrEmpty(textFileName))
            {
                bytesText = release.ToTextDocument();
            }

            if (documents == null)      // If HTML is not specified
            {
                documents = new Dictionary<string, byte[]>();
            }

            if (bytesPdf != null)
            {
                documents.Add(pdfFileName, bytesPdf);
            }
            if (bytesText != null)
            {
                documents.Add(textFileName, bytesText);
            }
            return documents;
        }

        internal static Dictionary<string, byte[]> GenerateFiles(HubEntities db, NewsRelease nr, string destFileDirectory)
        {
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

            if (!Directory.Exists(destFileDirectory))
            {
                Directory.CreateDirectory(destFileDirectory);
            }

            string nrNumber = nr.Key;

            string htmlFile = Path.Combine(destFileDirectory, nrNumber + ".htm");
            string pdfFile = Path.Combine(destFileDirectory, nrNumber + ".pdf");
            string textFile = Path.Combine(destFileDirectory, nrNumber + ".txt");

            Dictionary<string, byte[]> documents = GenerateFiles(db, nr, htmlFile, pdfFile, textFile);

            foreach (var document in documents)
            {
                //var history = new NewsReleaseHistory();
                //history.PublishDateTime = nr.PublishDateTime
                //nr.History.Add(history);

                if (document.Key.EndsWith(".pdf"))
                {
                }
                else if (document.Key.EndsWith(".txt"))
                {
                }
                else if (document.Key.EndsWith(".htm"))
                {
                }
            }

            foreach (string filePath in documents.Keys)
            {
                if (filePath.EndsWith(".pdf") || filePath.EndsWith(".txt"))
                {
                    System.IO.File.WriteAllBytes(filePath, documents[filePath]);
                }
                else
                {
                    // HTM files - do not have destFileDirectory
                    string htmlFilePath = Path.Combine(destFileDirectory, filePath);
                    System.IO.File.WriteAllBytes(htmlFilePath, documents[filePath]);
                }
            }

            return documents;
        }
    }
}