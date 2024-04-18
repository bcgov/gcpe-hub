using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Gcpe.Hub.Website.Models
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string DisplayAs { get; set; }

        public bool IsEditor { get; set; }

        public bool IsAdvanced { get; set; }

        public bool IsBCWSOnly { get; set; }

        public string WorkTelephone { get; set; }

        public string WorkTelephoneExtension { get; set; }

        public string MobileTelephone { get; set; }

        public string EmailAddress { get; set; }

        public string UserDomainName { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }
    }
}