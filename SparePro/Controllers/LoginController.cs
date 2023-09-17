using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SparePro.Model;
using SparePro.Repository;

namespace SparePro.Controllers
{
    [NoCacheAttribute]
    public class LoginController : Controller
    {
        ICommonRepository ObjCommRepository = new CommonRepository();
        
        #region "Login Master"
        // GET: Login
        public ActionResult Index()
        {
            Session["RequestedBy"] = null;
            Session["fromDate"] = null;
            Session["toDate"] = null;

            var Environment = ConfigurationManager.AppSettings["Environment"].ToString();
            if (Environment == "DEV")
            {
                var DevelopmentUserName = ConfigurationManager.AppSettings["DevelopmentUserName"].ToString();
                var DevelopmentPassword = ConfigurationManager.AppSettings["DevelopmentPassword"].ToString();
                GetLoginAccess(DevelopmentUserName, DevelopmentPassword);
                if (Session["HOME_ACTNAME"] != null)
                    return RedirectToAction(Session["HOME_ACTNAME"].ToString(), Session["HOME_CONTNAME"].ToString());
            }
            return View();
        }


        public JsonResult GetLoginAccess(String LoginName, String Password)  
        {
            List<SessionLoginDetail> ObjLoginSession = new List<SessionLoginDetail>();
                                  SessionInvalidLogin ObjInvalidLogin = new SessionInvalidLogin();
            bool IsLogin = false;
            bool IsStore = false;
            string Str_ControllerName = "";
            string Str_ActionName = "";
            int PasswordAttemptCount = ObjCommRepository.GetPasswordAttemptCount(LoginName);

            ObjInvalidLogin = ObjCommRepository.IsRoleMapInvalidPassword(LoginName, Password);

            if (ObjInvalidLogin.UserID == 0)
            {
                return Json(new { IsLogin = false, LoginName = LoginName, UserID = 0, PasswordAttemptCount = PasswordAttemptCount, InValidPassword = true, ISStore = IsStore, ControllerName= Str_ControllerName, ActionName = Str_ActionName }, JsonRequestBehavior.AllowGet);
            }
            else if (ObjInvalidLogin.InValidPassword == true)
            {
                return Json(new { IsLogin = IsLogin, LoginName = LoginName, UserID = ObjInvalidLogin.UserID,  InValidPassword = ObjInvalidLogin.InValidPassword, ISStore = IsStore, ControllerName = Str_ControllerName, ActionName = Str_ActionName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ObjLoginSession = ObjCommRepository.GetLoginAccess(LoginName, Password);

                if (ObjLoginSession.Count > 0)
                {
                    if (ObjLoginSession.Count == 1)
                    {
                        IsLogin = true;
                    } 

                    if (ObjLoginSession != null && IsLogin == true)
                    {                          
                        Session["USER_ID"] = ObjLoginSession[0].UserID;
                        Session["ROLE_ID"] = ObjLoginSession[0].RoleID;
                       
                        Session["USER_NAME"] = ObjLoginSession[0].UserName;
                        Session["ROLE_NAME"] = ObjLoginSession[0].RoleName;
                       
                        Session["HOME_CONTNAME"] = ObjLoginSession[0].ControllerName;
                        Session["HOME_ACTNAME"] = ObjLoginSession[0].ActionName;

                        Str_ControllerName = ObjLoginSession[0].ControllerName;
                        Str_ActionName = ObjLoginSession[0].ActionName;

                        Session["TIME_ZONE"] = "Singapore Standard Time";
                        if (PasswordAttemptCount != 0 && PasswordAttemptCount <= 3)
                        {
                            ISystemMasterRepository ObjMasterRepository = new SystemMasterRepository();
                            ObjMasterRepository.ResetUserLogin_Update(ObjLoginSession[0].UserID);
                        }
                        if(ObjLoginSession[0].RoleID == "5X6L281Y6W3O")
                        {
                            IsStore = true;
                        }
                    }
                    return Json(new { IsLogin = IsLogin, LoginName = LoginName, UserID = SessionExpire.GetUserID(),InValidPassword = false, ISStore = IsStore, ControllerName = Str_ControllerName, ActionName = Str_ActionName }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsLogin = false, LoginName = LoginName, UserID = 0, PasswordAttemptCount = PasswordAttemptCount,  InValidPassword = true, ISStore = IsStore, ControllerName = Str_ControllerName, ActionName = Str_ActionName }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        #endregion
    }
}