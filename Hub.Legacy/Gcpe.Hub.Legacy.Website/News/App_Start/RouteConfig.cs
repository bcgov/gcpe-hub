using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;
using Microsoft.AspNet.FriendlyUrls.Resolvers;

namespace Gcpe.Hub.News
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var settings = new FriendlyUrlSettings();

            //settings.AutoRedirectMode = RedirectMode.Temporary;
            settings.AutoRedirectMode = RedirectMode.Off;
            
            var resolvers = new IFriendlyUrlResolver[] { new NewsFriendlyUrlResolver() };

            routes.EnableFriendlyUrls(settings, resolvers);

            routes.MapPageRoute("ReleaseCreate", "News/ReleaseManagement/New", "~/News/ReleaseManagement/New.aspx", true);

            routes.MapPageRoute("ReleaseFolderReferenceDocument", "News/ReleaseManagement/{Folder}/{Reference}/{Document}/{Language}", "~/News/ReleaseManagement/Document.aspx", true, new RouteValueDictionary() { { "Language", "en-CA" } }, new RouteValueDictionary() { { "Folder", "^(Drafts)|(Scheduled)|(Published)$" }, { "Reference", @"^[0-9]{4}[A-Z]+[0-9]{4}\-[0-9]{6}$" }, { "Document", "^[0-9]+$" } });
            routes.MapPageRoute("ReleaseFolderReference", "News/ReleaseManagement/{Folder}/{Reference}", "~/News/ReleaseManagement/Release.aspx", true, null, new RouteValueDictionary() { { "Folder", "^(Drafts)|(Scheduled)|(Published)$" }, { "Reference", @"^([0-9]{4}[A-Z]+[0-9]{4}\-[0-9]{6})|(NEWS-[0-9]{5})$" } });

            routes.MapPageRoute("ReleaseFolderIdDocument", "News/ReleaseManagement/{Folder}/{Id}/{Document}/{Language}", "~/News/ReleaseManagement/Document.aspx", true, new RouteValueDictionary() { { "Language", "en-CA" } }, new RouteValueDictionary() { { "Folder", "^(Drafts)|(Scheduled)|(Published)$" }, { "Id", @"^[ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789\-_]{22}$" }, { "Document", "^[0-9]+$" } });
            routes.MapPageRoute("ReleaseFolderId", "News/ReleaseManagement/{Folder}/{Id}", "~/News/ReleaseManagement/Release.aspx", true, null, new RouteValueDictionary() { { "Folder", "^(Drafts)|(Scheduled)|(Published)$" }, { "Id", @"^[ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789\-_]{22}$" } });

            routes.MapPageRoute("ReleaseFolder", "News/ReleaseManagement/{Folder}/{Type}", "~/News/ReleaseManagement/Releases.aspx", true, new RouteValueDictionary() { { "Type", "All" } }, new RouteValueDictionary() { { "Folder", "^(Forecast)|(Drafts)|(Scheduled)|(Published)$" }, { "Type", "^(All)|(Story)|(Release)|(Factsheet)|(Update)|(Advisory)$" } });
            routes.MapPageRoute("Carousel", "News/ReleaseManagement/Carousel", "~/News/ReleaseManagement/Carousel.aspx", true);
        }
    }

    public class NewsFriendlyUrlResolver : WebFormsFriendlyUrlResolver
    {
        public override string ConvertToFriendlyUrl(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (path.ToLower().StartsWith("/Calendar/Admin/".ToLower()))
                {
                    return path; 
                }
            }

            return base.ConvertToFriendlyUrl(path);
        }
    }
}