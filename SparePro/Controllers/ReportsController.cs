using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SparePro.Model;
using SparePro.Repository;

namespace SparePro.Controllers
{
    [SessionExpire]
    [NoCacheAttribute]
    public class ReportsController : Controller
    {
        IPurchaseRepository _objReportsRepository = new PurchaseRepository();

        ICommonRepository ObjCommRepository = new CommonRepository();

        public ActionResult StockReport(string HeaderViewID, string DetailViewID)
        {
            return View();
        }

        public ActionResult OrderReport(string HeaderViewID, string DetailViewID)
        {
            return View();
        }
        public ActionResult PaymentReport(string HeaderViewID, string DetailViewID)
        {
            return View();
        }
        public ActionResult BalanceReport(string HeaderViewID, string DetailViewID)
        {
            return View();
        }
        public ActionResult PaidReport(string HeaderViewID, string DetailViewID)
        {
            return View();
        }
        public ActionResult MonthlyReport(string HeaderViewID, string DetailViewID)
        {
            return View();
        }
        public ActionResult UserOrderReport(string HeaderViewID, string DetailViewID)
        {
            return View();
        }

        #region "Stocks"

        [HttpGet]
        public ActionResult Stock_FindAll(int? page, int? limit, long? BrandID, long? PartID, long? ItemID, string sortBy, string direction)
        {
            int TotalCount = 0;

            List<StockModel> Stockslist = _objReportsRepository.Stock_FindAll(page, limit, sortBy, BrandID, PartID, ItemID, direction, out TotalCount);
            return Json(new { records = Stockslist, total = TotalCount }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Order"


        [HttpGet]
        public ActionResult Order_FindAll(int? page, int? limit, int? RequestedBy, DateTime? fromDate, DateTime? toDate, string sortBy, string direction)
        {
            int TotalCount = 0;

            List<OrderReportModel> Orderslist = _objReportsRepository.Order_FindAll(page, limit, sortBy, RequestedBy, fromDate, toDate, direction, out TotalCount);
            return Json(new { records = Orderslist, total = TotalCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UserOrder_FindAll(int? page, int? limit, int? RequestedBy, DateTime? fromDate, DateTime? toDate, string sortBy, string direction)
        {
            int TotalCount = 0;

            List<OrderReportModel> Orderslist = _objReportsRepository.Order_FindAll(page, limit, sortBy, SessionExpire.GetUserID(), fromDate, toDate, direction, out TotalCount);
            return Json(new { records = Orderslist, total = TotalCount }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Payment"

        [HttpGet]
        public ActionResult Payment_FindAll(int? page, int? limit, int? RequestedBy, DateTime? fromDate, DateTime? toDate, string sortBy, string direction)
        {
            int TotalCount = 0;

            List<PaymentReportModel> Orderslist = _objReportsRepository.Payment_FindAll(page, limit, sortBy, RequestedBy, fromDate, toDate, direction, out TotalCount);
            return Json(new { records = Orderslist, total = TotalCount }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "MonthlyReport"

        [HttpGet]
        public ActionResult MonthlyReport_FindAll(int? page, int? limit, int? RequestedBy, int? Year, string Month, string sortBy, string direction)
        {
            int TotalCount = 0;

            List<MonthlyReportModel> Orderslist = _objReportsRepository.MonthlyReport_FindAll(page, limit, sortBy, RequestedBy, Year, Month, direction, out TotalCount);
            return Json(new { records = Orderslist, total = TotalCount }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PaidReport
       
        #endregion
    }
}