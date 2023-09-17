using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace SparePro.Model
{
   
    public class OrderModel
	{
        public long OrderID { get; set; }
        
        public Nullable<int> CreatedBy { get; set; }
        //public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedDate { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }

        //public string DisplayCreatedDate
        //{
        //    get
        //    {
        //        return CreatedDate != null ? CreatedDate.Value.ToString(SparePro.Model.CommonDateFormat.StringDateFormat) : "";
        //    }
        //    set { }
        //}
        public string RequestedBy { get; set; }

        public string Brands { get; set; }
        public Nullable<int> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public List<OrderDetailModel> OrderDetail { get; set; }     

    }
    
    public class OrderDetailModel
    {
        public long OrderDetailID { get; set; }
        public long OrderID { get; set; }
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
