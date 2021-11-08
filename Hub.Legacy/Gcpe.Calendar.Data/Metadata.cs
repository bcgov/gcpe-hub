//using System.Web.DynamicData;

namespace CorporateCalendar.Data
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public enum LookAheadSection { Issues_and_Reports = 1, Events_and_Speeches = 2, In_the_News = 3, Not_On_LA = 4 }

    [ScaffoldTable(false)]
    public partial class ActiveActivity
    {
        public bool IsPremierRequestedOrConfirmed
        {
            get
            {
                return PremierRequested == "Yes" || PremierRequested == "Premier TBC" || PremierRequested == "Premier Confirmed";
            }
        }
        public string CityOrOther
        {
            get
            {
                return City == "Other..." ? (OtherCity ?? "").Trim() : (City ?? "");
            }
        }
        public bool NotForLookAhead
        {
            get
            {
                return IsConfidential || (Categories != null && Categories.Contains("CONFIDENTIAL or EMBARGOED"));
            }
        }
        public bool IsForLongTermOutlook
        {
            get
            {
                return HqSection < 0;
            }
        }
    }

    [ScaffoldTable(false)]
    public partial class ActiveCommunicationContact
    {
    }

    [ScaffoldTable(false)]
    public partial class ActiveDistinctCommunicationContact
    {
    }

    [ScaffoldTable(false)]
    public partial class Activity
    {
    }


    [ScaffoldTable(false)]
    public partial class ActivityTheme
    {
    }

    [ScaffoldTable(false)]
    public partial class ActivityFile
    {
    }

    [ScaffoldTable(false)]
    public partial class FavoriteActivity
    {
    }

    [ScaffoldTable(false)]
    public partial class ActivityKeyword
    {
    }

    [ScaffoldTable(false)]
    public partial class ActivityService
    {
    }

    [ScaffoldTable(false)]
    public partial class ActivityInitiative
    {
    }


    [ScaffoldTable(false)]
    public partial class ActivityCategory
    {
    }

    [ScaffoldTable(false)]
    public partial class ActivityCommunicationMaterial
    {
    }

    [ScaffoldTable(false)]
    public partial class ActivityFiles
    {
    }

    [ScaffoldTable(false)]
    public partial class ActivityFilter
    {
    }

    [ScaffoldTable(false)]
    public partial class ActivityNROrigin
    {
    }

    [ScaffoldTable(false)]
    public partial class ActivitySector
    {
    }

    [ScaffoldTable(false)]
    public partial class ActivitySharedWith
    {
    }

    [ScaffoldTable(false)]
    public partial class ActivityTag
    {
    }

    [ScaffoldTable(false)]
    public partial class Priority
    {
    }

    [MetadataType(typeof(Category_MD))]
    [ScaffoldTable(true)]
    public partial class Category
    {
        public class Category_MD
        {
            [ScaffoldColumn(false)]
            public object ActivityCategories { get; set; }
        }
    }

    [MetadataType(typeof(City_MD))]
    [ScaffoldTable(true)]
    public partial class City
    {
        public class City_MD
        {
            [ScaffoldColumn(false)]
            public object Activities { get; set; }
        }
    }


    [MetadataType(typeof(CommunicationContact_MD))]
    [ScaffoldTable(true)]
    public partial class CommunicationContact
    {
        public override string ToString()
        {
            return this.MinistryShortName + "/" + this.Name;
        }

        public class CommunicationContact_MD
        {
            [ScaffoldColumn(false)]
            public object Activities { get; set; }
        }
    }

    [MetadataType(typeof(CommunicationMaterial_MD))]
    [ScaffoldTable(true)]
    public partial class CommunicationMaterial
    {
        public class CommunicationMaterial_MD
        {
            [ScaffoldColumn(false)]
            public object ActivityCommunicationMaterials { get; set; }
        }
    }

    [MetadataType(typeof(EventPlanner_MD))]
    [ScaffoldTable(true)]
    public partial class EventPlanner
    {
        public class EventPlanner_MD
        {
            [ScaffoldColumn(false)]
            public object Activities { get; set; }
        }
    }

    [MetadataType(typeof(GovernmentRepresentative_MD))]
    [ScaffoldTable(true)]
    public partial class GovernmentRepresentative
    {
        public class GovernmentRepresentative_MD
        {
            [ScaffoldColumn(false)]
            public object Activities { get; set; }

            [StringLength(84)]
            public object Description { get; set; }
        }
    }

    [ScaffoldTable(false)]
    public partial class Log
    {
    }

    [DisplayName("HQ Initiatives & Leads")]
    [MetadataType(typeof(Initiative_MD))]
    [ScaffoldTable(true)]
    public partial class Initiative
    {
        public class Initiative_MD
        {
            [ScaffoldColumn(false)]
            public object ActivityInitiatives { get; set; }
        }
    }

    [MetadataType(typeof(Ministry_MD))]
    [ScaffoldTable(true)]
    public partial class Ministry
    {
        public class Ministry_MD
        {
            [ScaffoldColumn(false)]
            public object MinisterEmail { get; set; }

            [Required(AllowEmptyStrings = true)]
            public object MinisterPhotoUrl { get; set; }

            [ScaffoldColumn(false)]
            public object MinisterPageHtml { get; set; }

            [ScaffoldColumn(false)]
            public object Id { get; set; }

            [ScaffoldColumn(false)]
            public object MinisterAddress { get; set; }

            [ScaffoldColumn(false)]
            public object MiscHtml { get; set; }

            [ScaffoldColumn(false)]
            public object MiscRightHtml { get; set; }

            [ScaffoldColumn(false)]
            public object TwitterUsername { get; set; }

            [Required(AllowEmptyStrings = true)]
            public object MinistryUrl { get; set; }

            [ScaffoldColumn(false)]
            public object FlickrUrl { get; set; }

            [ScaffoldColumn(false)]
            public object YoutubeUrl { get; set; }

            [ScaffoldColumn(false)]
            public object AudioUrl { get; set; }

            [ScaffoldColumn(false)]
            public object FacebookEmbedHtml { get; set; }

            [ScaffoldColumn(false)]
            public object YoutubeEmbedHtml { get; set; }

            [ScaffoldColumn(false)]
            public object AudioEmbedHtml { get; set; }

            [ScaffoldColumn(false)]
            public object Activities { get; set; }

            [ScaffoldColumn(false)]
            public object ActivitySharedWiths { get; set; }

            [ScaffoldColumn(false)]
            public object NewsFeeds { get; set; }

            [ScaffoldColumn(false)]
            public object SystemUserMinistries { get; set; }

            [ScaffoldColumn(false)]
            public object CommunicationContacts { get; set; }

            [ScaffoldColumn(false)]
            public object Ministries { get; set; }
            [ScaffoldColumn(false)]

            public object EodFinalizedDateTime { get; set; }

            [ScaffoldColumn(false)]
            public object EodLastRunDateTime { get; set; }

            [ScaffoldColumn(false)]
            public object EodLastRunUser { get; set; }

            [Required(AllowEmptyStrings = true)]
            public object WeekendContactNumber { get; set; }

            [Required(AllowEmptyStrings = true)]
            public object DisplayAdditionalName { get; set; }
        }
    }

    [ScaffoldTable(false)]
    public partial class NewsFeed
    {
    }

    [MetadataType(typeof(NRDistribution_MD))]
    [ScaffoldTable(true)]
    public partial class NRDistribution
    {
        public class NRDistribution_MD
        {
            [ScaffoldColumn(false)]
            public object Activities { get; set; }
        }
    }

    [MetadataType(typeof(NROrigin_MD))]
    [ScaffoldTable(true)]
    public partial class NROrigin
    {
        public class NROrigin_MD
        {
            [ScaffoldColumn(false)]
            public object ActivityNROrigins { get; set; }
        }
    }

    [MetadataType(typeof(PremierRequested_MD))]
    [ScaffoldTable(true)]
    public partial class PremierRequested
    {
        public class PremierRequested_MD
        {
            [ScaffoldColumn(false)]
            public object Activities { get; set; }
        }
    }

    [ScaffoldTable(true)]
    public partial class Role
    {
    }

    [MetadataType(typeof(Sector_MD))]
    [ScaffoldTable(true)]
    public partial class Sector
    {
        public class Sector_MD
        {
            [ScaffoldColumn(false)]
            public object MiscHtml { get; set; }

            [ScaffoldColumn(false)]
            public object MiscRightHtml { get; set; }

            [ScaffoldColumn(false)]
            public object FacebookEmbedHtml { get; set; }

            [ScaffoldColumn(false)]
            public object YoutubeEmbedHtml { get; set; }

            [ScaffoldColumn(false)]
            public object AudioEmbedHtml { get; set; }

            [ScaffoldColumn(false)]
            public object ActivitySectors { get; set; }
        }
    }

    [MetadataType(typeof(Status_MD))]
    [ScaffoldTable(true)]
    public partial class Status
    {
        public class Status_MD
        {
            [ScaffoldColumn(false)]
            public object Activities { get; set; }
            [ScaffoldColumn(false)]
            public object Activities1 { get; set; }
        }
    }

    [MetadataType(typeof(SystemUser_MD))]
    [ScaffoldTable(true)]
    public partial class SystemUser
    {
        public class SystemUser_MD
        {
            [ScaffoldColumn(false)]
            public object SystemUserMinistries { get; set; }
            [ScaffoldColumn(false)]
            public object CreatedBySystemUserMinistries { get; set; }
            [ScaffoldColumn(false)]
            public object LastUpdatedBySystemUserMinistries { get; set; }

            [ScaffoldColumn(false)]
            public object ActivitySharedWiths { get; set; }
            [ScaffoldColumn(false)]
            public object ActivitySharedWiths1 { get; set; }

            [ScaffoldColumn(false)]
            public object CommunicationContacts { get; set; }

            [ScaffoldColumn(false)]
            public object Logs { get; set; }
            [ScaffoldColumn(false)]
            public object Logs1 { get; set; }

            [ScaffoldColumn(false)]
            public object Activities { get; set; } // CreatedBy
            [ScaffoldColumn(false)]
            public object Activities1 { get; set; } //LastUpdatedBy

            [ScaffoldColumn(false)]
            public object FavoriteActivities { get; set; } //LastUpdatedBy

            [ScaffoldColumn(false)]
            public object ActivityCategories { get; set; } // CreatedBy
            [ScaffoldColumn(false)]
            public object ActivityCategories1 { get; set; } // LastUpdatedBy

            [ScaffoldColumn(false)]
            public object ActivityInitiatives { get; set; }
            [ScaffoldColumn(false)]
            public object ActivityInitiatives1 { get; set; }

            [ScaffoldColumn(false)]
            public object ActivityCommunicationMaterials { get; set; }
            [ScaffoldColumn(false)]
            public object ActivityCommunicationMaterials1 { get; set; }

            [ScaffoldColumn(false)]
            public object ActivityNROrigins { get; set; }
            [ScaffoldColumn(false)]
            public object ActivityNROrigins1 { get; set; }

            [ScaffoldColumn(false)]
            public object ActivitySectors { get; set; }
            [ScaffoldColumn(false)]
            public object ActivitySectors1 { get; set; }

            [ScaffoldColumn(false)]
            public object NewsFeeds { get; set; }
            [ScaffoldColumn(false)]
            public object NewsFeeds1 { get; set; }

            [ScaffoldColumn(false)]
            public object SystemUser1 { get; set; } // CreatedBy
            [ScaffoldColumn(false)]
            public object SystemUser2 { get; set; } // LastUpdatedBy

            [ScaffoldColumn(false)]
            public object SystemUsers { get; set; } // CreatedBy
            [ScaffoldColumn(false)]
            public object SystemUsers1 { get; set; } // LastUpdatedBy

            [ScaffoldColumn(false)]
            public object ActivityFilters { get; set; }
            [ScaffoldColumn(false)]
            public object ActivityFilters1 { get; set; }

            [ScaffoldColumn(false)]
            public object CreatedDateTime { get; set; }

            [ScaffoldColumn(false)]
            public object Ministries { get; set; }
            [ScaffoldColumn(false)]
            public object Ministries1 { get; set; }
            [ScaffoldColumn(false)]
            public object Ministries2 { get; set; }
        }

    }

    [ScaffoldTable(false)]
    public partial class SystemUserMinistry
    {
    }

    [DisplayName("HQ Tags")]
    [ScaffoldTable(true)]
    public partial class Keyword
    {
    }

    [DisplayName("News Subscribe")]
    [ScaffoldTable(true)]
    public partial class Tag
    {     
    }


    [MetadataType(typeof(Theme_MD))]
    [ScaffoldTable(true)]
    public partial class Theme
    {
        public class Theme_MD
        {
            [ScaffoldColumn(false)]
            public object Id { get; set; }

            [ScaffoldColumn(false)]
            public object ActivityThemes { get; set; }
        }
    }

    [MetadataType(typeof(Videographer_MD))]
    [ScaffoldTable(true)]
    public partial class Videographer
    {
        public class Videographer_MD
        {
            [ScaffoldColumn(false)]
            public object Activities { get; set; }
        }
    }




}
