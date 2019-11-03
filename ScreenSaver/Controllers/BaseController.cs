using Newtonsoft.Json;
using ScreenSaver.Helper;
using ScreenSaver.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ScreenSaver.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected UserModel mEmployee = null;
        protected ADWebHelper adWebHelper = new ADWebHelper();
        // Get path
        protected static string pathTempImages = ConfigurationManager.AppSettings["Images_Temp_Path"];
        protected static string pathImages = ConfigurationManager.AppSettings["Images_Path"];
        protected static string pathCSV = ConfigurationManager.AppSettings["CSV_Path"];
        protected static string pathLockScreen = ConfigurationManager.AppSettings["Active_LockScreen_Path"];
        protected static string pathScreenSaver = ConfigurationManager.AppSettings["Active_ScreenSaver_Path"];
        protected static string pathDefaultLockScreen = ConfigurationManager.AppSettings["Default_LockScreen_Path"];
        protected static string pathDefaultScreenSaver = ConfigurationManager.AppSettings["Default_ScreenSaver_Path"];
        protected static string[] permissionDepartment = ConfigurationManager.AppSettings["Permisstion_Department"].Split(',');


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (Request.Cookies["user_cookie"] != null)
                {
                    var temp = Request.Cookies["user_cookie"].Value;
                    mEmployee = JsonConvert.DeserializeObject<UserModel>(temp);
                }
            }
            catch
            {
                mEmployee = null;
            }
            if (mEmployee == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "login", action = "index" }));
                return;
            }
            else
            {
                // Check Permisstion Here
                bool checkPermisstion = false;
                foreach (var item in permissionDepartment)
                    if (item == mEmployee.employee.department[1].ToString())
                        checkPermisstion = true;

                if (adWebHelper.getUserInfo(mEmployee.employee.access_token) != null && checkPermisstion == true)
                {
                    ViewBag.User = mEmployee;
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    Notification("Error", "You don't have permisstion login this page, Please contact with IT!", MyConstants.NOTIFY_ERROR);
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                    return;
                }
            }
        }

        protected void Notification(string title, string content, string type)
        {
            TempData["notify-title"] = title;
            TempData["notify-content"] = content;
            if (type.Equals(MyConstants.NOTIFY_SUCCESS))
                TempData["notify-type"] = "success";
            else if (type.Equals(MyConstants.NOTIFY_INFO))
                TempData["notify-type"] = "info";
            else if (type.Equals(MyConstants.NOTIFY_NOTICE))
                TempData["notify-type"] = "notice";
            else if (type.Equals(MyConstants.NOTIFY_ERROR))
                TempData["notify-type"] = "error";
        }
    }
}