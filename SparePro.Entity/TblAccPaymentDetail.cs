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
    
    public partial class TblAccPaymentDetail
    {
        public long AccPaymentID { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string PaymentBy { get; set; }
        public int PaidBy { get; set; }
        public Nullable<double> PaymentAmount { get; set; }
    }
}
