using Newtonsoft.Json;
using ScreenSaver.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScreenSaver.Controllers
{
    public class LoginController : Controller
    {
        ADWebHelper helper = new ADWebHelper();
        // GET: Login
        public ActionResult Index()
        {
            string login_uri = ConfigurationManager.AppSettings["ADWeb_URI"] +
              "/adweb/oauth2/authorization/v1?scope=read&redirect_uri=" +
              Url.Encode(ConfigurationManager.AppSettings["CLIENT_REDIRECT_URL"]) +
              "&response_type=code&client_id=" + ConfigurationManager.AppSettings["CLIENT_ID"] +
              "&state=online";
            ViewBag.Url = login_uri;
            return View();
        }
        public ActionResult Success(string code, string state)
        {
            string access_token = helper.GetAccessToken(code);
            if (!string.IsNullOrEmpty(access_token))
            {
                var user = helper.getUserInfo(access_token);
                if (user != null)
                {
                    user.employee.access_token = access_token;
                    Response.Cookies["user_cookie"].Value = JsonConvert.SerializeObject(user);
                    return RedirectToAction("Index", "Home");
                }
            }
            Response.Cookies["user_cookie"].Value = null;
            return RedirectToAction("index");
        }
        public ActionResult SignOut()
        {
            Response.Cookies["user_cookie"].Value = null;
            return Redirect(@"http://idmgt.fushan.fihnbb.com/web/session/logout?redirect=" + ConfigurationManager.AppSettings["CLIENT_REDIRECT_URL"]);
        }
    }
}


