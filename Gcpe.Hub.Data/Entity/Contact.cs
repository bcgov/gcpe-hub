using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Contact", Schema = "media")]
    public partial class Contact
    {
        public Contact()
        {
            ContactAddress = new HashSet<ContactAddress>();
            ContactBeat = new HashSet<ContactBeat>();
            ContactElectoralDistrict = new HashSet<ContactElectoralDistrict>();
            ContactMediaJobTitle = new HashSet<ContactMediaJobTitle>();
            ContactPhoneNumber = new HashSet<ContactPhoneNumber>();
            ContactRegion = new HashSet<ContactRegion>();
            ContactSector = new HashSet<ContactSector>();
            ContactWebAddress = new HashSet<ContactWebAddress>();
            MediaRequestContact = new HashSet<MediaRequestContact>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(150)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(150)]
        public string LastName { get; set; }
        public Guid? MinistryId { get; set; }
        public Guid? MinisterialJobTitleId { get; set; }
        [Column("MLAAssignmentId")]
        public Guid? MlaassignmentId { get; set; }
        public bool IsPressGallery { get; set; }
        public bool HasMinisterAssignment { get; set; }
        public bool IsPrimaryMediaContact { get; set; }
        public bool IsSecondaryMediaContact { get; set; }
        public bool IsActive { get; set; }
        [StringLength(50)]
        public string RecordEditedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        [StringLength(100)]
        public string ShowNotes { get; set; }

        [ForeignKey("MinisterialJobTitleId")]
        [InverseProperty("Contact")]
        public MinisterialJobTitle MinisterialJobTitle { get; set; }
        [ForeignKey("MinistryId")]
        [InverseProperty("Contact")]
        public Ministry Ministry { get; set; }
        [ForeignKey("MlaassignmentId")]
        [InverseProperty("Contact")]
        public ElectoralDistrict Mlaassignment { get; set; }
        [InverseProperty("Contact")]
        public ICollection<ContactAddress> ContactAddress { get; set; }
        [InverseProperty("Contact")]
        public ICollection<ContactBeat> ContactBeat { get; set; }
        [InverseProperty("Contact")]
        public ICollection<ContactElectoralDistrict> ContactElectoralDistrict { get; set; }
        [InverseProperty("Contact")]
        public ICollection<ContactMediaJobTitle> ContactMediaJobTitle { get; set; }
        [InverseProperty("Contact")]
        public ICollection<ContactPhoneNumber> ContactPhoneNumber { get; set; }
        [InverseProperty("Contact")]
        public ICollection<ContactRegion> ContactRegion { get; set; }
        [InverseProperty("Contact")]
        public ICollection<ContactSector> ContactSector { get; set; }
        [InverseProperty("Contact")]
        public ICollection<ContactWebAddress> ContactWebAddress { get; set; }
        [InverseProperty("Contact")]
        public ICollection<MediaRequestContact> MediaRequestContact { get; set; }
    }
}
