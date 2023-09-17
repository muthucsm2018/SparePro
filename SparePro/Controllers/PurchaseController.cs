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
    public class PurchaseController : Controller
    {
        IPurchaseRepository _objPurchasesRepository = new PurchaseRepository();
        ICommonRepository ObjCommRepository = new CommonRepository();        
        
        public ActionResult ListPurchases(string HeaderViewID, string DetailViewID)
        {
             return View();
        }

        public ActionResult AddPurchasesRequest(string PurchaseID, string HeaderViewID, string DetailViewID)
        {
            PurchaseModel obj = new PurchaseModel();
            //obj.PurchaseID = _objPurchasesRepository.GeneratePurchaseID(SessionExpire.GetUserID());
            return View(obj);
        }       
      
        #region "Purchase"

        public ActionResult GeneratePurchaseID()
        {
            long purchaseid;
            purchaseid = _objPurchasesRepository.GeneratePurchaseID(SessionExpire.GetUserID());
            return Json(purchaseid, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Purchase_FindAll(int? page, int? limit, int? RequestedBy, DateTime? fromDate, DateTime? toDate, string sortBy, string direction)
        {
            int TotalCount = 0;
            Session["RequestedBy"] = RequestedBy;
            Session["fromDate"] = fromDate;
            Session["toDate"] = toDate;
            //List<PurchaseModel> Purchaseslist = new List<PurchaseModel>();
            //if (RequestedBy == null && fromDate == null && toDate == null) { }
            //else
            //{
                List<PurchaseModel> Purchaseslist = _objPurchasesRepository.Purchase_FindAll(page, limit, RequestedBy, fromDate, toDate, sortBy, direction, out TotalCount);
            //}
            return Json(new { records = Purchaseslist, total = TotalCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Purchase_FindAllOnlyBalance(int? page, int? limit, int? RequestedBy, DateTime? fromDate, DateTime? toDate, string sortBy, string direction)
        {
            int TotalCount = 0;
            Session["RequestedBy"] = RequestedBy;
            Session["fromDate"] = fromDate;
            Session["toDate"] = toDate;
           
            List<PurchaseModel> Purchaseslist = _objPurchasesRepository.Purchase_FindAll(page, limit, RequestedBy, fromDate, toDate, sortBy, direction, out TotalCount);
            Purchaseslist = Purchaseslist.Where(a => a.PaymentStatus != "Paid").ToList();
            return Json(new { records = Purchaseslist, total = TotalCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PurchaseDetail_FindAll(long PurchaseID)
        {
            List<PurchaseDetailModel> Purchaseslist = _objPurchasesRepository.PurchaseDetail_FindAll(PurchaseID);
            return Json(new { records = Purchaseslist }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Purchase_Edit(long PurchaseID)
        {
            PurchaseModel ObjMessage = _objPurchasesRepository.Purchase_Edit(PurchaseID);
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Purchase_Save(PurchaseModel objPurchase)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            objPurchase.CreatedBy = SessionExpire.GetUserID();

            ObjMessage = _objPurchasesRepository.Purchase_Save(objPurchase, objPurchase.PurchaseDetail);
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Purchase_Delete(long PurchaseID, string PurchaseStatus)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            ObjMessage = _objPurchasesRepository.Purchase_Delete(PurchaseID, PurchaseStatus, SessionExpire.GetUserID());
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Purchase Payment Detail"

        [HttpGet]
        public ActionResult PurchasePayment_FindAll(long PurchaseID, string AccountMode, int? page, int? limit, string sortBy, string direction)
        {
            int TotalCount = 0;
            List<PurchasePaymentModel> Paymentlist = _objPurchasesRepository.PurchasePayment_FindAll(PurchaseID, page, limit, sortBy, direction, out TotalCount);
            return Json(new { records = Paymentlist, total = TotalCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PurchasePayment_Save(PurchasePaymentModel objPurchasePayments)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            objPurchasePayments.CreatedBy = SessionExpire.GetUserID();           
            ObjMessage = _objPurchasesRepository.PurchasePayment_Save(objPurchasePayments);
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PurchasePayment_Delete(int PurchasePaymentID)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            ObjMessage = _objPurchasesRepository.PurchasePayment_Delete(PurchasePaymentID, SessionExpire.GetUserID());
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }
        
        #endregion
        
    }
}