using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace SparePro.Model
{
   
    public class PurchaseModel
	{
        public long PurchaseID { get; set; }
        public string PaymentStatus { get; set; }
        public string PurchaseStatus { get; set; }
        public string PaymentStatusStr { get; set; }
        public string PurchaseStatusStr { get; set; }
        public string RequestedBy { get; set; }
        public string Note { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<double> PaidAmount { get; set; }
        public Nullable<double> BalanceAmount { get; set; }
        public Nullable<int> CreatedBy { get; set; }        
        public string CreatedDate { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        //public string DisplayCreatedDate
        //{
        //    get
        //    {
        //        return CreatedDate != null ? CreatedDate.Value.ToString(SparePro.Model.CommonDateFormat.StringDateFormat) : "";
        //    }
        //    set { }
        //}
        public Nullable<int> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
      
        public List<PurchaseDetailModel> PurchaseDetail { get; set; }
        public string Items { get; set; }
        public string Parts { get; set; }
        public bool IsChecked { get; set; }

    }
    
    public class PurchasePaymentModel
    {
        public long PaymentDetailsID { get; set; }
        public long PurchaseID { get; set; }
        public Nullable<double> PaidAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> BalanceAmount { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string PaymentBy { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string DisplayCreatedDate
        {
            get
            {
                return CreatedDate != null ? CreatedDate.Value.ToString(SparePro.Model.CommonDateFormat.StringDateFormat) : "";
            }
            set { }
        }
    }

    public class PurchaseDetailModel
    {
        public long PurchaseDetailsID { get; set; }
        public long PurchaseID { get; set; }
        public Nullable<double> Quantity { get; set; }
        public long ItemID { get; set; }
        public long BrandID { get; set; }
        public long PartID { get; set; }
        public string BrandName { get; set; }
        public string PartName { get; set; }
        public string ItemName { get; set; }
        public string Color { get; set; }
        public decimal? ItemPrice { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }

    }

}
