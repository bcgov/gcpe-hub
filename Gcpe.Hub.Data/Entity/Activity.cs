using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Activity", Schema = "calendar")]
    public partial class Activity
    {
        public Activity()
        {
            ActivityCategories = new HashSet<ActivityCategories>();
            ActivityCommunicationMaterials = new HashSet<ActivityCommunicationMaterials>();
            ActivityFiles = new HashSet<ActivityFiles>();
            ActivityInitiatives = new HashSet<ActivityInitiatives>();
            ActivityKeywords = new HashSet<ActivityKeywords>();
            ActivityNrorigins = new HashSet<ActivityNrorigins>();
            ActivitySectors = new HashSet<ActivitySectors>();
            ActivityServices = new HashSet<ActivityServices>();
            ActivitySharedWith = new HashSet<ActivitySharedWith>();
            ActivityThemes = new HashSet<ActivityThemes>();
            FavoriteActivity = new HashSet<FavoriteActivity>();
            Log = new HashSet<Log>();
            NewsFeed = new HashSet<NewsFeed>();
        }

        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartDateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDateTime { get; set; }
        [StringLength(70)]
        public string PotentialDates { get; set; }
        [Required]
        [StringLength(500)]
        public string Title { get; set; }
        [Required]
        [StringLength(500)]
        public string Details { get; set; }
        [Required]
        [StringLength(500)]
        public string Schedule { get; set; }
        [Required]
        [StringLength(500)]
        public string Significance { get; set; }
        [StringLength(500)]
        public string Strategy { get; set; }
        [StringLength(4000)]
        public string Comments { get; set; }
        [StringLength(2000)]
        public string HqComments { get; set; }
        [StringLength(100)]
        public string LeadOrganization { get; set; }
        [StringLength(150)]
        public string Venue { get; set; }
        public int? StatusId { get; set; }
        public int? HqStatusId { get; set; }
        public int HqSection { get; set; }
        [Column("NRDistributionId")]
        public int? NrdistributionId { get; set; }
        public int? PremierRequestedId { get; set; }
        public Guid? ContactMinistryId { get; set; }
        public int? GovernmentRepresentativeId { get; set; }
        public int? CommunicationContactId { get; set; }
        public int? EventPlannerId { get; set; }
        public int? VideographerId { get; set; }
        public int? CityId { get; set; }
        [StringLength(150)]
        public string OtherCity { get; set; }
        public bool IsActive { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsIssue { get; set; }
        public bool IsAllDay { get; set; }
        public bool IsAtLegislature { get; set; }
        public bool IsConfidential { get; set; }
        public bool IsCrossGovernment { get; set; }
        public bool IsMilestone { get; set; }
        public bool IsTitleNeedsReview { get; set; }
        public bool IsDetailsNeedsReview { get; set; }
        public bool IsRepresentativeNeedsReview { get; set; }
        public bool IsCityNeedsReview { get; set; }
        public bool IsStartDateNeedsReview { get; set; }
        public bool IsEndDateNeedsReview { get; set; }
        public bool IsCategoriesNeedsReview { get; set; }
        public bool IsCommMaterialsNeedsReview { get; set; }
        public bool IsActiveNeedsReview { get; set; }
        [Column("NRDateTime", TypeName = "datetime")]
        public DateTime? NrdateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateTime { get; set; }
        public int? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDateTime { get; set; }
        public int? LastUpdatedBy { get; set; }
        public byte[] TimeStamp { get; set; }
        public Guid? RowGuid { get; set; }

        [ForeignKey("CityId")]
        [InverseProperty("Activity")]
        public City City { get; set; }
        [ForeignKey("CommunicationContactId")]
        [InverseProperty("Activity")]
        public CommunicationContact CommunicationContact { get; set; }
        [ForeignKey("ContactMinistryId")]
        [InverseProperty("Activity")]
        public Ministry ContactMinistry { get; set; }
        [ForeignKey("CreatedBy")]
        [InverseProperty("ActivityCreatedByNavigation")]
        public SystemUser CreatedByNavigation { get; set; }
        [ForeignKey("EventPlannerId")]
        [InverseProperty("Activity")]
        public EventPlanner EventPlanner { get; set; }
        [ForeignKey("GovernmentRepresentativeId")]
        [InverseProperty("Activity")]
        public GovernmentRepresentative GovernmentRepresentative { get; set; }
        [ForeignKey("HqStatusId")]
        [InverseProperty("ActivityHqStatus")]
        public Status HqStatus { get; set; }
        [ForeignKey("LastUpdatedBy")]
        [InverseProperty("ActivityLastUpdatedByNavigation")]
        public SystemUser LastUpdatedByNavigation { get; set; }
        [ForeignKey("NrdistributionId")]
        [InverseProperty("Activity")]
        public Nrdistribution Nrdistribution { get; set; }
        [ForeignKey("PremierRequestedId")]
        [InverseProperty("Activity")]
        public PremierRequested PremierRequested { get; set; }
        [ForeignKey("StatusId")]
        [InverseProperty("ActivityStatus")]
        public Status Status { get; set; }
        [ForeignKey("VideographerId")]
        [InverseProperty("Activity")]
        public Videographer Videographer { get; set; }
        [InverseProperty("Activity")]
        public ICollection<ActivityCategories> ActivityCategories { get; set; }
        [InverseProperty("Activity")]
        public ICollection<ActivityCommunicationMaterials> ActivityCommunicationMaterials { get; set; }
        [InverseProperty("Activity")]
        public ICollection<ActivityFiles> ActivityFiles { get; set; }
        [InverseProperty("Activity")]
        public ICollection<ActivityInitiatives> ActivityInitiatives { get; set; }
        [InverseProperty("Activity")]
        public ICollection<ActivityKeywords> ActivityKeywords { get; set; }
        [InverseProperty("Activity")]
        public ICollection<ActivityNrorigins> ActivityNrorigins { get; set; }
        [InverseProperty("Activity")]
        public ICollection<ActivitySectors> ActivitySectors { get; set; }
        [InverseProperty("Activity")]
        public ICollection<ActivityServices> ActivityServices { get; set; }
        [InverseProperty("Activity")]
        public ICollection<ActivitySharedWith> ActivitySharedWith { get; set; }
        [InverseProperty("Activity")]
        public ICollection<ActivityThemes> ActivityThemes { get; set; }
        [InverseProperty("Activity")]
        public ICollection<FavoriteActivity> FavoriteActivity { get; set; }
        [InverseProperty("Activity")]
        public ICollection<Log> Log { get; set; }
        [InverseProperty("Activity")]
        public ICollection<NewsFeed> NewsFeed { get; set; }
    }
}
