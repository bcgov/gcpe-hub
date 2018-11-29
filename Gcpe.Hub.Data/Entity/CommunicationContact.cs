using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("CommunicationContact", Schema = "calendar")]
    public partial class CommunicationContact
    {
        public CommunicationContact()
        {
            Activity = new HashSet<Activity>();
        }

        public int Id { get; set; }
        public int SystemUserId { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(50)]
        public string MinistryShortName { get; set; }
        public Guid? MinistryId { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }
        public byte[] TimeStamp { get; set; }
        public Guid? RowGuid { get; set; }

        [ForeignKey("MinistryId")]
        [InverseProperty("CommunicationContact")]
        public Ministry Ministry { get; set; }
        [ForeignKey("SystemUserId")]
        [InverseProperty("CommunicationContact")]
        public SystemUser SystemUser { get; set; }
        [InverseProperty("CommunicationContact")]
        public ICollection<Activity> Activity { get; set; }
    }
}
