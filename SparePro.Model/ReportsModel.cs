using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace SparePro.Model
{
   
    public class StockModel
	{
        public string ItemDesc { get; set; }
        public Nullable<double> OrderQty { get; set; }       
        public Nullable<decimal> OrderTotal { get; set; }
        public Nullable<decimal> OrderedCost { get; set; }
        public Nullable<double> PurchasedQty { get; set; }
        public Nullable<decimal> PurchasedCost { get; set; }
        public Nullable<decimal> PurchasedTotal { get; set; }
        public Nullable<double> Stock { get; set; }
        public Nullable<decimal> StockTotal { get; set; }

    }

    public class PaymentReportModel
    {
        public string CreatedDate { get; set; }
        public string RequestedBy { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> BalanceAmount { get; set; }
        public string PaymentStatus { get; set; }

    }

    public class MonthlyReportModel
    {
        public int? Year { get; set; }
        public string Month { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> BalanceAmount { get; set; }
        public string RequestedBy { get; set; }

    }

    public class OrderReportModel
    {        
        public string CreatedDate { get; set; }       
        public string RequestedBy { get; set; }
        public string Items { get; set; }
        public string TotalAmount { get; set; }
        public string PaymentStatus { get; set; }

    }

}
