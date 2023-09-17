using SparePro.Model;
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
    public class HomeController : CommonController
    {
        ICommonRepository ObjCommonRepository = new CommonRepository();

        //Added for Session Handling
        [HttpPost]
        public JsonResult KeepSessionAlive()
        {
            return new JsonResult
            {
                Data = "Beat Generated"
            };
        }

        // GET: Home
        public ActionResult AdminIndex()
        {
            return View();
        }

        public ActionResult SalesIndex()
        {
            return View();
        }
    }
}