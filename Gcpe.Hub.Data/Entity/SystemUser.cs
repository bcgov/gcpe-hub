using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("SystemUser", Schema = "calendar")]
    public partial class SystemUser
    {
        public SystemUser()
        {
            ActivityCategoriesCreatedByNavigation = new HashSet<ActivityCategories>();
            ActivityCategoriesLastUpdatedByNavigation = new HashSet<ActivityCategories>();
            ActivityCommunicationMaterialsCreatedByNavigation = new HashSet<ActivityCommunicationMaterials>();
            ActivityCommunicationMaterialsLastUpdatedByNavigation = new HashSet<ActivityCommunicationMaterials>();
            ActivityCreatedByNavigation = new HashSet<Activity>();
            ActivityFilterCreatedByNavigation = new HashSet<ActivityFilter>();
            ActivityFilterLastUpdatedByNavigation = new HashSet<ActivityFilter>();
            ActivityInitiativesCreatedByNavigation = new HashSet<ActivityInitiatives>();
            ActivityInitiativesLastUpdatedByNavigation = new HashSet<ActivityInitiatives>();
            ActivityLastUpdatedByNavigation = new HashSet<Activity>();
            ActivityNroriginsCreatedByNavigation = new HashSet<ActivityNrorigins>();
            ActivityNroriginsLastUpdatedByNavigation = new HashSet<ActivityNrorigins>();
            ActivitySectorsCreatedByNavigation = new HashSet<ActivitySectors>();
            ActivitySectorsLastUpdatedByNavigation = new HashSet<ActivitySectors>();
            ActivitySharedWithCreatedByNavigation = new HashSet<ActivitySharedWith>();
            ActivitySharedWithLastUpdatedByNavigation = new HashSet<ActivitySharedWith>();
            CommunicationContact = new HashSet<CommunicationContact>();
            FavoriteActivity = new HashSet<FavoriteActivity>();
            InverseCreatedByNavigation = new HashSet<SystemUser>();
            InverseLastUpdatedByNavigation = new HashSet<SystemUser>();
            LogCreatedByNavigation = new HashSet<Log>();
            LogLastUpdatedByNavigation = new HashSet<Log>();
            MediaRequestCreatedBy = new HashSet<MediaRequest>();
            MediaRequestModifiedBy = new HashSet<MediaRequest>();
            MediaRequestResponsibleUser = new HashSet<MediaRequest>();
            MinistryContactUser = new HashSet<Ministry>();
            MinistryEodLastRunUser = new HashSet<Ministry>();
            MinistrySecondContactUser = new HashSet<Ministry>();
            NewsFeedCreatedByNavigation = new HashSet<NewsFeed>();
            NewsFeedLastUpdatedByNavigation = new HashSet<NewsFeed>();
            SystemUserMinistryCreatedByNavigation = new HashSet<SystemUserMinistry>();
            SystemUserMinistryLastUpdatedByNavigation = new HashSet<SystemUserMinistry>();
            SystemUserMinistrySystemUser = new HashSet<SystemUserMinistry>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        public string Username { get; set; }
        public int RoleId { get; set; }
        [StringLength(50)]
        public string Description { get; set; }
        [StringLength(50)]
        public string FullName { get; set; }
        [StringLength(50)]
        public string DisplayName { get; set; }
        [StringLength(100)]
        public string JobTitle { get; set; }
        [StringLength(20)]
        public string PhoneNumber { get; set; }
        [StringLength(15)]
        public string MobileNumber { get; set; }
        [StringLength(50)]
        public string EmailAddress { get; set; }
        public int? FilterDisplayValue { get; set; }
        public bool IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateTime { get; set; }
        public int? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDateTime { get; set; }
        public int? LastUpdatedBy { get; set; }
        [Required]
        public byte[] TimeStamp { get; set; }
        public Guid RowGuid { get; set; }
        [StringLength(50)]
        public string HiddenColumns { get; set; }

        [ForeignKey("CreatedBy")]
        [InverseProperty("InverseCreatedByNavigation")]
        public SystemUser CreatedByNavigation { get; set; }
        [ForeignKey("LastUpdatedBy")]
        [InverseProperty("InverseLastUpdatedByNavigation")]
        public SystemUser LastUpdatedByNavigation { get; set; }
        [ForeignKey("RoleId")]
        [InverseProperty("SystemUser")]
        public Role Role { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public ICollection<ActivityCategories> ActivityCategoriesCreatedByNavigation { get; set; }
        [InverseProperty("LastUpdatedByNavigation")]
        public ICollection<ActivityCategories> ActivityCategoriesLastUpdatedByNavigation { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public ICollection<ActivityCommunicationMaterials> ActivityCommunicationMaterialsCreatedByNavigation { get; set; }
        [InverseProperty("LastUpdatedByNavigation")]
        public ICollection<ActivityCommunicationMaterials> ActivityCommunicationMaterialsLastUpdatedByNavigation { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public ICollection<Activity> ActivityCreatedByNavigation { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public ICollection<ActivityFilter> ActivityFilterCreatedByNavigation { get; set; }
        [InverseProperty("LastUpdatedByNavigation")]
        public ICollection<ActivityFilter> ActivityFilterLastUpdatedByNavigation { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public ICollection<ActivityInitiatives> ActivityInitiativesCreatedByNavigation { get; set; }
        [InverseProperty("LastUpdatedByNavigation")]
        public ICollection<ActivityInitiatives> ActivityInitiativesLastUpdatedByNavigation { get; set; }
        [InverseProperty("LastUpdatedByNavigation")]
        public ICollection<Activity> ActivityLastUpdatedByNavigation { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public ICollection<ActivityNrorigins> ActivityNroriginsCreatedByNavigation { get; set; }
        [InverseProperty("LastUpdatedByNavigation")]
        public ICollection<ActivityNrorigins> ActivityNroriginsLastUpdatedByNavigation { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public ICollection<ActivitySectors> ActivitySectorsCreatedByNavigation { get; set; }
        [InverseProperty("LastUpdatedByNavigation")]
        public ICollection<ActivitySectors> ActivitySectorsLastUpdatedByNavigation { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public ICollection<ActivitySharedWith> ActivitySharedWithCreatedByNavigation { get; set; }
        [InverseProperty("LastUpdatedByNavigation")]
        public ICollection<ActivitySharedWith> ActivitySharedWithLastUpdatedByNavigation { get; set; }
        [InverseProperty("SystemUser")]
        public ICollection<CommunicationContact> CommunicationContact { get; set; }
        [InverseProperty("SystemUser")]
        public ICollection<FavoriteActivity> FavoriteActivity { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public ICollection<SystemUser> InverseCreatedByNavigation { get; set; }
        [InverseProperty("LastUpdatedByNavigation")]
        public ICollection<SystemUser> InverseLastUpdatedByNavigation { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public ICollection<Log> LogCreatedByNavigation { get; set; }
        [InverseProperty("LastUpdatedByNavigation")]
        public ICollection<Log> LogLastUpdatedByNavigation { get; set; }
        [InverseProperty("CreatedBy")]
        public ICollection<MediaRequest> MediaRequestCreatedBy { get; set; }
        [InverseProperty("ModifiedBy")]
        public ICollection<MediaRequest> MediaRequestModifiedBy { get; set; }
        [InverseProperty("ResponsibleUser")]
        public ICollection<MediaRequest> MediaRequestResponsibleUser { get; set; }
        [InverseProperty("ContactUser")]
        public ICollection<Ministry> MinistryContactUser { get; set; }
        [InverseProperty("EodLastRunUser")]
        public ICollection<Ministry> MinistryEodLastRunUser { get; set; }
        [InverseProperty("SecondContactUser")]
        public ICollection<Ministry> MinistrySecondContactUser { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public ICollection<NewsFeed> NewsFeedCreatedByNavigation { get; set; }
        [InverseProperty("LastUpdatedByNavigation")]
        public ICollection<NewsFeed> NewsFeedLastUpdatedByNavigation { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public ICollection<SystemUserMinistry> SystemUserMinistryCreatedByNavigation { get; set; }
        [InverseProperty("LastUpdatedByNavigation")]
        public ICollection<SystemUserMinistry> SystemUserMinistryLastUpdatedByNavigation { get; set; }
        [InverseProperty("SystemUser")]
        public ICollection<SystemUserMinistry> SystemUserMinistrySystemUser { get; set; }
    }
}
