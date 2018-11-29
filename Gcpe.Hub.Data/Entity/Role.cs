using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{
    [Table("Role", Schema = "calendar")]
    public partial class Role
    {
        public Role()
        {
            SystemUser = new HashSet<SystemUser>();
        }

        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(150)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public byte[] TimeStamp { get; set; }
        public Guid RowGuid { get; set; }

        [InverseProperty("Role")]
        public ICollection<SystemUser> SystemUser { get; set; }
    }
}
