using System.Collections.Generic;
using System.Linq;

namespace MediaRelationsDatabase
{
    public interface ISortableEntity
    {
        int SortOrder { get; set; }
    }

    public interface IDeletableEntity
    {
        bool IsActive { get; set; }
    }

    public partial class Beat : ISortableEntity { }

    public partial class City : ISortableEntity { }

    public partial class Contact : IDeletableEntity
    {
        public ContactWebAddress Email
        {
            get
            {
                return ContactWebAddresses.FirstOrDefault(x => x.WebAddressType.WebAddressTypeName == WebAddressType.Email);
            }
        }
        public ContactPhoneNumber CellPhone
        {
            get
            {
                return ContactPhoneNumbers.FirstOrDefault(x => x.PhoneType.PhoneTypeName == PhoneType.Cell);
            }
        }
    }

    public partial class Company : IDeletableEntity
    {
        public CompanyWebAddress Email
        {
            get
            {
                return CompanyWebAddresses.FirstOrDefault(x => x.WebAddressType.WebAddressTypeName == WebAddressType.Email);
            }
        }
        public CompanyPhoneNumber CellPhone
        {
            get
            {
                return CompanyPhoneNumbers.FirstOrDefault(x => x.PhoneType.PhoneTypeName == PhoneType.Cell);
            }
        }
        public CompanyPhoneNumber NewsDeskPhone
        {
            get
            {
                return CompanyPhoneNumbers.FirstOrDefault(x => x.PhoneType.PhoneTypeName == PhoneType.NewsDesk);
            }
        }
    }

    public partial class Country : ISortableEntity { }

    public partial class Distribution : ISortableEntity { }

    public partial class ElectoralDistrict : ISortableEntity { }

    public partial class Ethnicity : ISortableEntity { }

    public partial class Language : ISortableEntity { }

    public partial class MediaDesk : ISortableEntity { }

    public partial class MediaJobTitle : ISortableEntity { }

    public partial class MediaType : ISortableEntity { }

    public partial class MinisterialJobTitle : ISortableEntity { }

     public partial class Ministry : IDeletableEntity, ISortableEntity { }

    public partial class PhoneType : ISortableEntity
    {
        public const string Primary = "Primary";
        public const string NewsDesk = "News Desk";
        public const string Cell = "Cell";
    }

    public partial class PrintCategory : ISortableEntity { }

    public partial class ProvState : ISortableEntity { }

    public partial class PublicationDay : ISortableEntity { }

    public partial class PublicationFrequency : ISortableEntity { }

    public partial class Region : ISortableEntity { }

    public partial class Report : ISortableEntity { }

    public partial class Sector : ISortableEntity { }

    public partial class SpecialtyPublication : ISortableEntity { }

    public partial class WebAddressType : ISortableEntity
    {
        public const string Email = "Email Address";
        public const string Twitter = "Twitter Handle";
        public const string Facebook = "Facebook URL";
        public const string GooglePlus = "Google+ URL";
        public const string Website = "Website URL";
    }

}