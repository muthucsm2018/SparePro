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
    public class OrderController : Controller
    {
        IOrderRepository _objOrdersRepository = new OrderRepository();
        ICommonRepository ObjCommRepository = new CommonRepository();        
        
        public ActionResult ListOrders(string HeaderViewID, string DetailViewID)
        {
             return View();
        }

        public ActionResult AddOrderRequest(string OrderID, string HeaderViewID, string DetailViewID)
        {
            OrderModel obj = new OrderModel();
            //obj.OrderID = _objOrdersRepository.GenerateOrderID(SessionExpire.GetUserID());
            return View(obj);
        }       
      
        #region "Order"

        public ActionResult GenerateOrderID()
        {
            long Orderid;
            Orderid = _objOrdersRepository.GenerateOrderID(SessionExpire.GetUserID());
            return Json(Orderid, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Order_FindAll(int? page, string ItemName, int? limit, int? RequestedBy, string sortBy, string direction)
        {
            int TotalCount = 0;

            List<OrderModel> Orderslist = _objOrdersRepository.Order_FindAll(page, ItemName, limit, RequestedBy, sortBy, direction, out TotalCount);
            return Json(new { records = Orderslist, total = TotalCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult OrderDetail_FindAll(long OrderID)
        {
            List<OrderDetailModel> Orderslist = _objOrdersRepository.OrderDetail_FindAll(OrderID);
            return Json(new { records = Orderslist }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Order_Edit(long OrderID)
        {
            OrderModel ObjMessage = _objOrdersRepository.Order_Edit(OrderID);
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Order_Save(OrderModel objOrder)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            objOrder.CreatedBy = SessionExpire.GetUserID();

            ObjMessage = _objOrdersRepository.Order_Save(objOrder, objOrder.OrderDetail);
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Order_Delete(long OrderID)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            ObjMessage = _objOrdersRepository.Order_Delete(OrderID, SessionExpire.GetUserID());
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        #endregion

        
        
    }
}