using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("MediaRequest", Schema = "media")]
    public partial class MediaRequest
    {
        public MediaRequest()
        {
            InverseRequestParent = new HashSet<MediaRequest>();
            MediaRequestContact = new HashSet<MediaRequestContact>();
            MediaRequestSharedMinistry = new HashSet<MediaRequestSharedMinistry>();
        }

        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public int ModifiedById { get; set; }
        public Guid LeadMinistryId { get; set; }
        public int ResponsibleUserId { get; set; }
        public DateTimeOffset? DeadlineAt { get; set; }
        [Required]
        [StringLength(250)]
        public string RequestTopic { get; set; }
        [Required]
        [StringLength(4000)]
        public string RequestContent { get; set; }
        [Required]
        public string Response { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset RequestedAt { get; set; }
        public DateTimeOffset? AcknowledgedAt { get; set; }
        public DateTimeOffset? RespondedAt { get; set; }
        public Guid? RequestParentId { get; set; }
        public EodReportWith? EodReportWith { get; set; }
        public Guid? ResolutionId { get; set; }
        public Guid? TakeOverRequestMinistryId { get; set; }

        [ForeignKey("CreatedById")]
        [InverseProperty("MediaRequestCreatedBy")]
        public SystemUser CreatedBy { get; set; }
        [ForeignKey("LeadMinistryId")]
        [InverseProperty("MediaRequestLeadMinistry")]
        public Ministry LeadMinistry { get; set; }
        [ForeignKey("ModifiedById")]
        [InverseProperty("MediaRequestModifiedBy")]
        public SystemUser ModifiedBy { get; set; }
        [ForeignKey("RequestParentId")]
        [InverseProperty("InverseRequestParent")]
        public MediaRequest RequestParent { get; set; }
        [ForeignKey("ResolutionId")]
        [InverseProperty("MediaRequest")]
        public MediaRequestResolution Resolution { get; set; }
        [ForeignKey("ResponsibleUserId")]
        [InverseProperty("MediaRequestResponsibleUser")]
        public SystemUser ResponsibleUser { get; set; }
        [ForeignKey("TakeOverRequestMinistryId")]
        [InverseProperty("MediaRequestTakeOverRequestMinistry")]
        public Ministry TakeOverRequestMinistry { get; set; }
        [InverseProperty("RequestParent")]
        public ICollection<MediaRequest> InverseRequestParent { get; set; }
        [InverseProperty("MediaRequest")]
        public ICollection<MediaRequestContact> MediaRequestContact { get; set; }
        [InverseProperty("MediaRequest")]
        public ICollection<MediaRequestSharedMinistry> MediaRequestSharedMinistry { get; set; }
    }
}
