using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class Ministry
    {
        public Ministry()
        {
            Activity = new HashSet<Activity>();
            ActivitySharedWith = new HashSet<ActivitySharedWith>();
            CommunicationContact = new HashSet<CommunicationContact>();
            Contact = new HashSet<Contact>();
            InverseParent = new HashSet<Ministry>();
            MediaRequestLeadMinistry = new HashSet<MediaRequest>();
            MediaRequestSharedMinistry = new HashSet<MediaRequestSharedMinistry>();
            MediaRequestTakeOverRequestMinistry = new HashSet<MediaRequest>();
            MinistryLanguage = new HashSet<MinistryLanguage>();
            MinistryNewsletter = new HashSet<MinistryNewsletter>();
            MinistrySector = new HashSet<MinistrySector>();
            MinistryService = new HashSet<MinistryService>();
            MinistryTopic = new HashSet<MinistryTopic>();
            NewsFeed = new HashSet<NewsFeed>();
            NewsRelease = new HashSet<NewsRelease>();
            NewsReleaseMinistry = new HashSet<NewsReleaseMinistry>();
            SystemUserMinistry = new HashSet<SystemUserMinistry>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Key { get; set; }
        public int SortOrder { get; set; }
        [Required]
        [StringLength(255)]
        public string DisplayName { get; set; }
        [Required]
        [StringLength(10)]
        public string Abbreviation { get; set; }
        public bool IsActive { get; set; }
        [Required]
        [StringLength(255)]
        public string MinisterEmail { get; set; }
        [StringLength(255)]
        public string MinisterPhotoUrl { get; set; }
        [Required]
        public string MinisterPageHtml { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Timestamp { get; set; }
        [Required]
        public string MiscHtml { get; set; }
        [Required]
        public string MiscRightHtml { get; set; }
        [Required]
        [StringLength(255)]
        public string TwitterUsername { get; set; }
        [Required]
        [StringLength(255)]
        public string FlickrUrl { get; set; }
        [Required]
        [StringLength(255)]
        public string YoutubeUrl { get; set; }
        [Required]
        [StringLength(255)]
        public string AudioUrl { get; set; }
        [Required]
        public string FacebookEmbedHtml { get; set; }
        [Required]
        public string YoutubeEmbedHtml { get; set; }
        [Required]
        public string AudioEmbedHtml { get; set; }
        public Guid? TopReleaseId { get; set; }
        public Guid? FeatureReleaseId { get; set; }
        [Required]
        [StringLength(255)]
        public string MinisterAddress { get; set; }
        [Required]
        [StringLength(255)]
        public string MinisterName { get; set; }
        [Required]
        public string MinisterSummary { get; set; }
        [StringLength(255)]
        public string MinistryUrl { get; set; }
        public Guid? ParentId { get; set; }
        public int? ContactUserId { get; set; }
        public int? SecondContactUserId { get; set; }
        [StringLength(20)]
        public string WeekendContactNumber { get; set; }
        public DateTimeOffset? EodFinalizedDateTime { get; set; }
        public int? EodLastRunUserId { get; set; }
        public DateTimeOffset? EodLastRunDateTime { get; set; }
        [StringLength(255)]
        public string DisplayAdditionalName { get; set; }

        [ForeignKey("ContactUserId")]
        [InverseProperty("MinistryContactUser")]
        public SystemUser ContactUser { get; set; }
        [ForeignKey("EodLastRunUserId")]
        [InverseProperty("MinistryEodLastRunUser")]
        public SystemUser EodLastRunUser { get; set; }
        [ForeignKey("FeatureReleaseId")]
        [InverseProperty("MinistryFeatureRelease")]
        public NewsRelease FeatureRelease { get; set; }
        [ForeignKey("ParentId")]
        [InverseProperty("InverseParent")]
        public Ministry Parent { get; set; }
        [ForeignKey("SecondContactUserId")]
        [InverseProperty("MinistrySecondContactUser")]
        public SystemUser SecondContactUser { get; set; }
        [ForeignKey("TopReleaseId")]
        [InverseProperty("MinistryTopRelease")]
        public NewsRelease TopRelease { get; set; }
        [InverseProperty("ContactMinistry")]
        public ICollection<Activity> Activity { get; set; }
        [InverseProperty("Ministry")]
        public ICollection<ActivitySharedWith> ActivitySharedWith { get; set; }
        [InverseProperty("Ministry")]
        public ICollection<CommunicationContact> CommunicationContact { get; set; }
        [InverseProperty("Ministry")]
        public ICollection<Contact> Contact { get; set; }
        [InverseProperty("Parent")]
        public ICollection<Ministry> InverseParent { get; set; }
        [InverseProperty("LeadMinistry")]
        public ICollection<MediaRequest> MediaRequestLeadMinistry { get; set; }
        [InverseProperty("Ministry")]
        public ICollection<MediaRequestSharedMinistry> MediaRequestSharedMinistry { get; set; }
        [InverseProperty("TakeOverRequestMinistry")]
        public ICollection<MediaRequest> MediaRequestTakeOverRequestMinistry { get; set; }
        [InverseProperty("Ministry")]
        public ICollection<MinistryLanguage> MinistryLanguage { get; set; }
        [InverseProperty("Ministry")]
        public ICollection<MinistryNewsletter> MinistryNewsletter { get; set; }
        [InverseProperty("Ministry")]
        public ICollection<MinistrySector> MinistrySector { get; set; }
        [InverseProperty("Ministry")]
        public ICollection<MinistryService> MinistryService { get; set; }
        [InverseProperty("Ministry")]
        public ICollection<MinistryTopic> MinistryTopic { get; set; }
        [InverseProperty("Ministry")]
        public ICollection<NewsFeed> NewsFeed { get; set; }
        [InverseProperty("Ministry")]
        public ICollection<NewsRelease> NewsRelease { get; set; }
        [InverseProperty("Ministry")]
        public ICollection<NewsReleaseMinistry> NewsReleaseMinistry { get; set; }
        [InverseProperty("Ministry")]
        public ICollection<SystemUserMinistry> SystemUserMinistry { get; set; }
    }
}
