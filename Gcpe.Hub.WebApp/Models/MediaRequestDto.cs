using Gcpe.Hub.WebApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Gcpe.Hub.Website.Models
{
    public class MediaRequestDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public UserDto CreatedBy { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public UserDto ModifiedBy { get; set; }
        public MinistryDto LeadMinistry { get; set; }
        public IEnumerable<MinistryDto> SharedMinistries { get; set; }
        public MinistryDto TakeOverRequestMinistry { get; set; }
        public UserDto ResponsibleUser { get; set; }
        public IEnumerable<MediaContactDto> MediaContacts { get; set; }
        public DateTimeOffset? DeadlineAt { get; set; }
        public string RequestTopic { get; set; }
        public string RequestContent { get; set; }
        public DateTimeOffset RequestedAt { get; set; }
        public DateTimeOffset? AcknowledgedAt { get; set; }
        public string Response { get; set; }
        public DateTimeOffset? RespondedAt { get; set; }
        public MediaRequestDto ParentRequest { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Data.Entity.EodReportWith? EodReportWith { get; set; }
        public ResolutionDto Resolution { get; set; }
        public IEnumerable<UserDto> CommContacts { get; set; }
    }
}