using SparePro.Model;
using SparePro.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SparePro.Controllers
{
    [SessionExpire]
    [NoCacheAttribute]
    public class ProjectMasterController : Controller
    {
        IProjectMasterRepository ObjMasterRepository = new ProjectMasterRepository();
        ICommonRepository ObjCommRepository = new CommonRepository();

        #region "Control Pages Action" 

        public ActionResult DefaultMasterDetail(string HeaderViewID, string DetailViewID)
        {
            DefaultDetailMasterModel obj = new DefaultDetailMasterModel();
        
            return View(obj);
        }     

        public ActionResult BrandMaster(string HeaderViewID, string DetailViewID)
        {
            return View();
        }

        public ActionResult AddBrandMaster()
        {
            return View();
        }

        public ActionResult PartMaster(string HeaderViewID, string DetailViewID)
        {
            return View();
        }

        public ActionResult AddPartMaster()
        {
            return View();
        }

        public ActionResult ItemMaster(string HeaderViewID, string DetailViewID)
        {
            return View();
        }       

        public ActionResult AddItemMaster()
        {
            return View();
        }

        public ActionResult DefaultImageMaster(string HeaderViewID, string DetailViewID,string DefaultHeader, string title)
        {
            DefaultImageModel obj = new DefaultImageModel();
            obj.HeaderViewID = HeaderViewID;
            obj.DetailViewID = DetailViewID;
            obj.DefaultHeader = DefaultHeader;
            obj.Title = title;

            return View(obj);
        }

        public ActionResult AccPayment(string HeaderViewID, string DetailViewID)
        {
            return View();
        }

        #endregion

        #region "Default Master"
        [HttpGet]
        public JsonResult DefaultMasterDetail_FindAll(string DefaultHeaderID, bool? Status, int? page, int? limit, string sortBy, string direction)
        {
            int TotalCount;
            List<DefaultDetailMasterModel> ObjProject = ObjMasterRepository.DefaultMasterDetail_FindAll(DefaultHeaderID, Status, page, limit, sortBy, direction, out TotalCount);
            return Json(new { records = ObjProject, total = TotalCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DefaultMasterDetail_LookUp(DefaultDetailMasterModel ObjDefaultDetailModel)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();

            ObjDefaultDetailModel.CreatedBy = SessionExpire.GetUserID();
            ObjDefaultDetailModel.DefaultOrder = ObjMasterRepository.DefaultMaxOrder(ObjDefaultDetailModel.DefaultHeaderID);

            ObjMessage = ObjMasterRepository.DefaultMasterDetail_Save(ObjDefaultDetailModel);

            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public JsonResult DefaultMasterDetail_Save(DefaultDetailMasterModel ObjDefaultDetailModel)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();

            ObjDefaultDetailModel.CreatedBy = SessionExpire.GetUserID();
            ObjMessage = ObjMasterRepository.DefaultMasterDetail_Save(ObjDefaultDetailModel);

            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DefaultMasterDetail_Delete(string DefaultDetailID)
        {

            ReturnMessageModel ObjMessage = new ReturnMessageModel();

            ObjMessage = ObjMasterRepository.DefaultMasterDetail_Delete(DefaultDetailID, SessionExpire.GetUserID());

            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        #endregion                 

        #region "Brand Master"

        [HttpGet]
        public ActionResult BrandMaster_FindAll(int? page, int? limit, string BrandName, bool? Status,string sortBy, string direction)
        {
            int TotalCount = 0;
            List<BrandMasterModel> BrandMasterlist = ObjMasterRepository.BrandMaster_FindAll(page, limit, BrandName, Status,sortBy, direction, out TotalCount);
            return Json(new { records = BrandMasterlist, total = TotalCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetBrandImage(long BrandID, string CurrentTime)
        {
            byte[] imageByteData = ObjMasterRepository.GetBrandImage(BrandID);
            if (imageByteData == null)
            {
                imageByteData = System.IO.File.ReadAllBytes(HttpContext.Server.MapPath("~/Images/no_image.png"));
            }
            return File(imageByteData, "image/png");
        }

        [HttpGet]
        public JsonResult BrandMaster_Edit(int BrandID)
        {
            BrandMasterModel ObjMessage = ObjMasterRepository.BrandMaster_Edit(BrandID);
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BrandMaster_Save(BrandMasterModel objBrandMaster)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            objBrandMaster.CreatedBy = SessionExpire.GetUserID();

            HttpFileCollectionBase files = Request.Files;
            string fname;
            byte[] tempFile = null;

            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];

                // Checking for Internet Explorer  
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fname = file.FileName;
                }

                tempFile = new byte[file.ContentLength];
                file.InputStream.Read(tempFile, 0, file.ContentLength);

                String[] strFileName = fname.Split('.');


                objBrandMaster.BrandImage = tempFile;

            }


            ObjMessage = ObjMasterRepository.BrandMaster_Save(objBrandMaster);
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BrandMaster_Delete(int BrandID)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            ObjMessage = ObjMasterRepository.BrandMaster_Delete(BrandID, SessionExpire.GetUserID());
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult BrandMaster_LookUp(BrandMasterModel objBrandMaster)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();

            objBrandMaster.CreatedBy = SessionExpire.GetUserID();           

            ObjMessage = ObjMasterRepository.BrandMaster_Save(objBrandMaster);

            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Part Master"

        [HttpGet]
        public ActionResult PartMaster_FindAll(int? page, int? limit, string PartName, bool? Status, string sortBy, string direction)
        {
            int TotalCount = 0;
            List<PartMasterModel> PartMasterlist = ObjMasterRepository.PartMaster_FindAll(page, limit, PartName, Status, sortBy, direction, out TotalCount);
            return Json(new { records = PartMasterlist, total = TotalCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PartMaster_Edit(int PartID)
        {
            PartMasterModel ObjMessage = ObjMasterRepository.PartMaster_Edit(PartID);
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PartMaster_Save(PartMasterModel objPartMaster)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            objPartMaster.CreatedBy = SessionExpire.GetUserID();

            ObjMessage = ObjMasterRepository.PartMaster_Save(objPartMaster);
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PartMaster_Delete(int PartID)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            ObjMessage = ObjMasterRepository.PartMaster_Delete(PartID, SessionExpire.GetUserID());
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PartMaster_LookUp(PartMasterModel objPartMaster)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            objPartMaster.CreatedBy = SessionExpire.GetUserID();
            ObjMessage = ObjMasterRepository.PartMaster_Save(objPartMaster);
            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Item Master"

        [HttpGet]
        public JsonResult ItemMaster_FindAll(int? page, int? limit, long? BrandID, long? PartID, string Item, string sortBy, string direction, bool? Status)
        {
            int TotalCount;
            List<ItemMasterModel> ObjSC = ObjMasterRepository.ItemMaster_FindAll(page, limit, BrandID, PartID, Item, sortBy, direction, Status, out TotalCount);

            return Json(new { records = ObjSC, total = TotalCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ItemMaster_Save(ItemMasterModel ObjItemModel)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            ObjItemModel.CreatedBy = SessionExpire.GetUserID();
            ObjMessage = ObjMasterRepository.ItemMaster_Save(ObjItemModel);

            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ItemMaster_Edit(long ItemID)
        {
            ItemMasterModel ObjItemModel = ObjMasterRepository.ItemMaster_Edit(ItemID);
            return Json(ObjItemModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ItemMaster_Delete(long ItemID)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();

            ObjMessage = ObjMasterRepository.ItemMaster_Delete(ItemID, SessionExpire.GetUserID());

            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Acc Payment"

        [HttpGet]
        public JsonResult AccPayment_FindAll(int? page, int? limit, string sortBy, string direction, int? PaidBy)
        {
            int TotalCount;
            List<AccPaymentModel> ObjSC = ObjMasterRepository.AccPayment_FindAll(page, limit, sortBy, direction, PaidBy, out TotalCount);

            return Json(new { records = ObjSC, total = TotalCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AccPayment_Get(int? PaidBy)
        {
            AccPaymentModel ObjSC = ObjMasterRepository.AccPayment_Total(PaidBy);

            return Json(new { record = ObjSC }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AccPayment_Save(AccPaymentModel ObjItemModel)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            ObjMessage = ObjMasterRepository.AccPayment_Save(ObjItemModel);

            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }       

        [HttpPost]
        public JsonResult AccPayment_Delete(long ItemID)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();

            ObjMessage = ObjMasterRepository.AccPayment_Delete(ItemID);

            return Json(ObjMessage, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }

}