//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CorporateCalendar.Data.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class CommunicationMaterial
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CommunicationMaterial()
        {
            this.ActivityCommunicationMaterials = new HashSet<ActivityCommunicationMaterial>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public byte[] TimeStamp { get; set; }
        public System.Guid RowGuid { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityCommunicationMaterial> ActivityCommunicationMaterials { get; set; }
    }
}
