//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aik2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Pics
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pics()
        {
            this.Links = new HashSet<Links>();
            this.Links1 = new HashSet<Links>();
            this.WordLinks = new HashSet<WordLinks>();
        }
    
        public int PicId { get; set; }
        public int ArtId { get; set; }
        public int CraftId { get; set; }
        public string XPage { get; set; }
        public string NN { get; set; }
        public Nullable<int> NNN { get; set; }
        public string Type { get; set; }
        public Nullable<int> NType { get; set; }
        public string Path { get; set; }
        public string Text { get; set; }
        public string Grp { get; set; }
        public Nullable<bool> Copyright { get; set; }
    
        public virtual Arts Arts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Links> Links { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Links> Links1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WordLinks> WordLinks { get; set; }
        public virtual Crafts Crafts { get; set; }
    }
}
