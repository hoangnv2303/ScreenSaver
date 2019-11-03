using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScreenSaver.Helper
{
    public class MyConstants
    {
        private static ADWebHelper adHelper = new ADWebHelper();
        public static string NOTIFY_NOTICE = "notice";
        public static string NOTIFY_INFO = "info";
        public static string NOTIFY_SUCCESS = "success";
        public static string NOTIFY_ERROR = "error";
    }
}   