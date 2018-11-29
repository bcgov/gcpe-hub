using System;

namespace Gcpe.Hub.Website.Models
{
    public class MediaContactDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public MediaJobDto Job { get; set; }
        public string WorkPhone { get; set; }
        public string WorkPhoneExtension { get; set; }
        public string CellPhone { get; set; }
        public string Email { get; set; }
    }
}