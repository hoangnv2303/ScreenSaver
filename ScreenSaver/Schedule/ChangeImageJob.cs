using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using Quartz;
using Quartz.Impl;
using ScreenSaver.Controllers;
using ScreenSaver.Helper;
using ScreenSaver.Model;
namespace ScreenSaver.Schedule
{
    public class ChangeImageJob : BaseController, IJob
    {
        /// <summary>
        /// Read csv file;
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            // Read list from Database.
            var listImagesCSV = ReadWriteCSV.ReadCSV(pathCSV);

            // Change Active For LockScreen
            foreach (var item in listImagesCSV.Where(x => x.Type == "LockScreen").ToList())
            {
                // Check TimeTo duedate to change default images to active
                if (item.TimeTo > DateTime.Now)
                {
                    StartMove(-item.ID, item.TimeTo, Directory.GetFiles(pathDefaultLockScreen)[0], pathLockScreen + "default.jpg", item.Type);
                }
                // Check TimeForm to change images list to active images
                if (item.TimeFrom > DateTime.Now)
                {
                    StartMove(item.ID, item.TimeFrom, pathImages + item.Name, pathLockScreen + "default.jpg", item.Type);
                }
            }
            // Change Activ For ScreenSaver
            foreach (var item in listImagesCSV.Where(x => x.Type == "ScreenSaver").ToList())
            {
                // Check TimeTo duedate to change default images to active
                if (item.TimeTo > DateTime.Now)
                {
                    StartRemove(-item.ID, item.TimeTo, pathScreenSaver, item.Name);
                }
                // Check TimeStart to change images list to active images
                if (item.TimeFrom > DateTime.Now)
                {
                    StartMove(item.ID, item.TimeFrom, pathImages + item.Name, pathScreenSaver + item.Name, item.Type);
                }
            }
        }
        public static void StartMove(int id, DateTime time, string pathFrom, string pathTo, string type)
        {
            //khởi tạo schedule
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            //Tạo job
            IJobDetail job = JobBuilder.Create<MoveImages>()
                .WithIdentity("moveJob" + id, "job") // name "myJob", group "group1"
                .UsingJobData("pathFrom", pathFrom)
                .UsingJobData("pathTo", pathTo)
                .UsingJobData("type", type)
                .Build();

            ITrigger trigger2 = TriggerBuilder.Create()
            .WithPriority(2)
            .WithIdentity("trigger" + id, "trigger")
            .StartAt(time)
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(0))
            .Build();
            //Gán trigger với job
            scheduler.ScheduleJob(job, trigger2);
        }

        public static void StartRemove(int id, DateTime time, string pathFile, string fileName)
        {
            //khởi tạo schedule
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            //Tạo job
            IJobDetail job = JobBuilder.Create<RemoveScreenSaver>()
                .WithIdentity("removeJob" + id, "job") // name "myJob", group "group1"
                .UsingJobData("pathFile", pathFile)
                .UsingJobData("fileName", fileName)
                .Build();

            ITrigger trigger2 = TriggerBuilder.Create()
            .WithPriority(2)
            .WithIdentity("trigger" + id, "trigger")
            .StartAt(time)
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(0))
            .Build();
            //Gán trigger với job
            scheduler.ScheduleJob(job, trigger2);
        }
    }
    public class MoveImages : BaseController, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string pathFrom = dataMap.GetString("pathFrom");
            string pathTo = dataMap.GetString("pathTo");
            string type = dataMap.GetString("type");
            //Move
            if (type == "LockScreen")
            {
                //Remove all before copy
                string[] filePath2 = Directory.GetFiles(pathLockScreen);
                foreach (string filePath in filePath2)
                    System.IO.File.Delete(filePath);
                System.IO.File.Copy(pathFrom, pathTo);
            }
            else
            {
                //Remove default.
                if (System.IO.File.Exists(Path.Combine(pathScreenSaver, "default.jpg")))
                    System.IO.File.Delete(Path.Combine(pathScreenSaver, "default.jpg"));
                //Copy file.
                if (!System.IO.File.Exists(pathTo))
                    System.IO.File.Copy(pathFrom, pathTo);
            }
        }

    }

    public class RemoveScreenSaver : BaseController, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string pathFile = dataMap.GetString("pathFile");
            string fileName = dataMap.GetString("fileName");
            try
            {
                //Remove that file
                if (System.IO.File.Exists(Path.Combine(pathFile, fileName)))
                {
                    System.IO.File.Delete(Path.Combine(pathFile, fileName));
                }
                //Check empty foder?
                if (System.IO.Directory.GetFiles(pathFile).Length == 0)
                {
                    if (!System.IO.File.Exists(Path.Combine(pathScreenSaver, "default.jpg")))
                    {
                        System.IO.File.Copy(Directory.GetFiles(pathDefaultScreenSaver)[0], pathScreenSaver + "default.jpg");
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }

}