using System;
using System.Web.SessionState;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.CompositeWeb.Interfaces;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeWeb.Web;
using System.Linq;
using System.Linq.Expressions;

using Chai.WorkflowManagment.CoreDomain;
using Chai.WorkflowManagment.CoreDomain.DataAccess;
using Chai.WorkflowManagment.CoreDomain.Admins;
using Chai.WorkflowManagment.Shared.Navigation;
using Chai.WorkflowManagment.Services;
using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.CoreDomain.Request;
using Chai.WorkflowManagment.CoreDomain.Users;
using System.Collections.Generic;
using Chai.WorkflowManagment.Enums;

namespace Chai.WorkflowManagment.Modules.Shell
{
    public class ShellController : ControllerBase
    {
        private IWorkspace _workspace;
        private int currentUser;
        [InjectionConstructor]
        public ShellController([ServiceDependency] IHttpContextLocatorService httpContextLocatorService,
           [ServiceDependency] INavigationService navigationService)
            : base(httpContextLocatorService,navigationService)
        {
            _workspace = ZadsServices.Workspace;
        }
        public int GetAssignedUserbycurrentuser()
        {
            int userId = GetCurrentUser().Id;
            IList<AssignJob> AJ = _workspace.All<AssignJob>(x => x.AssignedTo == userId && x.Status == true).ToList();
            if (AJ.Count != 0)
            { return AJ[0].AssignedTo; }
            else
                return 0;
        }
        public AssignJob GetAssignedJobbycurrentuser()
        {
            int userId = GetCurrentUser().Id;
            IList<AssignJob> AJ = _workspace.All<AssignJob>(x => x.AssignedTo == userId && x.Status == true).ToList();
                    return AJ[0]; 
           

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeid"></param>
        /// <returns></returns>
        public Node ActiveNode(int nodeid)
        {
            using (var vr = WorkspaceFactory.CreateReadOnly())
            {
                return vr.Single<Node>(x => x.Id == nodeid, x => x.NodeRoles.Select(y => y.Role));     
            }
        }

        public Tab ActiveTab(int tabid)
        {
            using (var vr = WorkspaceFactory.CreateReadOnly())
            {
                return vr.Single<Tab>(x => x.Id == tabid, x => x.PocModule, x => x.TabRoles.Select(z => z.Role), x => x.TaskPans.Select(y => y.TaskPanNodes.Select(w => w.Node.PocModule)), x => x.TaskPans.Select(y => y.TaskPanNodes.Select(w => w.Node.NodeRoles.Select(a => a.Role) )));
            }
        }
        #region ReimbersmentStatus

        public int GetCashPaymentReimbersment()
        {
            currentUser = GetCurrentUser().Id;
            int Count = 0;
            Count = WorkspaceFactory.CreateReadOnly().Count<CashPaymentRequest>(x => x.PaymentReimbursementStatus == "Not Retired" && x.ProgressStatus == "Completed");
            if (Count != 0)
                return Count;
            else
                return 0;

        }
        public int GetBankPaymentReimbersment()
        {
            currentUser = GetCurrentUser().Id;
            int Count = 0;
            Count = WorkspaceFactory.CreateReadOnly().Count<OperationalControlRequest>(x => x.PaymentReimbursementStatus == "Not Retired" && x.ProgressStatus == "Completed");
            if (Count != 0)
                return Count;
            else
                return 0;

        }
        public int GetCostSharingPaymentReimbersment()
        {
            currentUser = GetCurrentUser().Id;
            int Count = 0;
            Count = WorkspaceFactory.CreateReadOnly().Count<CostSharingRequest>(x => x.PaymentReimbursementStatus == "Not Retired" && x.ProgressStatus == "Completed");
            if (Count != 0)
                return Count;
            else
                return 0;

        }
        #endregion
        #region MyTasks
        public int GetLeaveTasks()
        {
            currentUser = GetCurrentUser().Id;
            string filterExpression = "";
            filterExpression = " SELECT  *  FROM LeaveRequests INNER JOIN AppUsers on AppUsers.Id=LeaveRequests.CurrentApprover Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where LeaveRequests.ProgressStatus='InProgress' " +
                                   " AND  ((LeaveRequests.CurrentApprover = '" + currentUser + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by LeaveRequests.Id ";

            return _workspace.SqlQuery<LeaveRequest>(filterExpression).Count();
        }
        public int GetVehicleTasks()
        {
            string filterExpression = "";

            filterExpression = " SELECT  *  FROM VehicleRequests INNER JOIN AppUsers on AppUsers.Id=VehicleRequests.CurrentApprover Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1  Where VehicleRequests.ProgressStatus='InProgress' " +
                                   " AND  (VehicleRequests.CurrentApprover = '" + currentUser + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "') order by VehicleRequests.Id ";

            return _workspace.SqlQuery<VehicleRequest>(filterExpression).Count();
        }
       
       
        public int GetCashPaymentRequestTasks()
        {
            currentUser = GetCurrentUser().Id;
            string filterExpression = "";

            filterExpression = " SELECT * FROM CashPaymentRequests INNER JOIN AppUsers on AppUsers.Id = CashPaymentRequests.CurrentApprover Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where CashPaymentRequests.ProgressStatus='InProgress' " +
                                   " AND  ((CashPaymentRequests.CurrentApprover = '" + currentUser + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by CashPaymentRequests.Id ";

            return _workspace.SqlQuery<CashPaymentRequest>(filterExpression).Count();
        }
        public int GetCostSharingRequestTasks()
        {
            currentUser = GetCurrentUser().Id;
            string filterExpression = "";

            filterExpression = " SELECT * FROM CostSharingRequests INNER JOIN AppUsers on AppUsers.Id = CostSharingRequests.CurrentApprover Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where CostSharingRequests.ProgressStatus='InProgress' " +
                                   " AND  ((CostSharingRequests.CurrentApprover = '" + currentUser + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by CostSharingRequests.Id ";

            return _workspace.SqlQuery<CostSharingRequest>(filterExpression).Count();
        }
        public int GetTravelAdvanceRequestTasks()
        {
            currentUser = GetCurrentUser().Id;
            string filterExpression = "";

            filterExpression = " SELECT * FROM TravelAdvanceRequests INNER JOIN AppUsers on AppUsers.Id = TravelAdvanceRequests.CurrentApprover Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where TravelAdvanceRequests.ProgressStatus='InProgress' " +
                                   " AND  ((TravelAdvanceRequests.CurrentApprover = '" + currentUser + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by TravelAdvanceRequests.RequestDate ";

            return _workspace.SqlQuery<TravelAdvanceRequest>(filterExpression).Count();
        }
        public int GetPurchaseRequestsTasks()
        {
            currentUser = GetCurrentUser().Id;
            string filterExpression = "";

            filterExpression = " SELECT  *  FROM PurchaseRequests INNER JOIN AppUsers on AppUsers.Id=PurchaseRequests.CurrentApprover Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where PurchaseRequests.ProgressStatus='InProgress' " +
                                   " AND  ((PurchaseRequests.CurrentApprover = '" + currentUser + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by PurchaseRequests.Id ";

            return _workspace.SqlQuery<PurchaseRequest>(filterExpression).Count();

        }
        public int GetReviewExpenseLiquidationRequestsTasks()
        {
            currentUser = GetCurrentUser().Id;
            string filterExpression = "";

            filterExpression = " SELECT * FROM ExpenseLiquidationRequests INNER JOIN AppUsers on AppUsers.Id = ExpenseLiquidationRequests.CurrentApprover Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id  Where ExpenseLiquidationRequests.ProgressStatus='InProgress' " +
                                   " AND  ((ExpenseLiquidationRequests.CurrentApprover = '" + currentUser + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by ExpenseLiquidationRequests.Id ";

            return _workspace.SqlQuery<ExpenseLiquidationRequest>(filterExpression).Count();
        }
       
        public int GetExpenseLiquidationRequestsTasks()
        {
            currentUser = GetCurrentUser().Id;
            int Count = 0;
            Count = WorkspaceFactory.CreateReadOnly().Count<TravelAdvanceRequest>(x => x.AppUser.Id == currentUser && x.ExpenseLiquidationStatus == "Completed");
            if (Count != 0)
                return Count;
            else
                return 0;

        }
        public int GetBankPaymentTasks()
        {
            currentUser = GetCurrentUser().Id;
            string filterExpression = "";

            filterExpression = " SELECT * FROM OperationalControlRequests INNER JOIN AppUsers on AppUsers.Id = OperationalControlRequests.CurrentApprover Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id  Where OperationalControlRequests.ProgressStatus='InProgress' " +
                                  " AND  ((OperationalControlRequests.CurrentApprover = '" + currentUser+ "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by OperationalControlRequests.Id ";

            return _workspace.SqlQuery<OperationalControlRequest>(filterExpression).Count();
        }
    
        #endregion
        #region MyRequests
        public int GetLeaveMyRequest()
        {
            currentUser = GetCurrentUser().Id;
            int Count = 0;
            Count = WorkspaceFactory.CreateReadOnly().Count<LeaveRequest>(x => x.Requester == currentUser && x.ProgressStatus == "InProgress");
            if (Count != 0)
                return Count;
            else
                return 0;

        }
        public int GetVehicleMyRequest()
        {
            currentUser = GetCurrentUser().Id;
            int Count = 0;
            Count = WorkspaceFactory.CreateReadOnly().Count<VehicleRequest>(x => x.AppUser.Id == currentUser && x.ProgressStatus == "InProgress");
            if (Count != 0)
                return Count;
            else
                return 0;

        }
        public int GetCashPaymentRequestMyRequests()
        {
            currentUser = GetCurrentUser().Id;
            int Count = 0;
            Count = WorkspaceFactory.CreateReadOnly().Count<CashPaymentRequest>(x => x.AppUser.Id == currentUser && x.ProgressStatus == "InProgress");
            if (Count != 0)
                return Count;
            else
                return 0;

        }
        public int GetCostSharingRequestMyRequests()
        {
            currentUser = GetCurrentUser().Id;
            int Count = 0;
            Count = WorkspaceFactory.CreateReadOnly().Count<CostSharingRequest>(x => x.AppUser.Id == currentUser && x.ProgressStatus == "InProgress");
            if (Count != 0)
                return Count;
            else
                return 0;

        }
        public int GetTravelAdvanceRequestMyRequest()
        {
            currentUser = GetCurrentUser().Id;
            int Count = 0;
            Count = WorkspaceFactory.CreateReadOnly().Count<TravelAdvanceRequest>(x => x.AppUser.Id == currentUser && x.ProgressStatus == "InProgress");
            if (Count != 0)
                return Count;
            else
                return 0;

        }
        public int GetPurchaseRequestsMyRequest()
        {
            currentUser = GetCurrentUser().Id;
            int Count = 0;
            Count = WorkspaceFactory.CreateReadOnly().Count<PurchaseRequest>(x => x.Requester == currentUser && x.ProgressStatus == "InProgress");
            if (Count != 0)
                return Count;
            else
                return 0;

        }
        public int GetBankRequestsMyRequest()
        {
            currentUser = GetCurrentUser().Id;
            int Count = 0;
            Count = WorkspaceFactory.CreateReadOnly().Count<OperationalControlRequest>(x => x.AppUser.Id == currentUser && x.ProgressStatus == "InProgress");
            if (Count != 0)
                return Count;
            else
                return 0;

        }
        #endregion
    }
}
