using ClosedXML.Excel;
using SparePro.Model;
using System.Data;
using System.IO;
using SparePro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace SparePro.Controllers
{
    [SessionExpire]
    [NoCacheAttribute]
    public class CommonController : Controller
    {
        ICommonRepository ObjCommonRepository = new CommonRepository();
        IProjectMasterRepository ObjMasterRepository = new ProjectMasterRepository();
        // GET: Common        

        [HttpGet]
        public JsonResult Load_HomePage()
        {
            List<HomePage> PageList = null;

            PageList = ObjCommonRepository.Load_HomePage().ToList();

            return Json(PageList, JsonRequestBehavior.AllowGet);
        }       

        [HttpGet]
        public JsonResult GetIsSession()
        {
            return Json(new { IsSession = SessionExpire.GetUserID() != 0 ? true : false }, JsonRequestBehavior.AllowGet);
        } 

        #region "Menu Master"

        public List<MenuMasterHeader> MenuMasterHeader()
        {
            List<MenuMasterHeader> ObjMenuMasterHeader = null;
            ObjMenuMasterHeader = ObjCommonRepository.MenuMasterHeader(SessionExpire.GetRoleID());
            return ObjMenuMasterHeader;
        }
        
        public List<MenuMasterDetail> MenuMasterDetail(string HeaderViewID)
        {
            List<MenuMasterDetail> ObjMenuMasterDetail = null;
            ObjMenuMasterDetail = ObjCommonRepository.MenuMasterDetail(HeaderViewID, SessionExpire.GetRoleID());
            return ObjMenuMasterDetail;
        }

       
        #endregion

        #region "Common Functions"

        [HttpGet]
        public JsonResult Load_CommonDefaultDetails(string DefaultTypeHeaderID)
        {
            List<DefaultTypeItems> DefaultList = null;

            DefaultList = ObjCommonRepository.Load_CommonDefaultDetails(DefaultTypeHeaderID).ToList();

            return Json(DefaultList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RoundDown(decimal Amount)
        {
            decimal RoundOffValue = 0;
            if (Amount != 0) RoundOffValue = ObjCommonRepository.RoundDown(Amount);
            return Json(RoundOffValue, JsonRequestBehavior.AllowGet);
        }
              
        [HttpGet]
        public JsonResult Load_CommonDefaultHeaderDetails()
        {
            List<DefaultHeaderNameItems> LevelList = null;
            
            LevelList = ObjCommonRepository.Load_CommonDefaultHeaderDetails().ToList();

            return Json(LevelList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Load_Status()
        {
            List<LoadItems_Int> StatusList = null;

            StatusList = ObjCommonRepository.Load_Status().ToList();

            return Json(StatusList, JsonRequestBehavior.AllowGet);
        } 

        [HttpGet]
        public JsonResult Load_DefaultType(string DefaultType)
        {
            List<DefaultTypeItems> DefaultTypeList = null;

            DefaultTypeList = ObjCommonRepository.Load_DefaultType(DefaultType).ToList();

            return Json(DefaultTypeList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Load_SystemUserDetail()
        {
            List<LoadItems_BigInt> DefaultTypeList = null;

            DefaultTypeList = ObjCommonRepository.Load_SystemUserDetail();

            return Json(DefaultTypeList, JsonRequestBehavior.AllowGet);
        }  

        [HttpGet]
        public JsonResult Load_DefaultTypeSearch(string DefaultType)
        {
            List<DefaultTypeItems> DefaultTypeList = null;

            DefaultTypeList = ObjCommonRepository.Load_DefaultTypeSearch(DefaultType).ToList();

            return Json(DefaultTypeList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Load_DefaultTypePage(string DefaultType, bool IsPage)
        {
            List<DefaultTypeItems> DefaultTypeList = null;

            DefaultTypeList = ObjCommonRepository.Load_DefaultTypePage(DefaultType, IsPage).ToList();

            return Json(DefaultTypeList, JsonRequestBehavior.AllowGet);
        }              

        [HttpGet]
        public JsonResult DecryptPassword(string vPassword)
        {
            var DecryptPassword = "";
            if (vPassword != null) DecryptPassword = ObjCommonRepository.DecryptPassword(vPassword);

            return Json(DecryptPassword, JsonRequestBehavior.AllowGet);
        } 

        [HttpGet]
        public JsonResult Load_Brand()
        {
            List<LoadItems_BigInt> BrandList = null;

            BrandList = ObjCommonRepository.Load_Brand().ToList();

            return Json(BrandList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Load_Part()
        {
            List<LoadItems_BigInt> PartList = null;

            PartList = ObjCommonRepository.Load_Part().ToList();

            return Json(PartList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Product_PurchaseAutocomplete(string term, long BrandID, long PartID)
        {
            List<Query> ProductList = null;

            ProductList = ObjCommonRepository.Product_Autocomplete(term, BrandID, PartID).ToList();

            return Json(new { records = ProductList }, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Product_GetDetail(long ItemID)
        {
            ItemMasterModel ObjItemModel = ObjMasterRepository.ItemMaster_Edit(ItemID);
            return Json(ObjItemModel, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}
