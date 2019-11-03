using ScreenSaver.Helper;
using ScreenSaver.Model;
using ScreenSaver.Schedule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace ScreenSaver.Controllers
{
    public class ImagesController : BaseController
    {
        List<ImageModel> imagesList;
        List<ImageModel> imagesListDefault;
        public ActionResult Index(int? type)
        {
            try
            {
                if (type != null)
                {
                    Session["Type"] = type;
                }
                ViewBag.Type = ((int)Session["Type"] != 2) ? "ScreenSaver" : "LockScreen";
                if (Session["Images"] != null)
                {
                    imagesList = (List<ImageModel>)Session["Images"];
                    foreach (var item in imagesList)
                        item.Url = pathTempImages + item.Name;
                    ViewBag.listImages = imagesList.Where(x => x.Type == ViewBag.Type).ToList();
                }
                return View();
            }
            catch (Exception)
            {
                return View("../Home/Index");
            }
        }

        #region LockScreen
        [HttpPost]
        public ActionResult MutipleImages(MultipleImagesModel imagesFiles)
        {
            var startDate = imagesFiles.images.TimeFrom;
            //Ensure model state is valid  
            if (ModelState.IsValid && imagesFiles.dayInterval > 0)
            {
                int count = 0;
                foreach (HttpPostedFileBase file in imagesFiles.files.files)
                {
                    if (file != null)
                    {
                        var InputFileName = Path.GetFileName(file.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath(pathTempImages) + InputFileName);
                        //Save file to server folder  
                        file.SaveAs(ServerSavePath);
                        //Add a images
                        var images = new ImageModel();
                        //Store images obj
                        if (Session["Images"] != null)
                        {
                            imagesList = (List<ImageModel>)Session["Images"];
                            images.ID = imagesList.LastOrDefault().ID + 1;
                            images.TimeFrom = (count == 0) ? startDate : imagesList.LastOrDefault().TimeTo.AddMinutes(1);
                        }
                        else
                        {
                            imagesList = new List<ImageModel>();
                            images.ID = 1;
                            images.TimeFrom = startDate;
                        }
                        count++;
                        images.TimeTo = images.TimeFrom.AddDays(imagesFiles.dayInterval);
                        images.Type = ((int)Session["Type"] == 1) ? "ScreenSaver" : "LockScreen";
                        images.Name = InputFileName;
                        images.Url = pathTempImages + InputFileName;
                        //checkTime
                        var checkTimeResult = CheckTime(1, images.ID, images.TimeFrom, DateTime.MaxValue, images.Type);
                        if (checkTimeResult == "Oke")
                        {
                            imagesList.Add(images);
                            Session["Images"] = imagesList;
                            Notification("Notify", "Upload file successfuly", MyConstants.NOTIFY_SUCCESS);
                        }
                        else
                        {
                            Notification("Error", checkTimeResult, MyConstants.NOTIFY_ERROR);
                        }
                    }
                    else
                    {
                        Notification("Error", "No files being selected", MyConstants.NOTIFY_ERROR);
                    }
                }
            }
            else
            {
                Notification("Error", "Please recheck infomation", MyConstants.NOTIFY_ERROR);
            }
            ViewBag.Type = ((int)Session["Type"] != 2) ? "ScreenSaver" : "LockScreen";
            if (Session["Images"] != null)
            {
                var sessionCart = (List<ImageModel>)Session["Images"];
                ViewBag.listImages = sessionCart.Where(x => x.Type == ViewBag.Type).ToList();
            }
            ModelState.Clear();
            return View("Index");
        }

        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase file, ImageModel image)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    if (ModelState.IsValid)
                    {
                        string _FileName = Path.GetFileName(file.FileName);
                        string _path = Path.Combine(Server.MapPath(pathTempImages), _FileName);
                        file.SaveAs(_path);
                        //Store images obj
                        if (Session["Images"] != null)
                        {
                            imagesList = (List<ImageModel>)Session["Images"];
                            image.ID = imagesList.LastOrDefault().ID + 1;
                        }
                        else
                        {
                            imagesList = new List<ImageModel>();
                            image.ID = 1;
                        }
                        image.Type = ((int)Session["Type"] == 1) ? "ScreenSaver" : "LockScreen";
                        image.Name = _FileName;
                        image.Url = pathTempImages + _FileName;

                        //Check time
                        var checkTimeResult = CheckTime(1, image.ID, image.TimeFrom, image.TimeTo, image.Type);
                        if (checkTimeResult == "Oke")
                        {
                            imagesList.Add(image);
                            Session["Images"] = imagesList;
                            Notification("Notify", "Upload file successfuly", MyConstants.NOTIFY_SUCCESS);
                        }
                        else
                        {
                            Notification("Error", checkTimeResult, MyConstants.NOTIFY_ERROR);
                        }
                    }
                    else
                    {
                        Notification("Error", "Please recheck information", MyConstants.NOTIFY_ERROR);
                    }
                }
            }
            catch (Exception e)
            {
                Notification("Error", "No file be choosing", MyConstants.NOTIFY_ERROR);
            }
            ViewBag.Type = ((int)Session["Type"] != 2) ? "ScreenSaver" : "LockScreen";
            if (Session["Images"] != null)
            {
                var sessionCart = (List<ImageModel>)Session["Images"];
                ViewBag.listImages = sessionCart.Where(x => x.Type == ViewBag.Type).ToList();
            }
            return View("Index");
        }

        #endregion

        #region ScreenSaver
        [HttpPost]
        public ActionResult UploadMultipleImages(MultipleImagesModel imagesFiles)
        {
            //Ensure model state is valid  
            bool check = true;
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (HttpPostedFileBase file in imagesFiles.files.files)
                    {
                        if (file != null)
                        {
                            var InputFileName = Path.GetFileName(file.FileName);
                            var ServerSavePath = Path.Combine(Server.MapPath(pathTempImages) + InputFileName);
                            //Save file to server folder  
                            file.SaveAs(ServerSavePath);
                            //Add a images
                            var images = new ImageModel();
                            //Store images obj
                            if (Session["Images"] != null)
                            {
                                imagesList = (List<ImageModel>)Session["Images"];
                                images.ID = imagesList.LastOrDefault().ID + 1;
                            }
                            else
                            {
                                imagesList = new List<ImageModel>();
                                images.ID = 1;
                            }
                            images.TimeFrom = imagesFiles.images.TimeFrom;
                            images.TimeTo = imagesFiles.images.TimeTo;
                            //Check time here
                            images.Type = "ScreenSaver";
                            images.Name = InputFileName;
                            images.Url = pathTempImages + InputFileName;
                            //checkTime
                            //var checkTimeResult = CheckTime(1, images.ID, images.TimeFrom, DateTime.MaxValue, images.Type);
                            if (images.TimeFrom < images.TimeTo && images.TimeFrom > DateTime.Now)
                            {
                                imagesList.Add(images);
                                Session["Images"] = imagesList;
                            }
                            else
                            {
                                Notification("Error", "TimeStart must greater than TimeNow and less than TimeEnd", MyConstants.NOTIFY_ERROR);
                                check = false;
                            }
                        }
                        else
                        {
                            Notification("Error", "No files being selected", MyConstants.NOTIFY_ERROR);
                            check = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Notification("Error", "Please check time correct!", MyConstants.NOTIFY_ERROR);
                    check = false;
                }
            }
            else
            {
                Notification("Error", "Please recheck infomation", MyConstants.NOTIFY_ERROR);
                check = false;
            }
            if (check == true)
                Notification("Notify", "Upload file successfuly", MyConstants.NOTIFY_SUCCESS);
            ViewBag.Type = "ScreenSaver";
            var sessionCart = (List<ImageModel>)Session["Images"];
            if (sessionCart != null)
                ViewBag.listImages = sessionCart.Where(x => x.Type == "ScreenSaver").ToList();
            ModelState.Clear();
            return View("Index");
        }
        #endregion

        [HttpGet]
        public ActionResult Edit(long id)
        {
            imagesList = (List<ImageModel>)Session["Images"];
            foreach (var item in imagesList)
                item.Url = pathTempImages + item.Name;
            ImageModel image = imagesList.SingleOrDefault(x => x.ID == id);
            ViewBag.Button = "Update";
            ViewBag.Type = ((int)Session["Type"] != 2) ? "ScreenSaver" : "LockScreen";
            ViewBag.listImages = imagesList.Where(x => x.Type == ViewBag.Type);
            return PartialView(image);
        }

        [HttpPost]
        public ActionResult Edit(ImageModel image)
        {
            var sessionCart = (List<ImageModel>)Session["Images"];
            bool check = true;
            var checkTimeResult = CheckTime(2, image.ID, image.TimeFrom, image.TimeTo, image.Type);
            if (image.Type == "ScreenSaver")
            {
                if (image.TimeFrom >= image.TimeTo && image.TimeFrom <= DateTime.Now)
                {
                    Notification("Error", "TimeStart must less than TimeNow and greater than TimeEnd.", MyConstants.NOTIFY_ERROR);
                    check = false;
                }
            }
            else if (checkTimeResult != "Oke")
            {
                Notification("Error", checkTimeResult, MyConstants.NOTIFY_ERROR);
                check = false;
            }
            if (check == true)
            {
                foreach (var item in sessionCart)
                {
                    if (item.ID == image.ID)
                    {
                        item.TimeFrom = image.TimeFrom;
                        item.TimeTo = image.TimeTo;
                    }
                }
                Notification("Notify", "Update image successfuly", MyConstants.NOTIFY_SUCCESS);
            }
            imagesList = (List<ImageModel>)Session["Images"];
            ViewBag.Type = ((int)Session["Type"] != 2) ? "ScreenSaver" : "LockScreen";
            ViewBag.listImages = imagesList.Where(x => x.Type == image.Type).ToList();
            return View("Index");
        }
        [HttpGet]
        public ActionResult DefaultImages()
        {
            //List<ImageModel> imagesListDefault;
            imagesListDefault = new List<ImageModel>();
            ImageModel images;
            //Add default ScreenSaver
            try
            {
                var imageNameSS = Path.GetFileName(Directory.GetFiles(pathDefaultScreenSaver)[0]);
                //if (!System.IO.File.Exists(Server.MapPath(pathTempImages) + imageNameSS))
                //System.IO.File.Copy(pathDefaultScreenSaver + imageNameSS, Server.MapPath(pathTempImages) + imageNameSS); // Copy to inner project foder to view
                images = new ImageModel();
                images.ID = 1;
                images.Url = pathTempImages + "default_temp_screensaver.jpg";
                images.Type = "ScreenSaver";
                imagesListDefault.Add(images);
                //Add default LockScreen
                var imageNameLS = Path.GetFileName(Directory.GetFiles(pathDefaultLockScreen)[0]);
                images = new ImageModel();
                images.ID = 2;
                images.Url = pathTempImages + "default_temp_lockscreen.jpg";
                images.Type = "LockScreen";
                imagesListDefault.Add(images);
                //return list
                ViewBag.listImages = imagesListDefault;
                Session["ImagesDefault"] = imagesListDefault;
                return View();
            }
            catch (Exception)
            {
                throw;
            }

        }
        [HttpPost]
        public ActionResult DefaultImages(HttpPostedFileBase file, HttpPostedFileBase file1, ImageModel hoang)
        {
            string type = "";
            string path = "";
            bool isDefaultLS = false;
            HttpPostedFileBase File = Request.Files["file"];
            HttpPostedFileBase File1 = Request.Files["file1"];
            try
            {
                if (CalculateMD5(Directory.GetFiles(pathLockScreen)[0]) == CalculateMD5(Directory.GetFiles(pathDefaultLockScreen)[0]))
                    isDefaultLS = true;
                if (File.ContentLength > 0)
                {
                    if (ModelState.IsValid)
                    {
                        //System.IO.File.Delete(pathDefaultScreenSaver);
                        string[] filePaths = Directory.GetFiles(pathDefaultScreenSaver);
                        foreach (string filePath in filePaths)
                            System.IO.File.Delete(filePath);

                        // Check if exists in images upload
                        if (System.IO.File.Exists(Path.Combine(Server.MapPath(pathTempImages), "default_temp_screensaver.jpg")))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath(pathTempImages), "default_temp_screensaver.jpg"));
                        }

                        //System.IO.Directory.Delete(pathDefaultScreenSaver, true);
                        //string _FileName = Path.GetFileName(file.FileName);
                        string _path = Path.Combine(pathDefaultScreenSaver, "default_temp_screensaver.jpg");
                        file.SaveAs(_path);
                        type = "ScreenSaver";
                        path = "default_temp_screensaver.jpg";
                        Notification("Success", "Change default images for " + type + " success", MyConstants.NOTIFY_SUCCESS);

                        //Move images
                        var imageNameSS = Path.GetFileName(Directory.GetFiles(pathDefaultScreenSaver)[0]);
                        if (!System.IO.File.Exists(Server.MapPath(pathTempImages) + imageNameSS))
                            System.IO.File.Copy(pathDefaultScreenSaver + imageNameSS, Server.MapPath(pathTempImages) + imageNameSS);

                        // If active images is being default images => Update images default
                        var imagesActiveScreenSaver = Path.GetFileName(Directory.GetFiles(pathScreenSaver)[0]);
                        if (imagesActiveScreenSaver == "default.jpg")
                        {
                            System.IO.File.Delete(Path.Combine(pathScreenSaver, "default.jpg"));
                            System.IO.File.Copy(Directory.GetFiles(pathDefaultScreenSaver)[0], pathScreenSaver + "default.jpg");
                        }

                    }
                }
                else if (File1.ContentLength > 0)
                {
                    if (ModelState.IsValid)
                    {
                        //System.IO.File.Delete(pathDefaultScreenSaver);
                        string[] filePaths = Directory.GetFiles(pathDefaultLockScreen);
                        foreach (string filePath in filePaths)
                            System.IO.File.Delete(filePath);
                        // Check if exists in images upload
                        if (System.IO.File.Exists(Path.Combine(Server.MapPath(pathTempImages), "default_temp_lockscreen.jpg")))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath(pathTempImages), "default_temp_lockscreen.jpg"));
                        }
                        //System.IO.Directory.Delete(pathDefaultScreenSaver, true);
                        //string _FileName = Path.GetFileName(file1.FileName);
                        string _path = Path.Combine(pathDefaultLockScreen, "default_temp_lockscreen.jpg");
                        file1.SaveAs(_path);
                        type = "LockScreen";
                        path = "default_temp_lockscreen.jpg";
                        Notification("Success", "Change default images for " + type + " success", MyConstants.NOTIFY_SUCCESS);

                        //Move images
                        var imageNameLS = Path.GetFileName(Directory.GetFiles(pathDefaultLockScreen)[0]);
                        if (!System.IO.File.Exists(Server.MapPath(pathTempImages) + imageNameLS))
                            System.IO.File.Copy(pathDefaultLockScreen + imageNameLS, Server.MapPath(pathTempImages) + imageNameLS); // Copy to inner project foder to view

                        // If active images is being default images => Update images default
                        if (isDefaultLS)
                        {
                            System.IO.File.Delete(Path.Combine(pathLockScreen, "default.jpg"));
                            System.IO.File.Copy(Directory.GetFiles(pathDefaultLockScreen)[0], pathLockScreen + "default.jpg");
                        }

                    }
                }
                else
                {
                    Notification("Error", "Please select file before.", MyConstants.NOTIFY_ERROR);
                }
            }
            catch (Exception e)
            {
                Notification("Error", e.ToString() + "Please contact with IT!", MyConstants.NOTIFY_ERROR);
                throw;
            }
            // Return list
            List<ImageModel> listDefault = (List<ImageModel>)Session["ImagesDefault"];
            listDefault.Where(w => w.Type == type).ToList().ForEach(f => f.Url = pathTempImages + path);
            ViewBag.listImages = listDefault;
            return View();
        }

        public string CheckTime(int type, long? id, DateTime timeFrom, DateTime? timeTo, string imageType)
        {
            if (Session["Images"] != null)
            {
                var sessionCart = (List<ImageModel>)Session["Images"];
                //type == 1: when upload new images
                //type == 2: when edit images
                if (type == 1 && sessionCart.Where(x => x.Type == imageType).LastOrDefault() != null)
                {
                    var timeFormBefore_newImages = (id != 1) ? sessionCart.Where(x => x.Type == imageType).LastOrDefault().TimeTo : DateTime.MinValue;
                    if (timeFormBefore_newImages >= timeFrom)
                        return "TimeStart being overlap with before Image";
                }
                if (type == 2)
                {
                    var currentItem = sessionCart.FirstOrDefault(x => x.ID == id);
                    //Get before - after
                    if (sessionCart.Where(x => x.Type == imageType).TakeWhile(x => x != currentItem).LastOrDefault() != null)
                    {
                        var timeFromBefore = sessionCart.Where(x => x.Type == imageType).TakeWhile(x => x != currentItem).LastOrDefault().TimeTo;
                        if (timeFromBefore >= timeFrom)
                        {
                            return "TimeStart being overlap with before Image";
                        }
                    }
                    if (sessionCart.Where(x => x.Type == imageType).SkipWhile(x => x != currentItem).Skip(1).FirstOrDefault() != null)
                    {
                        var timeToAfter = sessionCart.Where(x => x.Type == imageType).SkipWhile(x => x != currentItem).Skip(1).FirstOrDefault().TimeFrom;
                        if (timeToAfter <= timeTo)
                        {
                            return "TimeEnd being overlap with after Image";
                        }
                    }

                }
            }
            if (timeFrom >= timeTo)
                return "TimeEnd must greater than TimeStart";
            return "Oke";
        }
        public JsonResult GetTimeFrom()
        {
            //check when add1
            string timeFrom;
            if (Session["Images"] != null)
            {
                var sessionCart = (List<ImageModel>)Session["Images"];
                // check with type
                if (sessionCart.Where(x => x.Type == (((int)Session["Type"] != 2) ? "ScreenSaver" : "LockScreen")).LastOrDefault() != null)
                {
                    timeFrom = sessionCart.Where(x => x.Type == (((int)Session["Type"] != 2) ? "ScreenSaver" : "LockScreen")).LastOrDefault().TimeTo.AddMinutes(1).ToString();
                    return Json(new { value = timeFrom });
                }
            }
            return Json(new { value = DateTime.Now.AddMinutes(5).ToString() });
        }
        public JsonResult Done()
        {
            try
            {
                var imagesList = (List<ImageModel>)Session["Images"];
                if (imagesList != null)
                {
                    //Create list object - update url
                    List<ImageModel> newListImages = new List<ImageModel>();
                    newListImages = imagesList;
                    foreach (var item in newListImages)
                    {
                        if (!System.IO.File.Exists(pathImages + item.Name))
                        {
                            System.IO.File.Copy(Server.MapPath(pathTempImages) + item.Name, pathImages + item.Name);
                        }
                        item.Url = pathImages + item.Name;
                    }
                    //Save object to file.txt
                    ReadWriteCSV.WriteCSV(newListImages);
                    Session["Images"] = imagesList;
                    Notification("Notify", "Save list images successfuly", MyConstants.NOTIFY_SUCCESS);

                    // Start Schedule.
                    JobsSchedule.RemoveAllSchedule();
                    JobsSchedule.Start();
                    return Json(new { status = true });
                }
                else
                {
                    Notification("Error", "This list being empty", MyConstants.NOTIFY_ERROR);
                    return Json(new { status = false });
                }
            }
            catch (Exception e)
            {
                Notification("Error", "Have some error, Please contact with IT!", MyConstants.NOTIFY_ERROR);
                return Json(new { status = e.ToString() });
            }
        }

        public JsonResult DeleteAll()
        {
            try
            {
                string type = ((int)Session["Type"] != 2) ? "ScreenSaver" : "LockScreen";

                // Remove all images in ImagesUpload and Uploaded
                List<ImageModel> removeList = (List<ImageModel>)Session["Images"];
                foreach (var item in removeList.Where(x => x.Type == type).ToList())
                {
                    // Check if not exists other type 
                    if (!removeList.Exists(x => x.Type != type && x.Name == item.Name))
                    {
                        System.IO.File.Delete(Path.Combine(Server.MapPath(pathTempImages), item.Name));
                        System.IO.File.Delete(Path.Combine(pathImages, item.Name));
                    }
                    // Remove Active Images
                    if (item.TimeFrom < DateTime.Now && item.TimeTo > DateTime.Now)
                    {
                        if (item.Type == "ScreenSaver")
                            RemoveActiveToDefault(pathScreenSaver, item.Name, pathDefaultScreenSaver);
                        else
                            RemoveActiveToDefault(pathLockScreen, "default.jpg", pathDefaultLockScreen);
                    }
                }

                List<ImageModel> tempList = (List<ImageModel>)Session["Images"];
                tempList.RemoveAll(x => x.Type == type);
                Session["Images"] = tempList;
                //ReadWriteCSV.WriteCSV(new List<ImageModel>());
                ReadWriteCSV.WriteCSV(tempList);
                // Remove all schedule
                JobsSchedule.RemoveAllSchedule();
                Notification("Notify", "Remove all list images successfuly", MyConstants.NOTIFY_SUCCESS);
                return Json(new { status = true });
            }
            catch (Exception)
            {
                Notification("Error", "Remove all list images failure", MyConstants.NOTIFY_ERROR);
                return Json(new { status = false });
                throw;
            }

        }
        public JsonResult Delete(long id)
        {
            var sessionCart = (List<ImageModel>)Session["Images"];
            string type = ((int)Session["Type"] != 2) ? "ScreenSaver" : "LockScreen";

            // Remove that images: all information
            foreach (var item in sessionCart.Where(x => x.ID == id).ToList())
            {
                // Check if not exists other type 
                if (!sessionCart.Exists(x => x.Type != type && x.Name == item.Name))
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath(pathTempImages), item.Name));
                    System.IO.File.Delete(Path.Combine(pathImages, item.Name));
                }

                // Remove active images
                if (item.TimeFrom < DateTime.Now && item.TimeTo > DateTime.Now)
                {
                    if (item.Type == "ScreenSaver")
                        RemoveActiveToDefault(pathScreenSaver, item.Name, pathDefaultScreenSaver);
                    else
                        RemoveActiveToDefault(pathLockScreen, "default.jpg", pathDefaultLockScreen);
                }
                // Remove all schedule apply
                JobsSchedule.RemoveAllSchedule();
            }

            sessionCart.RemoveAll(x => x.ID == id);
            Session["Images"] = sessionCart;
            ReadWriteCSV.WriteCSV(sessionCart);
            // Restart schedule apply after delete that item
            JobsSchedule.Start();
            if (sessionCart.Count == 0)
            {
                Session["Images"] = null;
            }
            return Json(new
            {
                status = true
            });
        }
        /// <summary>
        /// Remove active images and change to default images if empty images
        /// </summary>
        /// <param name="pathRemove">path of active images</param>
        /// <param name="fileName">active images name</param>
        /// <param name="pathDefault">path of default images</param>
        public void RemoveActiveToDefault(string pathRemove, string fileName, string pathDefault)
        {
            // Remove in active forder
            try
            {
                //Remove that file
                if (System.IO.File.Exists(Path.Combine(pathRemove, fileName)))
                {
                    System.IO.File.Delete(Path.Combine(pathRemove, fileName));
                }
                //Check empty foder?
                if (System.IO.Directory.GetFiles(pathRemove).Length == 0)
                {
                    // Check time default

                    if (!System.IO.File.Exists(Path.Combine(pathRemove, "default.jpg")))
                    {
                        System.IO.File.Copy(Directory.GetFiles(pathDefault)[0], pathRemove + "default.jpg");
                    }
                }
            }
            catch (Exception)
            {
                Notification("Error", "Remove active image have some error, please contact with IT!", MyConstants.NOTIFY_ERROR);
            }
        }

        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}