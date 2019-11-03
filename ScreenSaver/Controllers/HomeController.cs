using ScreenSaver.Helper;
using ScreenSaver.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using ScreenSaver.Controllers;

namespace ScreenSaver.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            try
            {
                if (ReadWriteCSV.ReadCSV(pathCSV).Count != 0)
                    Session["Images"] = ReadWriteCSV.ReadCSV(pathCSV);
                else
                    Session["Images"] = null;
            }
            catch (Exception ex)
            {
                Notification("Error", "Can't read CSV file!, " + ex.ToString(), MyConstants.NOTIFY_ERROR);
            }
            return View();
        }
    }
}