using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using ScreenSaver.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
    
namespace ScreenSaver.Helper
{
    public class ADWebHelper
    {
        private string ADWeb_URI = @"http://idmgt.fushan.fihnbb.com";
        private string CLIENT_ID = @"vEwSNKfc6USHiiyLA3sR7DuKFFrUlbJVm";
        private string CLIENT_SECRET = @"F1uk98jcuxlUoQPAFsqfrJYF3AZwBm7a";
        private string CLIENT_REDIRECT_URL = @"http://localhost:50349/login/success";

        public ADWebHelper()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ADWeb_URI"]))
                ADWeb_URI = ConfigurationManager.AppSettings["ADWeb_URI"];

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CLIENT_ID"]))
                CLIENT_ID = ConfigurationManager.AppSettings["CLIENT_ID"];

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CLIENT_SECRET"]))
                CLIENT_SECRET = ConfigurationManager.AppSettings["CLIENT_SECRET"];

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CLIENT_REDIRECT_URL"]))
                CLIENT_REDIRECT_URL = ConfigurationManager.AppSettings["CLIENT_REDIRECT_URL"];
        }

        public string GetAccessToken(string _code)
        {
            try
            {
                var res = (ADWeb_URI + "/adweb/oauth2/access_token/v1")
                    .PostUrlEncodedAsync(new
                    {
                        client_id = CLIENT_ID,
                        redirect_uri = CLIENT_REDIRECT_URL,
                        client_secret = CLIENT_SECRET,
                        code = _code,
                        grant_type = "authorization_code"
                    }).ReceiveString().Result;
                dynamic obj = JsonConvert.DeserializeObject<object>(res);
                if (!string.IsNullOrEmpty(obj.access_token.ToString()))
                    return obj.access_token.ToString();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public UserModel getUserInfo(string access_token)
        {
            try
            {
                var res = (ADWeb_URI + "/adweb/people/me/v1")
                    .WithOAuthBearerToken(access_token)
                    .GetStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<UserModel>(res);
                return obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}