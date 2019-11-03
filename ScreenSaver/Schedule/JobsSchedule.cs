using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;

namespace ScreenSaver.Schedule
{
    public class JobsSchedule
    {
        public static void Start()
        {
            //khởi tạo schedule
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();
            //Tạo job
            IJobDetail job = JobBuilder.Create<ChangeImageJob>().Build();

            //Tạo trigger
            ITrigger trigger = TriggerBuilder.Create()
            .WithPriority(2)
            .WithIdentity("trigger", "trigger")
            //.WithSchedule(
            //CronScheduleBuilder.CronSchedule("0 0 0/1 * * ?"))

            //CronScheduleBuilder.CronSchedule("0 20  * * ?"))
            .StartAt(DateTime.Now)
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(0))

            .Build();
            //Gán trigger với job
            scheduler.ScheduleJob(job, trigger);
        }

        public static void RemoveAllSchedule()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            foreach (IScheduler scheduler in schedFact.AllSchedulers)
            {
                scheduler.Clear();
            }
        }
    }


}