using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Gcpe.Hub.Data.Entity
{
    public partial class HubDbContext : DbContext
    {
        public virtual DbSet<Activity> Activity { get; set; }
        public virtual DbSet<ActivityCategories> ActivityCategories { get; set; }
        public virtual DbSet<ActivityCommunicationMaterials> ActivityCommunicationMaterials { get; set; }
        public virtual DbSet<ActivityFiles> ActivityFiles { get; set; }
        public virtual DbSet<ActivityFilter> ActivityFilter { get; set; }
        public virtual DbSet<ActivityInitiatives> ActivityInitiatives { get; set; }
        public virtual DbSet<ActivityKeywords> ActivityKeywords { get; set; }
        public virtual DbSet<ActivityNrorigins> ActivityNrorigins { get; set; }
        public virtual DbSet<ActivitySectors> ActivitySectors { get; set; }
        public virtual DbSet<ActivityServices> ActivityServices { get; set; }
        public virtual DbSet<ActivitySharedWith> ActivitySharedWith { get; set; }
        public virtual DbSet<ActivityThemes> ActivityThemes { get; set; }
        public virtual DbSet<ApplicationSetting> ApplicationSetting { get; set; }
        public virtual DbSet<Beat> Beat { get; set; }
        public virtual DbSet<Blob> Blob { get; set; }
        public virtual DbSet<Carousel> Carousel { get; set; }
        public virtual DbSet<CarouselSlide> CarouselSlide { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<CommunicationContact> CommunicationContact { get; set; }
        public virtual DbSet<CommunicationMaterial> CommunicationMaterial { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<CompanyAddress> CompanyAddress { get; set; }
        public virtual DbSet<CompanyDistribution> CompanyDistribution { get; set; }
        public virtual DbSet<CompanyElectoralDistrict> CompanyElectoralDistrict { get; set; }
        public virtual DbSet<CompanyEthnicity> CompanyEthnicity { get; set; }
        public virtual DbSet<CompanyLanguage> CompanyLanguage { get; set; }
        public virtual DbSet<CompanyMediaDesk> CompanyMediaDesk { get; set; }
        public virtual DbSet<CompanyMediaPartner> CompanyMediaPartner { get; set; }
        public virtual DbSet<CompanyMediaType> CompanyMediaType { get; set; }
        public virtual DbSet<CompanyPhoneNumber> CompanyPhoneNumber { get; set; }
        public virtual DbSet<CompanyPublicationDays> CompanyPublicationDays { get; set; }
        public virtual DbSet<CompanyRegion> CompanyRegion { get; set; }
        public virtual DbSet<CompanySector> CompanySector { get; set; }
        public virtual DbSet<CompanySpecialtyPublication> CompanySpecialtyPublication { get; set; }
        public virtual DbSet<CompanyWebAddress> CompanyWebAddress { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<ContactAddress> ContactAddress { get; set; }
        public virtual DbSet<ContactBeat> ContactBeat { get; set; }
        public virtual DbSet<ContactCity> ContactCity { get; set; }
        public virtual DbSet<ContactElectoralDistrict> ContactElectoralDistrict { get; set; }
        public virtual DbSet<ContactMediaJobTitle> ContactMediaJobTitle { get; set; }
        public virtual DbSet<ContactPhoneNumber> ContactPhoneNumber { get; set; }
        public virtual DbSet<ContactRegion> ContactRegion { get; set; }
        public virtual DbSet<ContactSector> ContactSector { get; set; }
        public virtual DbSet<ContactWebAddress> ContactWebAddress { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Distribution> Distribution { get; set; }
        public virtual DbSet<ElectoralDistrict> ElectoralDistrict { get; set; }
        public virtual DbSet<Ethnicity> Ethnicity { get; set; }
        public virtual DbSet<EventPlanner> EventPlanner { get; set; }
        public virtual DbSet<FavoriteActivity> FavoriteActivity { get; set; }
        public virtual DbSet<GovernmentRepresentative> GovernmentRepresentative { get; set; }
        public virtual DbSet<Initiative> Initiative { get; set; }
        public virtual DbSet<Keyword> Keyword { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<MediaDesk> MediaDesk { get; set; }
        public virtual DbSet<MediaDistributionList> MediaDistributionList { get; set; }
        public virtual DbSet<MediaJobTitle> MediaJobTitle { get; set; }
        public virtual DbSet<MediaLanguage> MediaLanguage { get; set; }
        public virtual DbSet<MediaRequest> MediaRequest { get; set; }
        public virtual DbSet<MediaRequestContact> MediaRequestContact { get; set; }
        public virtual DbSet<MediaRequestResolution> MediaRequestResolution { get; set; }
        public virtual DbSet<MediaRequestSharedMinistry> MediaRequestSharedMinistry { get; set; }
        public virtual DbSet<MediaType> MediaType { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<MinisterialJobTitle> MinisterialJobTitle { get; set; }
        public virtual DbSet<Ministry> Ministry { get; set; }
        public virtual DbSet<MinistryLanguage> MinistryLanguage { get; set; }
        public virtual DbSet<MinistryNewsletter> MinistryNewsletter { get; set; }
        public virtual DbSet<MinistrySector> MinistrySector { get; set; }
        public virtual DbSet<MinistryService> MinistryService { get; set; }
        public virtual DbSet<MinistryTopic> MinistryTopic { get; set; }
        public virtual DbSet<NewsFeed> NewsFeed { get; set; }
        public virtual DbSet<NewsRegion> NewsRegion { get; set; }
        public virtual DbSet<NewsRelease> NewsRelease { get; set; }
        public virtual DbSet<NewsReleaseCollection> NewsReleaseCollection { get; set; }
        public virtual DbSet<NewsReleaseDocument> NewsReleaseDocument { get; set; }
        public virtual DbSet<NewsReleaseDocumentContact> NewsReleaseDocumentContact { get; set; }
        public virtual DbSet<NewsReleaseDocumentLanguage> NewsReleaseDocumentLanguage { get; set; }
        public virtual DbSet<NewsReleaseHistory> NewsReleaseHistory { get; set; }
        public virtual DbSet<NewsReleaseImage> NewsReleaseImage { get; set; }
        public virtual DbSet<NewsReleaseImageLanguage> NewsReleaseImageLanguage { get; set; }
        public virtual DbSet<NewsReleaseLanguage> NewsReleaseLanguage { get; set; }
        public virtual DbSet<NewsReleaseLog> NewsReleaseLog { get; set; }
        public virtual DbSet<NewsReleaseMediaDistribution> NewsReleaseMediaDistribution { get; set; }
        public virtual DbSet<NewsReleaseMinistry> NewsReleaseMinistry { get; set; }
        public virtual DbSet<NewsReleaseSector> NewsReleaseSector { get; set; }
        public virtual DbSet<NewsReleaseService> NewsReleaseService { get; set; }
        public virtual DbSet<NewsReleaseTag> NewsReleaseTag { get; set; }
        public virtual DbSet<NewsReleaseTheme> NewsReleaseTheme { get; set; }
        public virtual DbSet<NewsReleaseType> NewsReleaseType { get; set; }
        public virtual DbSet<Nrdistribution> Nrdistribution { get; set; }
        public virtual DbSet<Nrorigin> Nrorigin { get; set; }
        public virtual DbSet<PhoneType> PhoneType { get; set; }
        public virtual DbSet<PremierRequested> PremierRequested { get; set; }
        public virtual DbSet<PrintCategory> PrintCategory { get; set; }
        public virtual DbSet<Priority> Priority { get; set; }
        public virtual DbSet<ProvState> ProvState { get; set; }
        public virtual DbSet<PublicationDays> PublicationDays { get; set; }
        public virtual DbSet<PublicationFrequency> PublicationFrequency { get; set; }
        public virtual DbSet<Report> Report { get; set; }
        public virtual DbSet<ResourceLink> ResourceLink { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Sector> Sector { get; set; }
        public virtual DbSet<SectorLanguage> SectorLanguage { get; set; }
        public virtual DbSet<Service> Service { get; set; }
        public virtual DbSet<Slide> Slide { get; set; }
        public virtual DbSet<SocialMediaPost> SocialMediaPost { get; set; }
        public virtual DbSet<SpecialtyPublication> SpecialtyPublication { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<SysConfig> SysConfig { get; set; }
        public virtual DbSet<SysLog> SysLog { get; set; }
        public virtual DbSet<SystemUser> SystemUser { get; set; }
        public virtual DbSet<SystemUserMinistry> SystemUserMinistry { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<Theme> Theme { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Videographer> Videographer { get; set; }
        public virtual DbSet<WebAddressType> WebAddressType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Activity>(entity =>
            {
                entity.HasIndex(e => e.EndDateTime)
                    .HasName("IX_EndDateTime");

                entity.HasIndex(e => e.StartDateTime)
                    .HasName("IX_StartDateTime");

                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Details).HasDefaultValueSql("('')");

                entity.Property(e => e.HqSection).HasDefaultValueSql("((2))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Schedule).HasDefaultValueSql("('')");

                entity.Property(e => e.Significance).HasDefaultValueSql("('')");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.Property(e => e.Title).HasDefaultValueSql("('')");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_Activity_City");

                entity.HasOne(d => d.CommunicationContact)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.CommunicationContactId)
                    .HasConstraintName("FK_Activity_CommunicationContact");

                entity.HasOne(d => d.ContactMinistry)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.ContactMinistryId)
                    .HasConstraintName("FK_Activity_Ministry");

                entity.HasOne(d => d.EventPlanner)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.EventPlannerId)
                    .HasConstraintName("FK_Activity_EventPlanner");

                entity.HasOne(d => d.GovernmentRepresentative)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.GovernmentRepresentativeId)
                    .HasConstraintName("FK_Activity_GovernmentRepresentative");

                entity.HasOne(d => d.HqStatus)
                    .WithMany(p => p.ActivityHqStatus)
                    .HasForeignKey(d => d.HqStatusId)
                    .HasConstraintName("FK_Activity_HqStatus");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.ActivityLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("FK_Activity_SystemUser_UpdatedBy");

                entity.HasOne(d => d.Nrdistribution)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.NrdistributionId)
                    .HasConstraintName("FK_Activity_NRDistribution");

                entity.HasOne(d => d.PremierRequested)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.PremierRequestedId)
                    .HasConstraintName("FK_Activity_PremierRequested");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.ActivityStatus)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_Activity_Status");

                entity.HasOne(d => d.Videographer)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.VideographerId)
                    .HasConstraintName("FK_Activity_Videographer");
            });

            modelBuilder.Entity<ActivityCategories>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("IX_ActivityId");

                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivityCategories)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityCategories_Activity");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.ActivityCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityCategories_Category");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.ActivityCategoriesLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("FK_ActivityCategories_SystemUser_UpdatedBy");
            });

            modelBuilder.Entity<ActivityCommunicationMaterials>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("IX_ActivityId");

                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivityCommunicationMaterials)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityCommunicationMaterials_Activity");

                entity.HasOne(d => d.CommunicationMaterial)
                    .WithMany(p => p.ActivityCommunicationMaterials)
                    .HasForeignKey(d => d.CommunicationMaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityCommunicationMaterials_CommunicationMaterial");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.ActivityCommunicationMaterialsLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("FK_ActivityCommunicationMaterials_SystemUser_UpdatedBy");
            });

            modelBuilder.Entity<ActivityFiles>(entity =>
            {
                entity.Property(e => e.FileName).IsUnicode(false);

                entity.Property(e => e.FileType).IsUnicode(false);

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Md5).IsUnicode(false);

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivityFiles)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityFiles_Activity");
            });

            modelBuilder.Entity<ActivityFilter>(entity =>
            {
                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.ActivityFilterLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("FK_ActivityFilter_SystemUser_UpdatedBy");
            });

            modelBuilder.Entity<ActivityInitiatives>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("IX_ActivityId");

                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivityInitiatives)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityInitiatives_Activity");

                entity.HasOne(d => d.Initiative)
                    .WithMany(p => p.ActivityInitiatives)
                    .HasForeignKey(d => d.InitiativeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityInitiatives_Initiative");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.ActivityInitiativesLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("FK_ActivityInitiatives_SystemUser_UpdatedBy");
            });

            modelBuilder.Entity<ActivityKeywords>(entity =>
            {
                entity.HasKey(e => new { e.ActivityId, e.KeywordId });

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivityKeywords)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityKeywords_Activity");

                entity.HasOne(d => d.Keyword)
                    .WithMany(p => p.ActivityKeywords)
                    .HasForeignKey(d => d.KeywordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityKeywords_Keyword");
            });

            modelBuilder.Entity<ActivityNrorigins>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("IX_ActivityId");

                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivityNrorigins)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityNROrigins_Activity");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.ActivityNroriginsLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("FK_ActivityNROrigins_SystemUser_UpdatedBy");

                entity.HasOne(d => d.Nrorigin)
                    .WithMany(p => p.ActivityNrorigins)
                    .HasForeignKey(d => d.NroriginId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityNROrigins_NROrigin");
            });

            modelBuilder.Entity<ActivitySectors>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("IX_ActivityId");

                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivitySectors)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivitySectors_Activity");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.ActivitySectorsLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("FK_ActivitySectors_SystemUser_UpdatedBy");

                entity.HasOne(d => d.Sector)
                    .WithMany(p => p.ActivitySectors)
                    .HasForeignKey(d => d.SectorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivitySectors_Sector");
            });

            modelBuilder.Entity<ActivityServices>(entity =>
            {
                entity.HasKey(e => new { e.ActivityId, e.ServiceId });

                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivityServices)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityServices_Activity");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ActivityServices)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityServices_Service");
            });

            modelBuilder.Entity<ActivitySharedWith>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("IX_ActivityId");

                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivitySharedWith)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivitySharedWith_Activity");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.ActivitySharedWithLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("FK_ActivitySharedWith_SystemUser_UpdatedBy");

                entity.HasOne(d => d.Ministry)
                    .WithMany(p => p.ActivitySharedWith)
                    .HasForeignKey(d => d.MinistryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivitySharedWith_Ministry");
            });

            modelBuilder.Entity<ActivityThemes>(entity =>
            {
                entity.HasKey(e => new { e.ActivityId, e.ThemeId });

                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivityThemes)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityThemes_Activity");

                entity.HasOne(d => d.Theme)
                    .WithMany(p => p.ActivityThemes)
                    .HasForeignKey(d => d.ThemeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityThemes_Theme");
            });

            modelBuilder.Entity<ApplicationSetting>(entity =>
            {
                entity.Property(e => e.SettingName)
                    .IsUnicode(false)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<Beat>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Blob>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Carousel>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<CarouselSlide>(entity =>
            {
                entity.HasKey(e => new { e.CarouselId, e.SlideId });

                entity.HasIndex(e => new { e.CarouselId, e.SlideId, e.SortIndex })
                    .HasName("UX_CarouselSlide_SortIndex")
                    .IsUnique();

                entity.HasOne(d => d.Carousel)
                    .WithMany(p => p.CarouselSlide)
                    .HasForeignKey(d => d.CarouselId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CarouselSlide_Carousel");

                entity.HasOne(d => d.Slide)
                    .WithMany(p => p.CarouselSlide)
                    .HasForeignKey(d => d.SlideId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CarouselSlide_Slide");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.SortOrder).HasDefaultValueSql("((0))");

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<CommunicationContact>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.MinistryShortName).IsUnicode(false);

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Ministry)
                    .WithMany(p => p.CommunicationContact)
                    .HasForeignKey(d => d.MinistryId)
                    .HasConstraintName("FK_CommunicationContact_Ministry");

                entity.HasOne(d => d.SystemUser)
                    .WithMany(p => p.CommunicationContact)
                    .HasForeignKey(d => d.SystemUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunicationContact_SystemUser_SystemUser");
            });

            modelBuilder.Entity<CommunicationMaterial>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasIndex(e => e.CompanyName)
                    .HasName("FK_Company_UniqueActiveOutletName")
                    .IsUnique()
                    .HasFilter("([IsActive]=(1))");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CirculationDescription).IsUnicode(false);

                entity.Property(e => e.CompanyDescription).IsUnicode(false);

                entity.Property(e => e.CompanyName).IsUnicode(false);

                entity.Property(e => e.Deadlines).IsUnicode(false);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.KeyPrograms).IsUnicode(false);

                entity.Property(e => e.RecordEditedBy).IsUnicode(false);

                entity.HasOne(d => d.ParentCompany)
                    .WithMany(p => p.InverseParentCompany)
                    .HasForeignKey(d => d.ParentCompanyId)
                    .HasConstraintName("FK_Company_Company");

                entity.HasOne(d => d.PrintCategory)
                    .WithMany(p => p.Company)
                    .HasForeignKey(d => d.PrintCategoryId)
                    .HasConstraintName("FK_Company_PrintCategory");

                entity.HasOne(d => d.PublicationFrequency)
                    .WithMany(p => p.Company)
                    .HasForeignKey(d => d.PublicationFrequencyId)
                    .HasConstraintName("FK_Company_PublicationFrequency");
            });

            modelBuilder.Entity<CompanyAddress>(entity =>
            {
                entity.HasIndex(e => e.AddressType)
                    .HasName("IX_AddressType");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CityName).IsUnicode(false);

                entity.Property(e => e.PostalZipCode).IsUnicode(false);

                entity.Property(e => e.ProvStateName).IsUnicode(false);

                entity.Property(e => e.StreetAddress).IsUnicode(false);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.CompanyAddress)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_Address_City");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyAddress)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Address_Company");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.CompanyAddress)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Address_Country");

                entity.HasOne(d => d.ProvState)
                    .WithMany(p => p.CompanyAddress)
                    .HasForeignKey(d => d.ProvStateId)
                    .HasConstraintName("FK_Address_ProvState");
            });

            modelBuilder.Entity<CompanyDistribution>(entity =>
            {
                entity.HasKey(e => new { e.CompanyId, e.DistributionId });

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyDistribution)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyDistribution_Company");

                entity.HasOne(d => d.Distribution)
                    .WithMany(p => p.CompanyDistribution)
                    .HasForeignKey(d => d.DistributionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyDistribution_Distribution");
            });

            modelBuilder.Entity<CompanyElectoralDistrict>(entity =>
            {
                entity.HasKey(e => new { e.CompanyId, e.DistrictId });

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyElectoralDistrict)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyElectoralDistrict_Company");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.CompanyElectoralDistrict)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyElectoralDistrict_ElectoralDistrict");
            });

            modelBuilder.Entity<CompanyEthnicity>(entity =>
            {
                entity.HasKey(e => new { e.CompanyId, e.EthnicityId });

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyEthnicity)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyEthnicity_Company");

                entity.HasOne(d => d.Ethnicity)
                    .WithMany(p => p.CompanyEthnicity)
                    .HasForeignKey(d => d.EthnicityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyEthnicity_Ethnicity");
            });

            modelBuilder.Entity<CompanyLanguage>(entity =>
            {
                entity.HasKey(e => new { e.CompanyId, e.LanguageId });

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyLanguage)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyLanguage_Company");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.CompanyLanguage)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyLanguage_Language");
            });

            modelBuilder.Entity<CompanyMediaDesk>(entity =>
            {
                entity.HasKey(e => new { e.CompanyId, e.MediaDeskId });

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyMediaDesk)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyMediaDesk_Company");

                entity.HasOne(d => d.MediaDesk)
                    .WithMany(p => p.CompanyMediaDesk)
                    .HasForeignKey(d => d.MediaDeskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyMediaDesk_MediaDesk");
            });

            modelBuilder.Entity<CompanyMediaPartner>(entity =>
            {
                entity.HasKey(e => new { e.CompanyId, e.MediaPartnerId });

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyMediaPartnerCompany)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyMediaPartner_Company");

                entity.HasOne(d => d.MediaPartner)
                    .WithMany(p => p.CompanyMediaPartnerMediaPartner)
                    .HasForeignKey(d => d.MediaPartnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyMediaPartner_MediaPartner");
            });

            modelBuilder.Entity<CompanyMediaType>(entity =>
            {
                entity.HasKey(e => new { e.CompanyId, e.MediaTypeId });

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyMediaType)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyMediaType_Company");

                entity.HasOne(d => d.MediaType)
                    .WithMany(p => p.CompanyMediaType)
                    .HasForeignKey(d => d.MediaTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyMediaType_MediaType");
            });

            modelBuilder.Entity<CompanyPhoneNumber>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.PhoneNumber).IsUnicode(false);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyPhoneNumber)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyPhoneNumber_Company");

                entity.HasOne(d => d.PhoneType)
                    .WithMany(p => p.CompanyPhoneNumber)
                    .HasForeignKey(d => d.PhoneTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyPhoneNumber_PhoneType");
            });

            modelBuilder.Entity<CompanyPublicationDays>(entity =>
            {
                entity.HasKey(e => new { e.CompanyId, e.PublicationDaysId });

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyPublicationDays)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyPublicationDays_Company");

                entity.HasOne(d => d.PublicationDays)
                    .WithMany(p => p.CompanyPublicationDays)
                    .HasForeignKey(d => d.PublicationDaysId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyPublicationDays_PublicationDays");
            });

            modelBuilder.Entity<CompanyRegion>(entity =>
            {
                entity.HasKey(e => new { e.CompanyId, e.RegionId });

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyRegion)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyRegion_Company");

                entity.HasOne(d => d.Region)
                    .WithMany(p => p.CompanyRegion)
                    .HasForeignKey(d => d.RegionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyRegion_Region");
            });

            modelBuilder.Entity<CompanySector>(entity =>
            {
                entity.HasKey(e => new { e.CompanyId, e.SectorId });

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanySector)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanySector_Company");

                entity.HasOne(d => d.Sector)
                    .WithMany(p => p.CompanySector)
                    .HasForeignKey(d => d.SectorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanySector_Sector");
            });

            modelBuilder.Entity<CompanySpecialtyPublication>(entity =>
            {
                entity.HasKey(e => new { e.CompanyId, e.SpecialtyPublicationId });

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanySpecialtyPublication)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanySpecialtyPublication_Company");

                entity.HasOne(d => d.SpecialtyPublication)
                    .WithMany(p => p.CompanySpecialtyPublication)
                    .HasForeignKey(d => d.SpecialtyPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanySpecialtyPublication_SpecialtyPublication");
            });

            modelBuilder.Entity<CompanyWebAddress>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.WebAddress).IsUnicode(false);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyWebAddress)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyWebAddress_Company");

                entity.HasOne(d => d.WebAddressType)
                    .WithMany(p => p.CompanyWebAddress)
                    .HasForeignKey(d => d.WebAddressTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyWebAddress_WebAddressType");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasIndex(e => new { e.FirstName, e.LastName })
                    .HasName("FK_Contact_UniqueActiveName")
                    .IsUnique()
                    .HasFilter("([IsActive]=(1))");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.MinisterialJobTitle)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.MinisterialJobTitleId)
                    .HasConstraintName("FK_Contact_MinisterialJobTitle");

                entity.HasOne(d => d.Ministry)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.MinistryId)
                    .HasConstraintName("FK_Contact_Ministry");

                entity.HasOne(d => d.Mlaassignment)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.MlaassignmentId)
                    .HasConstraintName("FK_Contact_ElectoralDistrict");
            });

            modelBuilder.Entity<ContactAddress>(entity =>
            {
                entity.HasIndex(e => e.AddressType)
                    .HasName("IX_AddressType");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CityName).IsUnicode(false);

                entity.Property(e => e.PostalZipCode).IsUnicode(false);

                entity.Property(e => e.ProvStateName).IsUnicode(false);

                entity.Property(e => e.StreetAddress).IsUnicode(false);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.ContactAddress)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_ContactAddress_City");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ContactAddress)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Address_Contact");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.ContactAddress)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactAddress_Country");

                entity.HasOne(d => d.ProvState)
                    .WithMany(p => p.ContactAddress)
                    .HasForeignKey(d => d.ProvStateId)
                    .HasConstraintName("FK_ContactAddress_ProvState");
            });

            modelBuilder.Entity<ContactBeat>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Beat)
                    .WithMany(p => p.ContactBeat)
                    .HasForeignKey(d => d.BeatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactBeat_Beat");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.ContactBeat)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactBeat_Company");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ContactBeat)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactBeat_ContactBeat");
            });

            modelBuilder.Entity<ContactCity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CityName).IsUnicode(false);
            });

            modelBuilder.Entity<ContactElectoralDistrict>(entity =>
            {
                entity.HasKey(e => new { e.ContactId, e.DistrictId });

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ContactElectoralDistrict)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactElectoralDistrict_Contact");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.ContactElectoralDistrict)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactElectoralDistrict_ElectoralDistrict");
            });

            modelBuilder.Entity<ContactMediaJobTitle>(entity =>
            {
                entity.HasIndex(e => new { e.ContactId, e.CompanyId })
                    .HasName("FK_ContactMediaJobTitle_1JobPerOutlet")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.ContactMediaJobTitle)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactMediaJobTitle_Company");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ContactMediaJobTitle)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactMediaJobTitle_Contact");

                entity.HasOne(d => d.MediaJobTitle)
                    .WithMany(p => p.ContactMediaJobTitle)
                    .HasForeignKey(d => d.MediaJobTitleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactMediaJobTitle_MediaJobTitle");
            });

            modelBuilder.Entity<ContactPhoneNumber>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.PhoneNumber).IsUnicode(false);

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ContactPhoneNumber)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactPhoneNumber_Contact");

                entity.HasOne(d => d.PhoneType)
                    .WithMany(p => p.ContactPhoneNumber)
                    .HasForeignKey(d => d.PhoneTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactPhoneNumber_PhoneType");
            });

            modelBuilder.Entity<ContactRegion>(entity =>
            {
                entity.HasKey(e => new { e.ContactId, e.RegionId });

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ContactRegion)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactRegion_Contact");

                entity.HasOne(d => d.Region)
                    .WithMany(p => p.ContactRegion)
                    .HasForeignKey(d => d.RegionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactRegion_Region");
            });

            modelBuilder.Entity<ContactSector>(entity =>
            {
                entity.HasKey(e => new { e.ContactId, e.SectorId });

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ContactSector)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactSector_Contact");

                entity.HasOne(d => d.Sector)
                    .WithMany(p => p.ContactSector)
                    .HasForeignKey(d => d.SectorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactSector_Sector");
            });

            modelBuilder.Entity<ContactWebAddress>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.WebAddress).IsUnicode(false);

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ContactWebAddress)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactWebAddress_Contact");

                entity.HasOne(d => d.WebAddressType)
                    .WithMany(p => p.ContactWebAddress)
                    .HasForeignKey(d => d.WebAddressTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactWebAddress_WebAddressType");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CountryAbbrev).IsUnicode(false);

                entity.Property(e => e.CountryName).IsUnicode(false);
            });

            modelBuilder.Entity<Distribution>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DistributionName).IsUnicode(false);
            });

            modelBuilder.Entity<ElectoralDistrict>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DistrictName).IsUnicode(false);
            });

            modelBuilder.Entity<Ethnicity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EthnicityName).IsUnicode(false);
            });

            modelBuilder.Entity<EventPlanner>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<FavoriteActivity>(entity =>
            {
                entity.HasKey(e => new { e.ActivityId, e.SystemUserId });

                entity.HasIndex(e => e.ActivityId)
                    .HasName("IX_FavoriteActivity");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.FavoriteActivity)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FavoriteActivity_Activity");

                entity.HasOne(d => d.SystemUser)
                    .WithMany(p => p.FavoriteActivity)
                    .HasForeignKey(d => d.SystemUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FavoriteActivity_SystemUser");
            });

            modelBuilder.Entity<GovernmentRepresentative>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<Initiative>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<Keyword>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name).IsUnicode(false);
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("UK_Language_Name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsUnicode(false);
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.Log)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Log_Activity");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.LogLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("FK_Log_SystemUser_UpdatedBy");
            });

            modelBuilder.Entity<MediaDesk>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.MediaDeskName).IsUnicode(false);
            });

            modelBuilder.Entity<MediaDistributionList>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<MediaJobTitle>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.MediaJobTitleName).IsUnicode(false);
            });

            modelBuilder.Entity<MediaLanguage>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.LanguageName).IsUnicode(false);
            });

            modelBuilder.Entity<MediaRequest>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RequestContent).HasDefaultValueSql("('')");

                entity.Property(e => e.RequestTopic).HasDefaultValueSql("('')");

                entity.Property(e => e.Response).HasDefaultValueSql("('')");

                entity.HasOne(d => d.CreatedBy)
                    .WithMany(p => p.MediaRequestCreatedBy)
                    .HasForeignKey(d => d.CreatedById)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MediaRequest_SystemUser_Created");

                entity.HasOne(d => d.LeadMinistry)
                    .WithMany(p => p.MediaRequestLeadMinistry)
                    .HasForeignKey(d => d.LeadMinistryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MediaRequest_Ministry");

                entity.HasOne(d => d.ModifiedBy)
                    .WithMany(p => p.MediaRequestModifiedBy)
                    .HasForeignKey(d => d.ModifiedById)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MediaRequest_SystemUser_Modified");

                entity.HasOne(d => d.RequestParent)
                    .WithMany(p => p.InverseRequestParent)
                    .HasForeignKey(d => d.RequestParentId)
                    .HasConstraintName("FK_MediaRequest_Parent");

                entity.HasOne(d => d.Resolution)
                    .WithMany(p => p.MediaRequest)
                    .HasForeignKey(d => d.ResolutionId)
                    .HasConstraintName("FK_MediaRequest_MediaRequestResolution");

                entity.HasOne(d => d.ResponsibleUser)
                    .WithMany(p => p.MediaRequestResponsibleUser)
                    .HasForeignKey(d => d.ResponsibleUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MediaRequest_SystemUser");

                entity.HasOne(d => d.TakeOverRequestMinistry)
                    .WithMany(p => p.MediaRequestTakeOverRequestMinistry)
                    .HasForeignKey(d => d.TakeOverRequestMinistryId)
                    .HasConstraintName("FK_MediaRequest_TakeOverRequestMinistry");
            });

            modelBuilder.Entity<MediaRequestContact>(entity =>
            {
                entity.HasKey(e => new { e.MediaRequestId, e.ContactId });

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.MediaRequestContact)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MediaRequestContact_Company");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.MediaRequestContact)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MediaRequestContact_Contact");

                entity.HasOne(d => d.MediaRequest)
                    .WithMany(p => p.MediaRequestContact)
                    .HasForeignKey(d => d.MediaRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MediaRequestContact_MediaRequest");
            });

            modelBuilder.Entity<MediaRequestResolution>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<MediaRequestSharedMinistry>(entity =>
            {
                entity.HasKey(e => new { e.MediaRequestId, e.MinistryId });

                entity.HasOne(d => d.MediaRequest)
                    .WithMany(p => p.MediaRequestSharedMinistry)
                    .HasForeignKey(d => d.MediaRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MediaRequestSharedMinistry_MediaRequest");

                entity.HasOne(d => d.Ministry)
                    .WithMany(p => p.MediaRequestSharedMinistry)
                    .HasForeignKey(d => d.MinistryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MediaRequestSharedMinistry_Ministry");
            });

            modelBuilder.Entity<MediaType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.MediaTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Title).IsUnicode(false);
            });

            modelBuilder.Entity<MinisterialJobTitle>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.MinisterialJobTitleName).IsUnicode(false);
            });

            modelBuilder.Entity<Ministry>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => e.Abbreviation)
                    .HasName("UK_Ministry_Abbreviation")
                    .IsUnique();

                entity.HasIndex(e => e.DisplayName)
                    .HasName("UK_MinistryLanguage_DisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.Key)
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.WeekendContactNumber).IsUnicode(false);

                entity.HasOne(d => d.ContactUser)
                    .WithMany(p => p.MinistryContactUser)
                    .HasForeignKey(d => d.ContactUserId)
                    .HasConstraintName("FK_Ministry_ContactSystemUser");

                entity.HasOne(d => d.EodLastRunUser)
                    .WithMany(p => p.MinistryEodLastRunUser)
                    .HasForeignKey(d => d.EodLastRunUserId)
                    .HasConstraintName("FK_Ministry_EodLastRunSystemUser");

                entity.HasOne(d => d.FeatureRelease)
                    .WithMany(p => p.MinistryFeatureRelease)
                    .HasForeignKey(d => d.FeatureReleaseId)
                    .HasConstraintName("FK_Ministry_FeatureRelease");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Ministry_Parent");

                entity.HasOne(d => d.SecondContactUser)
                    .WithMany(p => p.MinistrySecondContactUser)
                    .HasForeignKey(d => d.SecondContactUserId)
                    .HasConstraintName("FK_Ministry_SecondContactSystemUser");

                entity.HasOne(d => d.TopRelease)
                    .WithMany(p => p.MinistryTopRelease)
                    .HasForeignKey(d => d.TopReleaseId)
                    .HasConstraintName("FK_Ministry_TopRelease");
            });

            modelBuilder.Entity<MinistryLanguage>(entity =>
            {
                entity.HasKey(e => new { e.MinistryId, e.LanguageId });

                entity.HasIndex(e => new { e.LanguageId, e.Name })
                    .HasName("UK_MinistryLanguage_LanguageName")
                    .IsUnique();

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.MinistryLanguage)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MinistryLanguage_Language");

                entity.HasOne(d => d.Ministry)
                    .WithMany(p => p.MinistryLanguage)
                    .HasForeignKey(d => d.MinistryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MinistryLanguage_Ministry");
            });

            modelBuilder.Entity<MinistryNewsletter>(entity =>
            {
                entity.HasKey(e => new { e.MinistryId, e.NewsletterId });

                entity.HasIndex(e => new { e.MinistryId, e.NewsletterId })
                    .HasName("IX_MinistryNewsletter_MinistryNewsletter")
                    .IsUnique();

                entity.HasOne(d => d.Ministry)
                    .WithMany(p => p.MinistryNewsletter)
                    .HasForeignKey(d => d.MinistryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MinistryNewsletter_Ministry");
            });

            modelBuilder.Entity<MinistrySector>(entity =>
            {
                entity.HasKey(e => new { e.MinistryId, e.SectorId });

                entity.HasOne(d => d.Ministry)
                    .WithMany(p => p.MinistrySector)
                    .HasForeignKey(d => d.MinistryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MinistrySector_Ministry");

                entity.HasOne(d => d.Sector)
                    .WithMany(p => p.MinistrySector)
                    .HasForeignKey(d => d.SectorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MinistrySector_Sector");
            });

            modelBuilder.Entity<MinistryService>(entity =>
            {
                entity.HasKey(e => new { e.MinistryId, e.SortIndex });

                entity.Property(e => e.LinkUrl).IsUnicode(false);

                entity.HasOne(d => d.Ministry)
                    .WithMany(p => p.MinistryService)
                    .HasForeignKey(d => d.MinistryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MinistryService_Ministry");
            });

            modelBuilder.Entity<MinistryTopic>(entity =>
            {
                entity.HasKey(e => new { e.MinistryId, e.SortIndex });

                entity.Property(e => e.LinkUrl).IsUnicode(false);

                entity.HasOne(d => d.Ministry)
                    .WithMany(p => p.MinistryTopic)
                    .HasForeignKey(d => d.MinistryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MinistryTopic_Ministry");
            });

            modelBuilder.Entity<NewsFeed>(entity =>
            {
                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.NewsFeed)
                    .HasForeignKey(d => d.ActivityId)
                    .HasConstraintName("FK_NewsFeed_Activity");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.NewsFeedLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("FK_NewsFeed_SystemUser_UpdatedBy");

                entity.HasOne(d => d.Ministry)
                    .WithMany(p => p.NewsFeed)
                    .HasForeignKey(d => d.MinistryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NewsFeed_Ministry");
            });

            modelBuilder.Entity<NewsRegion>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.RegionName).IsUnicode(false);
            });

            modelBuilder.Entity<NewsRelease>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => e.Reference)
                    .IsUnique()
                    .HasFilter("([Reference]<>'')");

                entity.HasIndex(e => new { e.ReleaseType, e.Key })
                    .HasName("IX_NewsRelease_Key")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.HasIndex(e => new { e.Year, e.YearRelease })
                    .HasName("IX_NewsRelease_YearRelease")
                    .HasFilter("([Year] IS NOT NULL)");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AssetUrl).IsUnicode(false);

                entity.Property(e => e.AtomId).IsUnicode(false);

                entity.Property(e => e.Key).IsUnicode(false);

                entity.Property(e => e.RedirectUrl)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Reference).IsUnicode(false);

                entity.Property(e => e.Timestamp).HasDefaultValueSql("('1900-01-01')");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.NewsRelease)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NewsRelease_NewsCollection");

                entity.HasOne(d => d.Ministry)
                    .WithMany(p => p.NewsRelease)
                    .HasForeignKey(d => d.MinistryId)
                    .HasConstraintName("FK_NewsRelease_Ministry");
            });

            modelBuilder.Entity<NewsReleaseCollection>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("UK_NewsCollection_Name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsUnicode(false);
            });

            modelBuilder.Entity<NewsReleaseDocument>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Release)
                    .WithMany(p => p.NewsReleaseDocument)
                    .HasForeignKey(d => d.ReleaseId)
                    .HasConstraintName("FK_Document_Release");
            });

            modelBuilder.Entity<NewsReleaseDocumentContact>(entity =>
            {
                entity.HasKey(e => new { e.DocumentId, e.LanguageId, e.SortIndex });

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.NewsReleaseDocumentContact)
                    .HasForeignKey(d => d.DocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DocumentLanguageContact_Document");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.NewsReleaseDocumentContact)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DocumentLanguageContact_Language");

                entity.HasOne(d => d.NewsReleaseDocumentLanguage)
                    .WithMany(p => p.NewsReleaseDocumentContact)
                    .HasForeignKey(d => new { d.DocumentId, d.LanguageId })
                    .HasConstraintName("FK_NewsDocumentLanguageContact_NewsDocumentLanguage");
            });

            modelBuilder.Entity<NewsReleaseDocumentLanguage>(entity =>
            {
                entity.HasKey(e => new { e.DocumentId, e.LanguageId });

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.NewsReleaseDocumentLanguage)
                    .HasForeignKey(d => d.DocumentId)
                    .HasConstraintName("FK_DocumentLanguage_Document");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.NewsReleaseDocumentLanguage)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DocumentLanguage_Language");

                entity.HasOne(d => d.NewsReleaseImageLanguage)
                    .WithMany(p => p.NewsReleaseDocumentLanguage)
                    .HasForeignKey(d => new { d.PageImageId, d.LanguageId })
                    .HasConstraintName("FK_DocumentLanguage_Image");
            });

            modelBuilder.Entity<NewsReleaseHistory>(entity =>
            {
                entity.HasKey(e => new { e.ReleaseId, e.PublishDateTime, e.MimeType });

                entity.Property(e => e.MimeType).IsUnicode(false);

                entity.HasOne(d => d.Blob)
                    .WithMany(p => p.NewsReleaseHistory)
                    .HasForeignKey(d => d.BlobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NewsReleaseHistory_Blob");

                entity.HasOne(d => d.Release)
                    .WithMany(p => p.NewsReleaseHistory)
                    .HasForeignKey(d => d.ReleaseId)
                    .HasConstraintName("FK_NewsReleaseHistory_NewsRelease");
            });

            modelBuilder.Entity<NewsReleaseImage>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("UK_NewsReleaseImage_Name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.MimeType).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.HasOne(d => d.Blob)
                    .WithMany(p => p.NewsReleaseImage)
                    .HasForeignKey(d => d.BlobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NewsReleaseImage_Blob");
            });

            modelBuilder.Entity<NewsReleaseImageLanguage>(entity =>
            {
                entity.HasKey(e => new { e.ImageId, e.LanguageId });

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.NewsReleaseImageLanguage)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NewsReleaseImageLanguage_NewsReleaseImage");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.NewsReleaseImageLanguage)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NewsReleaseImageLanguage_Language");
            });

            modelBuilder.Entity<NewsReleaseLanguage>(entity =>
            {
                entity.HasKey(e => new { e.ReleaseId, e.LanguageId });

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.NewsReleaseLanguage)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReleaseLanguage_Language");

                entity.HasOne(d => d.Release)
                    .WithMany(p => p.NewsReleaseLanguage)
                    .HasForeignKey(d => d.ReleaseId)
                    .HasConstraintName("FK_ReleaseLanguage_Release");
            });

            modelBuilder.Entity<NewsReleaseLog>(entity =>
            {
                entity.HasOne(d => d.Release)
                    .WithMany(p => p.NewsReleaseLog)
                    .HasForeignKey(d => d.ReleaseId)
                    .HasConstraintName("FK_NewsReleaseLog_NewsRelease");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.NewsReleaseLog)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_NewsReleaseLog_User");
            });

            modelBuilder.Entity<NewsReleaseMediaDistribution>(entity =>
            {
                entity.HasKey(e => new { e.ReleaseId, e.MediaDistributionListId });

                entity.HasOne(d => d.MediaDistributionList)
                    .WithMany(p => p.NewsReleaseMediaDistribution)
                    .HasForeignKey(d => d.MediaDistributionListId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NewsReleaseMediaDistribution_List");

                entity.HasOne(d => d.Release)
                    .WithMany(p => p.NewsReleaseMediaDistribution)
                    .HasForeignKey(d => d.ReleaseId)
                    .HasConstraintName("FK_NewsReleaseMediaDistribution_NewsRelease");
            });

            modelBuilder.Entity<NewsReleaseMinistry>(entity =>
            {
                entity.HasKey(e => new { e.ReleaseId, e.MinistryId });

                entity.HasOne(d => d.Ministry)
                    .WithMany(p => p.NewsReleaseMinistry)
                    .HasForeignKey(d => d.MinistryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NewsReleaseMinistry_Ministry");

                entity.HasOne(d => d.Release)
                    .WithMany(p => p.NewsReleaseMinistry)
                    .HasForeignKey(d => d.ReleaseId)
                    .HasConstraintName("FK_NewsReleaseMinistry_NewsRelease");
            });

            modelBuilder.Entity<NewsReleaseSector>(entity =>
            {
                entity.HasKey(e => new { e.ReleaseId, e.SectorId });

                entity.HasOne(d => d.Release)
                    .WithMany(p => p.NewsReleaseSector)
                    .HasForeignKey(d => d.ReleaseId)
                    .HasConstraintName("FK_NewsReleaseSector_NewsRelease");

                entity.HasOne(d => d.Sector)
                    .WithMany(p => p.NewsReleaseSector)
                    .HasForeignKey(d => d.SectorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NewsReleaseSector_Sector");
            });

            modelBuilder.Entity<NewsReleaseService>(entity =>
            {
                entity.HasKey(e => new { e.ReleaseId, e.ServiceId });

                entity.HasOne(d => d.Release)
                    .WithMany(p => p.NewsReleaseService)
                    .HasForeignKey(d => d.ReleaseId)
                    .HasConstraintName("FK_NewsReleaseService_NewsRelease");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.NewsReleaseService)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NewsReleaseService_Service");
            });

            modelBuilder.Entity<NewsReleaseTag>(entity =>
            {
                entity.HasKey(e => new { e.ReleaseId, e.TagId });

                entity.HasOne(d => d.Release)
                    .WithMany(p => p.NewsReleaseTag)
                    .HasForeignKey(d => d.ReleaseId)
                    .HasConstraintName("FK_NewsReleaseTag_NewsRelease");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.NewsReleaseTag)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NewsReleaseTag_Tag");
            });

            modelBuilder.Entity<NewsReleaseTheme>(entity =>
            {
                entity.HasKey(e => new { e.ReleaseId, e.ThemeId });

                entity.HasOne(d => d.Release)
                    .WithMany(p => p.NewsReleaseTheme)
                    .HasForeignKey(d => d.ReleaseId)
                    .HasConstraintName("FK_NewsReleaseTheme_NewsRelease");

                entity.HasOne(d => d.Theme)
                    .WithMany(p => p.NewsReleaseTheme)
                    .HasForeignKey(d => d.ThemeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NewsReleaseTheme_Theme");
            });

            modelBuilder.Entity<NewsReleaseType>(entity =>
            {
                entity.HasKey(e => new { e.PageTitle, e.LanguageId });

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.NewsReleaseType)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NewsReleaseType_Language");

                entity.HasOne(d => d.PageImage)
                    .WithMany(p => p.NewsReleaseType)
                    .HasForeignKey(d => d.PageImageId)
                    .HasConstraintName("FK_NewsReleaseType_NewsReleaseImage");
            });

            modelBuilder.Entity<Nrdistribution>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.SortOrder).HasDefaultValueSql("((1))");

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<Nrorigin>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<PhoneType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.PhoneTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<PremierRequested>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<PrintCategory>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.PrintCategoryName).IsUnicode(false);
            });

            modelBuilder.Entity<Priority>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<ProvState>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ProvStateAbbrev).IsUnicode(false);

                entity.Property(e => e.ProvStateName).IsUnicode(false);
            });

            modelBuilder.Entity<PublicationDays>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.PublicationDaysName).IsUnicode(false);
            });

            modelBuilder.Entity<PublicationFrequency>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.PublicationFrequencyName).IsUnicode(false);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ReportName).IsUnicode(false);

                entity.Property(e => e.ReportOwner).IsUnicode(false);

                entity.Property(e => e.ReportQueryString).IsUnicode(false);
            });

            modelBuilder.Entity<ResourceLink>(entity =>
            {
                entity.Property(e => e.SortIndex).ValueGeneratedNever();

                entity.Property(e => e.LinkUrl).IsUnicode(false);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<Sector>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => e.Key)
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AudioUrl).IsUnicode(false);

                entity.Property(e => e.DisplayName).IsUnicode(false);

                entity.Property(e => e.FlickrUrl).IsUnicode(false);

                entity.Property(e => e.Key).IsUnicode(false);

                entity.Property(e => e.TwitterUsername).IsUnicode(false);

                entity.Property(e => e.YoutubeUrl).IsUnicode(false);

                entity.HasOne(d => d.FeatureRelease)
                    .WithMany(p => p.SectorFeatureRelease)
                    .HasForeignKey(d => d.FeatureReleaseId)
                    .HasConstraintName("FK_Sector_FeatureRelease");

                entity.HasOne(d => d.TopRelease)
                    .WithMany(p => p.SectorTopRelease)
                    .HasForeignKey(d => d.TopReleaseId)
                    .HasConstraintName("FK_Sector_TopRelease");
            });

            modelBuilder.Entity<SectorLanguage>(entity =>
            {
                entity.HasKey(e => new { e.SectorId, e.LanguageId });

                entity.HasIndex(e => e.Name)
                    .HasName("UK_SectorLanguage_Name")
                    .IsUnique();

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.SectorLanguage)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SectorLanguage_Language");

                entity.HasOne(d => d.Sector)
                    .WithMany(p => p.SectorLanguage)
                    .HasForeignKey(d => d.SectorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SectorLanguage_Sector");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DisplayName).IsUnicode(false);

                entity.Property(e => e.Key).IsUnicode(false);
            });

            modelBuilder.Entity<Slide>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SocialMediaPost>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SpecialtyPublication>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.SpecialtyPublicationName).IsUnicode(false);
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<SysConfig>(entity =>
            {
                entity.HasIndex(e => e.ConfigDataType)
                    .HasName("IX_sys_config_2");

                entity.HasIndex(e => e.ConfigKey)
                    .HasName("IX_sys_config");

                entity.HasIndex(e => e.ConfigValue)
                    .HasName("IX_sys_config_1");

                entity.Property(e => e.ConfigKey)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.ConfigDescription).IsUnicode(false);

                entity.Property(e => e.ConfigValue).IsUnicode(false);
            });

            modelBuilder.Entity<SysLog>(entity =>
            {
                entity.Property(e => e.Action).IsUnicode(false);

                entity.Property(e => e.EntityData).IsUnicode(false);

                entity.Property(e => e.EntityType).IsUnicode(false);

                entity.Property(e => e.EventData).IsUnicode(false);

                entity.Property(e => e.EventUser).IsUnicode(false);
            });

            modelBuilder.Entity<SystemUser>(entity =>
            {
                entity.HasIndex(e => e.RowGuid)
                    .HasName("UX_SystemUser_RowGuid")
                    .IsUnique();

                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.InverseLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("FK_SystemUser_SystemUser_UpdatedBy");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.SystemUser)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SystemUser_SystemUser_Role");
            });

            modelBuilder.Entity<SystemUserMinistry>(entity =>
            {
                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.SystemUserMinistryLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("FK_SystemUserMinistry_SystemUser_UpdatedBy");

                entity.HasOne(d => d.Ministry)
                    .WithMany(p => p.SystemUserMinistry)
                    .HasForeignKey(d => d.MinistryId)
                    .HasConstraintName("FK_SystemUserMinistry_Ministry");

                entity.HasOne(d => d.SystemUser)
                    .WithMany(p => p.SystemUserMinistrySystemUser)
                    .HasForeignKey(d => d.SystemUserId)
                    .HasConstraintName("FK_SystemUserMinistry_SystemUserId");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DisplayName).IsUnicode(false);

                entity.Property(e => e.Key).IsUnicode(false);
            });

            modelBuilder.Entity<Theme>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DisplayName).IsUnicode(false);

                entity.Property(e => e.Key).IsUnicode(false);

                entity.HasOne(d => d.FeatureRelease)
                    .WithMany(p => p.ThemeFeatureRelease)
                    .HasForeignKey(d => d.FeatureReleaseId)
                    .HasConstraintName("FK_Theme_FeatureRelease");

                entity.HasOne(d => d.TopRelease)
                    .WithMany(p => p.ThemeTopRelease)
                    .HasForeignKey(d => d.TopReleaseId)
                    .HasConstraintName("FK_Theme_TopRelease");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.DisplayName)
                    .HasName("UK_User_DisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.EmailAddress)
                    .HasName("UK_User_EmailAddress")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DisplayName).HasDefaultValueSql("('')");

                entity.Property(e => e.EmailAddress).HasDefaultValueSql("('')");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Videographer>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RowGuid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<WebAddressType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.WebAddressTypeName).IsUnicode(false);
            });
        }
    }
}
