using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    public partial class User
    {
        public User()
        {
            NewsReleaseLog = new HashSet<NewsReleaseLog>();
        }

        public Guid Id { get; set; }
        [Required]
        [StringLength(256)]
        public string DisplayName { get; set; }
        [Required]
        [StringLength(256)]
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }

        [InverseProperty("User")]
        public ICollection<NewsReleaseLog> NewsReleaseLog { get; set; }
    }
}
