//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace mi.Areas.Panel.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_product()
        {
            this.tbl_productImage = new HashSet<tbl_productImage>();
        }
    
        public int productId { get; set; }
        public string productName { get; set; }
        public Nullable<int> stock { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<int> categoryId { get; set; }
        public Nullable<int> brandId { get; set; }
    
        public virtual tbl_brand tbl_brand { get; set; }
        public virtual tbl_category tbl_category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_productImage> tbl_productImage { get; set; }
    }
}
