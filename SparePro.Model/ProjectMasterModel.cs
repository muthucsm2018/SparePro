using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SparePro.Model
{
    public class RunningNumberModel
    {
        public int RunningNumber { get; set; }
        public int ReturnRunningNumber { get; set; }
    }

    public class DefaultImageModel
    {
        public string HeaderViewID { get; set; }
        public string DetailViewID { get; set; }
        public string DefaultHeader { get; set; }
        public string Title { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }

    }

    public class BrandMasterModel
    {
        public long BrandID { get; set; }        
        public string BrandName { get; set; }       
        public byte[] BrandImage { get; set; }
        public int? SortOrder { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public System.DateTime? CreatedDate { get; set; }
        public int? LastModifiedBy { get; set; }
        public System.DateTime? LastModifiedDate { get; set; }

    }

    public class PartMasterModel
    {
        public long PartID { get; set; }
        public string PartName { get; set; }
        public int? SortOrder { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public System.DateTime? CreatedDate { get; set; }
        public int? LastModifiedBy { get; set; }
        public System.DateTime? LastModifiedDate { get; set; }

    }

    public class ItemMasterModel
    {
        public long ItemID { get; set; }
        public long BrandID { get; set; }
        public long PartID { get; set; }
        public string BrandName { get; set; }
        public string PartName { get; set; }
        public string ItemName { get; set; }
        public decimal? ItemPrice { get; set; }
        public Nullable<bool> InStock { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string CreatedUser { get; set; }
        public string LastModifiedUser { get; set; }
        public Nullable<double> Quantity { get; set; }
    }

    public class AccPaymentModel
    {
        public long AccPaymentID { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string PaymentBy { get; set; }
        public Nullable<int> PaidBy { get; set; }
        public string PaidByName { get; set; }
        public Nullable<double> PaidAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string DisplayPaymentDate
        {
            get
            {
                return PaymentDate != null ? PaymentDate.Value.ToString(SparePro.Model.CommonDateFormat.StringDateFormat) : "";
            }
            set { }
        }
    }

}
