using Chai.WorkflowManagment.CoreDomain.DataAccess;
using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.Shared.MailSender;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chai.WorkflowManagment.Modules.Request
{
    public class EmailJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (var wr = WorkspaceFactory.CreateReadOnly())
                {

                    IList<TravelAdvanceRequest> notExpensedTravelAdvances = wr.Query<TravelAdvanceRequest>(x => x.ExpenseLiquidationStatus == "Completed" && x.ExpenseLiquidationRequest == null && x.AppUser.IsActive == true, x => x.AppUser, x => x.Project.AppUser, x => x.ExpenseLiquidationRequest).ToList();

                    if (notExpensedTravelAdvances.Count != 0)
                    {
                        foreach (TravelAdvanceRequest notExpensedTravelAdvance in notExpensedTravelAdvances)
                        {
                            if (notExpensedTravelAdvance.AppUser != null && notExpensedTravelAdvance.Project != null)
                            {
                                EmailSender.Send(notExpensedTravelAdvance.AppUser.Email, "Please Liquidate your Travel Advance", "Your Travel Advance with Travel Advance No. " + notExpensedTravelAdvance.TravelAdvanceNo + " is still not yet liquidated!");
                                EmailSender.Send(notExpensedTravelAdvance.Project.AppUser.Email, "Travel Advance not yet Liquidated", "The Travel Advance requested by " + notExpensedTravelAdvance.AppUser.FullName + " with Travel Advance No. " + notExpensedTravelAdvance.TravelAdvanceNo + " is still not yet liquidated!");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, "MailJob");
                ExceptionUtility.NotifySystemOps(ex, "");
            }


        }
    }
}
