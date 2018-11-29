using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Company", Schema = "media")]
    public partial class Company
    {
        public Company()
        {
            CompanyAddress = new HashSet<CompanyAddress>();
            CompanyDistribution = new HashSet<CompanyDistribution>();
            CompanyElectoralDistrict = new HashSet<CompanyElectoralDistrict>();
            CompanyEthnicity = new HashSet<CompanyEthnicity>();
            CompanyLanguage = new HashSet<CompanyLanguage>();
            CompanyMediaDesk = new HashSet<CompanyMediaDesk>();
            CompanyMediaPartnerCompany = new HashSet<CompanyMediaPartner>();
            CompanyMediaPartnerMediaPartner = new HashSet<CompanyMediaPartner>();
            CompanyMediaType = new HashSet<CompanyMediaType>();
            CompanyPhoneNumber = new HashSet<CompanyPhoneNumber>();
            CompanyPublicationDays = new HashSet<CompanyPublicationDays>();
            CompanyRegion = new HashSet<CompanyRegion>();
            CompanySector = new HashSet<CompanySector>();
            CompanySpecialtyPublication = new HashSet<CompanySpecialtyPublication>();
            CompanyWebAddress = new HashSet<CompanyWebAddress>();
            ContactBeat = new HashSet<ContactBeat>();
            ContactMediaJobTitle = new HashSet<ContactMediaJobTitle>();
            InverseParentCompany = new HashSet<Company>();
            MediaRequestContact = new HashSet<MediaRequestContact>();
        }

        public Guid Id { get; set; }
        public Guid? ParentCompanyId { get; set; }
        [Required]
        [StringLength(250)]
        public string CompanyName { get; set; }
        [Required]
        [StringLength(2500)]
        public string CompanyDescription { get; set; }
        [StringLength(500)]
        public string CirculationDescription { get; set; }
        [StringLength(500)]
        public string Deadlines { get; set; }
        [StringLength(500)]
        public string KeyPrograms { get; set; }
        public Guid? PrintCategoryId { get; set; }
        public Guid? PublicationFrequencyId { get; set; }
        public bool? IsMajorMedia { get; set; }
        public bool? IsEthnicMedia { get; set; }
        public bool? IsLiveMedia { get; set; }
        public bool IsOutlet { get; set; }
        public bool IsActive { get; set; }
        [StringLength(50)]
        public string RecordEditedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }

        [ForeignKey("ParentCompanyId")]
        [InverseProperty("InverseParentCompany")]
        public Company ParentCompany { get; set; }
        [ForeignKey("PrintCategoryId")]
        [InverseProperty("Company")]
        public PrintCategory PrintCategory { get; set; }
        [ForeignKey("PublicationFrequencyId")]
        [InverseProperty("Company")]
        public PublicationFrequency PublicationFrequency { get; set; }
        [InverseProperty("Company")]
        public ICollection<CompanyAddress> CompanyAddress { get; set; }
        [InverseProperty("Company")]
        public ICollection<CompanyDistribution> CompanyDistribution { get; set; }
        [InverseProperty("Company")]
        public ICollection<CompanyElectoralDistrict> CompanyElectoralDistrict { get; set; }
        [InverseProperty("Company")]
        public ICollection<CompanyEthnicity> CompanyEthnicity { get; set; }
        [InverseProperty("Company")]
        public ICollection<CompanyLanguage> CompanyLanguage { get; set; }
        [InverseProperty("Company")]
        public ICollection<CompanyMediaDesk> CompanyMediaDesk { get; set; }
        [InverseProperty("Company")]
        public ICollection<CompanyMediaPartner> CompanyMediaPartnerCompany { get; set; }
        [InverseProperty("MediaPartner")]
        public ICollection<CompanyMediaPartner> CompanyMediaPartnerMediaPartner { get; set; }
        [InverseProperty("Company")]
        public ICollection<CompanyMediaType> CompanyMediaType { get; set; }
        [InverseProperty("Company")]
        public ICollection<CompanyPhoneNumber> CompanyPhoneNumber { get; set; }
        [InverseProperty("Company")]
        public ICollection<CompanyPublicationDays> CompanyPublicationDays { get; set; }
        [InverseProperty("Company")]
        public ICollection<CompanyRegion> CompanyRegion { get; set; }
        [InverseProperty("Company")]
        public ICollection<CompanySector> CompanySector { get; set; }
        [InverseProperty("Company")]
        public ICollection<CompanySpecialtyPublication> CompanySpecialtyPublication { get; set; }
        [InverseProperty("Company")]
        public ICollection<CompanyWebAddress> CompanyWebAddress { get; set; }
        [InverseProperty("Company")]
        public ICollection<ContactBeat> ContactBeat { get; set; }
        [InverseProperty("Company")]
        public ICollection<ContactMediaJobTitle> ContactMediaJobTitle { get; set; }
        [InverseProperty("ParentCompany")]
        public ICollection<Company> InverseParentCompany { get; set; }
        [InverseProperty("Company")]
        public ICollection<MediaRequestContact> MediaRequestContact { get; set; }
    }
}
