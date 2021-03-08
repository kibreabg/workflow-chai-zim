using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chai.WorkflowManagment.Modules.Request
{
    public class MailScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();            

            TriggerKey liquidationTriggerKey = new TriggerKey("liquidationTrigger", "liquidationGroup");
            IJobDetail job = JobBuilder.Create<EmailJob>().Build();

            bool exists = scheduler.CheckExists(liquidationTriggerKey);
            if (!exists)
            {
                scheduler.Start();

                ITrigger trigger = TriggerBuilder.Create()
                .ForJob(job)
                .WithIdentity(liquidationTriggerKey)
                .StartNow()
                .WithCalendarIntervalSchedule(x => x
                    .WithIntervalInWeeks(1)
                    )
                .Build();

                scheduler.ScheduleJob(job, trigger);
            }
        }
    }
}
