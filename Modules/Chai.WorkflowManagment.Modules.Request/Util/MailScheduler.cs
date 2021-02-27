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
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<EmailJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .ForJob(job)
                .WithIdentity("liquidationTrigger", "liquidationGroup")
                .StartNow()
                .WithCalendarIntervalSchedule(x => x
                    .WithIntervalInWeeks(1)
                    )
                .Build();

            bool flag = scheduler.CheckExists(trigger.JobKey);
            if (!flag)
            {
                scheduler.ScheduleJob(job, trigger);
            }
        }
    }
}
