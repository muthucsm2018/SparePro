//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SparePro.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class TblDefaultMasterDetail
    {
        public string DefaultDetailID { get; set; }
        public string DefaultHeaderID { get; set; }
        public string DefaultTextField { get; set; }
        public string DefaultValueField { get; set; }
        public byte[] Image { get; set; }
        public Nullable<int> DefaultOrder { get; set; }
        public Nullable<bool> IsSearch { get; set; }
        public Nullable<bool> IsPage { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}