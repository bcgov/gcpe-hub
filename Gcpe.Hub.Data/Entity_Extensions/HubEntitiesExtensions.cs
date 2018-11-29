using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Gcpe.Hub.Data.Entity
{
    using NewsReleaseWhereExp = Expression<Func<NewsRelease, bool>>;

    //TODO: Remove explicit loading of Entity Framework Core data
    public static class HubEntitiesExtensions
    {
        public static Language English(this HubDbContext db)
        {
            return db.Language.Single(l => l.Id == 4105);
        }

        public static NewsReleaseLanguage Language(this EntityEntry<NewsRelease> release, int languageId)
        {
            var languages = release.Collection(e => e.NewsReleaseLanguage);

            if (!languages.IsLoaded)
                languages.Load();

            return release.Entity.NewsReleaseLanguage.Single(e => e.LanguageId == languageId);
        }

        public static NewsReleaseLanguage English(this EntityEntry<NewsRelease> release)
        {
            return Language(release, Entity.Language.enCA);
        }

        public static NewsReleaseDocumentLanguage Language(this EntityEntry<NewsReleaseDocument> document, int languageId)
        {
            var languages = document.Collection(e => e.NewsReleaseDocumentLanguage);

            if (!languages.IsLoaded)
                languages.Load();

            return document.Entity.NewsReleaseDocumentLanguage.Single(e => e.LanguageId == languageId);
        }

        public static NewsReleaseDocumentLanguage English(this EntityEntry<NewsReleaseDocument> document)
        {
            return Language(document, Entity.Language.enCA);
        }

        public static MinistryLanguage Language(this EntityEntry<Ministry> ministry, int languageId)
        {
            var languages = ministry.Collection(e => e.MinistryLanguage);

            if (!languages.IsLoaded)
                languages.Load();

            return ministry.Entity.MinistryLanguage.Single(e => e.LanguageId == languageId);
        }

        public static MinistryLanguage English(this EntityEntry<Ministry> ministry)
        {
            return Language(ministry, Entity.Language.enCA);
        }

        public static SectorLanguage Language(this EntityEntry<Sector> sector, int languageId)
        {
            var languages = sector.Collection(e => e.SectorLanguage);

            if (!languages.IsLoaded)
                languages.Load();

            return sector.Entity.SectorLanguage.Single(e => e.LanguageId == languageId);
        }

        public static SectorLanguage English(this EntityEntry<Sector> sector)
        {
            return Language(sector, Entity.Language.enCA);
        }

        public static NewsReleaseWhereExp PublishDateTimeHasBeenReached()
        {
            DateTimeOffset now = DateTimeOffset.Now;
            DateTime dtNow = DateTime.Now;

            return (r => r.PublishDateTime < now && (!r.ReleaseDateTime.HasValue || r.ReleaseDateTime.Value < dtNow));
        }

        public static IQueryable<NewsRelease> GetPublishingOrPublished(this DbSet<NewsRelease> releases)
        {
            return releases.Where(r => r.IsActive && r.IsCommitted).Where(PublishDateTimeHasBeenReached());
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

        private const string HomeTopReleaseId = "HomeTopReleaseId";
        private const string HomeFeatureReleaseId = "HomeFeatureReleaseId";
        public static async Task<NewsRelease> GetHomeTopReleaseAsync(this HubDbContext db, IDictionary<string, string> applicationSettings)
        {
            return await db.NewsRelease.FindAsync(Guid.Parse(applicationSettings[HomeTopReleaseId]));
        }
        public static async Task<NewsRelease> GetHomeFeatureReleaseAsync(this HubDbContext db, IDictionary<string, string> applicationSettings)
        {
            return await db.NewsRelease.FindAsync(Guid.Parse(applicationSettings[HomeFeatureReleaseId]));
        }
    }
}