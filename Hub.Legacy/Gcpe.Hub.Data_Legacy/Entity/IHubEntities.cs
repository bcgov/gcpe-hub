using System.Data.Entity;

namespace Gcpe.Hub.Data.Entity
{
    public interface IHubEntities
    {
        DbSet<Language> Languages { get; }
        DbSet<Ministry> Ministries { get; }
        DbSet<MinistryLanguage> MinistryLanguages { get; }
        DbSet<NewsRelease> NewsReleases { get; }
        DbSet<NewsReleaseLanguage> NewsReleaseLanguages { get; }
        DbSet<Sector> Sectors { get; }
        DbSet<SectorLanguage> SectorLanguages { get; }
        DbSet<User> Users { get; }
        DbSet<NewsReleaseLog> NewsReleaseLogs { get; }
        DbSet<NewsReleaseImage> NewsReleaseImages { get; }
        DbSet<NewsReleaseImageLanguage> NewsReleaseImageLanguages { get; }
        DbSet<NewsReleaseHistory> NewsReleaseHistory { get; }
        DbSet<NewsReleaseType> NewsReleaseTypes { get; }
        DbSet<ApplicationSetting> ApplicationSettings { get; }
        DbSet<Blob> Blobs { get; }
        DbSet<NewsReleaseCollection> NewsReleaseCollections { get; }
        DbSet<NewsReleaseDocument> NewsReleaseDocuments { get; }
        DbSet<NewsReleaseDocumentContact> NewsReleaseDocumentContacts { get; }
        DbSet<NewsReleaseDocumentLanguage> NewsReleaseDocumentLanguages { get; }
    }
}
