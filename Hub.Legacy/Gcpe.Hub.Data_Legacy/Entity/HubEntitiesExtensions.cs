using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.Hub.Data.Entity
{
    using System.Data.Entity;

    public static class HubEntitiesExtensions
    {
        public static Language English(this IHubEntities db)
        {
            return db.Languages.Single(l => l.Id == 4105);
        }

        public static NewsReleaseLanguage Language(this NewsRelease release, int languageId)
        {
            return release.Languages.Single(e => e.Language.Id == languageId);
        }

        public static NewsReleaseLanguage English(this NewsRelease release)
        {
            return release.Languages.Single(e => e.Language.Id == 4105);
        }

        public static NewsReleaseDocumentLanguage Language(this NewsReleaseDocument document, int languageId)
        {
            return document.Languages.Single(e => e.Language.Id == languageId);
        }

        public static NewsReleaseDocumentLanguage English(this NewsReleaseDocument document)
        {
            return document.Languages.Single(e => e.Language.Id == 4105);
        }

        public static MinistryLanguage English(this Ministry ministry)
        {
            return ministry.Languages.Single(e => e.Language.Id == 4105);
        }

        public static SectorLanguage English(this Sector sector)
        {
            return sector.Languages.Single(e => e.Language.Id == 4105);
        }

        public static MinistryLanguage Language(this Ministry ministry, int languageId)
        {
            return ministry.Languages.Single(e => e.Language.Id == languageId);
        }

        public static SectorLanguage Language(this Sector sector, int languageId)
        {
            return sector.Languages.Single(e => e.Language.Id == languageId);
        }

        public static string ReleasePathName(this NewsRelease post)
        {
            switch (post.ReleaseType.ToString())
            {
                case "Story":
                    return "stories";
                case "Advisory":
                    return "advisories";
                default:
                    return post.ReleaseType.ToString().ToLowerInvariant() + "s";
            }
        }

        public static bool IsPublishNewsOnDemand(this NewsRelease post)
        {
            return post.PublishOptions.HasFlag(PublishOptions.PublishNewsOnDemand);
        }

        public static bool IsPublishMediaContacts(this NewsRelease post)
        {
            return post.PublishOptions.HasFlag(PublishOptions.PublishMediaContacts);
        }

        public static string GetAtomId(this NewsRelease post)
        {
            return post.AtomId.Length != 0 ? post.AtomId : "uuid:" + post.Id.ToString().ToLower();
        }

        public static IQueryable<NewsRelease> GetPublishingOrPublished(this DbSet<NewsRelease> releases)
        {
            DateTimeOffset now = DateTimeOffset.Now;
            DateTime dtNow = DateTime.Now;

            var publishingOrPublished = from r in releases
                                        where r.IsCommitted && r.PublishDateTime < now && (!r.ReleaseDateTime.HasValue || r.ReleaseDateTime.Value < dtNow) && r.IsActive
                                        select r;

            return publishingOrPublished;
        }

        public static IQueryable<NewsRelease> GetPublished(this DbSet<NewsRelease> releases)
        {
            var publishedReleases = from r in releases
                                    where r.IsCommitted && r.IsPublished && r.IsActive
                                    select r;

            return publishedReleases;
        }

        public static Blob FindOrNew(this DbSet<Blob> blobs, byte[] data)
        {
            Guid blobId;

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hash = md5.ComputeHash(data);
                blobId = new Guid(hash);
            }


            Blob blob = blobs.Find(blobId);

            if (blob == null)
            {
                blob = new Blob();
                blob.Id = blobId;
                blob.Data = data;
            }

            return blob;
        }
    }
}