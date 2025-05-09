using Chai.WorkflowManagment.CoreDomain;
using Chai.WorkflowManagment.CoreDomain.Approval;
using Chai.WorkflowManagment.CoreDomain.DataAccess;
using Chai.WorkflowManagment.CoreDomain.Request;
using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Services;
using Chai.WorkflowManagment.Shared.Navigation;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.CompositeWeb.Interfaces;
using Microsoft.Practices.ObjectBuilder;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Chai.WorkflowManagment.Modules.Approval
{
    public class ApprovalController : ControllerBase
    {
        private readonly IWorkspace _workspace;

        [InjectionConstructor]
        public ApprovalController([ServiceDependency] IHttpContextLocatorService httpContextLocatorService, [ServiceDependency] INavigationService navigationService)
            : base(httpContextLocatorService, navigationService)
        {
            _workspace = ZadsServices.Workspace;
        }
        public AppUser CurrentUser()
        {
            return GetCurrentUser();
        }
        public AppUser Approver(int position)
        {
            return _workspace.Single<AppUser>(x => x.EmployeePosition.Id == position);
        }
        public AppUser GetSuperviser(int superviser)
        {
            return _workspace.Single<AppUser>(x => x.Id == superviser);
        }
        #region CurrentObject
        public object CurrentObject
        {
            get
            {
                return GetCurrentContext().Session["CurrentObject"];
            }
            set
            {
                GetCurrentContext().Session["CurrentObject"] = value;
            }
        }
        #endregion
        #region Vehicle Approval
        public IList<VehicleRequest> ListVehicleRequests(string RequestNo, string RequestDate, string ProgressStatus)
        {
            string filterExpression = "";

            filterExpression = " SELECT  *  FROM VehicleRequests INNER JOIN AppUsers on AppUsers.Id=VehicleRequests.CurrentApprover Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id  AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When VehicleRequests.RequestNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When VehicleRequests.RequestDate = '" + RequestDate + "'  Then 1 END AND VehicleRequests.ProgressStatus='" + ProgressStatus + "' " +
                                   " AND  ((VehicleRequests.CurrentApprover = '" + CurrentUser().Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by VehicleRequests.Id Desc";

            return _workspace.SqlQuery<VehicleRequest>(filterExpression).ToList();
        }
        #endregion
        #region Cash Payment Approval
        public IList<CashPaymentRequest> ListCashPaymentRequests(string RequestNo, string RequestDate, string ProgressStatus, string Supplier)
        {
            string filterExpression = "";
            if (ProgressStatus == "InProgress")
            {
                filterExpression = " SELECT * FROM CashPaymentRequests " +
                                   " INNER JOIN AppUsers ON (AppUsers.Id = CashPaymentRequests.CurrentApprover) OR (AppUsers.EmployeePosition_Id = CashPaymentRequests.CurrentApproverPosition AND AppUsers.Id = '" + CurrentUser().Id + "') " +
                                   " LEFT JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 " +
                                   " LEFT JOIN Suppliers on CashPaymentRequests.Supplier_Id = Suppliers.Id " +
                                   " WHERE 1 = CASE WHEN '" + RequestNo + "' = '' Then 1 When CashPaymentRequests.VoucherNo = '" + RequestNo + "' Then 1 END " +
                                   " AND 1 = CASE WHEN '" + RequestDate + "' = '' Then 1 When CashPaymentRequests.RequestDate = '" + RequestDate + "' Then 1 END " +
                                   " AND 1 = CASE WHEN '" + Supplier + "' = '' THEN 1 WHEN CashPaymentRequests.Supplier_Id = '" + Supplier + "' THEN 1 END " +
                                   " AND CashPaymentRequests.ProgressStatus='" + ProgressStatus + "' " +
                                   " AND ((CashPaymentRequests.CurrentApprover = '" + CurrentUser().Id + "') OR (CashPaymentRequests.CurrentApproverPosition = '" + CurrentUser().EmployeePosition.Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) " +
                                   " ORDER BY CashPaymentRequests.Id DESC";
            }
            else if (ProgressStatus == "Not Retired" || ProgressStatus == "Retired")
            {
                filterExpression = " SELECT * FROM CashPaymentRequests " +
                                   " INNER JOIN AppUsers ON (AppUsers.Id = CashPaymentRequests.CurrentApprover) OR (AppUsers.EmployeePosition_Id = CashPaymentRequests.CurrentApproverPosition AND AppUsers.Id = '" + CurrentUser().Id + "') " +
                                   " LEFT JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 " +
                                   " LEFT JOIN Suppliers ON CashPaymentRequests.Supplier_Id = Suppliers.Id " +
                                   " WHERE 1 = CASE WHEN '" + RequestNo + "' = '' THEN 1 WHEN CashPaymentRequests.VoucherNo = '" + RequestNo + "'  Then 1 END " +
                                   " AND 1 = CASE WHEN '" + RequestDate + "' = '' THEN 1 WHEN CashPaymentRequests.RequestDate = '" + RequestDate + "'  Then 1 END " +
                                   " AND 1 = CASE WHEN '" + Supplier + "' = '' THEN 1 WHEN CashPaymentRequests.Supplier_Id = '" + Supplier + "' THEN 1 END " +
                                   " AND CashPaymentRequests.ProgressStatus = 'Completed' AND CashPaymentRequests.PaymentReimbursementStatus = '" + ProgressStatus + "' " +
                                   " AND  (CashPaymentRequests.CurrentApprover = '" + CurrentUser().Id + "') or (CashPaymentRequests.CurrentApproverPosition = '" + CurrentUser().EmployeePosition.Id + "') OR (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "') " +
                                   " ORDER BY CashPaymentRequests.Id DESC ";
            }
            else if (ProgressStatus == "Completed")
            {
                filterExpression = " SELECT * FROM CashPaymentRequests " +
                                   " INNER JOIN AppUsers ON (AppUsers.Id = CashPaymentRequests.CurrentApprover) OR (AppUsers.EmployeePosition_Id = CashPaymentRequests.CurrentApproverPosition AND AppUsers.Id = '" + CurrentUser().Id + "') " +
                                   " INNER JOIN CashPaymentRequestStatuses ON CashPaymentRequestStatuses.CashPaymentRequest_Id = CashPaymentRequests.Id " +
                                   " LEFT JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 " +
                                   " LEFT JOIN Suppliers ON CashPaymentRequests.Supplier_Id = Suppliers.Id " +
                                   " WHERE 1 = CASE WHEN '" + RequestNo + "' = '' THEN 1 WHEN CashPaymentRequests.VoucherNo = '" + RequestNo + "' THEN 1 END " +
                                   " AND 1 = CASE WHEN '" + RequestDate + "' = '' Then 1 WHEN CashPaymentRequests.RequestDate = '" + RequestDate + "' THEN 1 END " +
                                   " AND 1 = CASE WHEN '" + Supplier + "' = '' THEN 1 WHEN CashPaymentRequests.Supplier_Id = '" + Supplier + "' THEN 1 END " +
                                   " AND CashPaymentRequests.ProgressStatus='" + ProgressStatus + "' " +
                                   " AND (CashPaymentRequestStatuses.ApprovalStatus IS NOT NULL AND (CashPaymentRequestStatuses.Approver = '" + CurrentUser().Id + "') OR (CashPaymentRequestStatuses.ApproverPosition = '" + CurrentUser().EmployeePosition.Id + "') OR (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) " +
                                   " ORDER BY CashPaymentRequests.Id DESC ";
            }
            return _workspace.SqlQuery<CashPaymentRequest>(filterExpression).ToList();
        }
        #endregion
        #region Cost Sharing Approval
        public IList<CostSharingRequest> ListCostSharingRequests(string RequestNo, string RequestDate, string ProgressStatus)
        {
            string filterExpression = "";
            if (ProgressStatus == "InProgress")
            {
                filterExpression = " SELECT * FROM CostSharingRequests INNER JOIN AppUsers ON (AppUsers.Id = CostSharingRequests.CurrentApprover) OR (AppUsers.EmployeePosition_Id = CostSharingRequests.CurrentApproverPosition AND AppUsers.Id = '" + CurrentUser().Id + "') Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When CostSharingRequests.VoucherNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When CostSharingRequests.RequestDate = '" + RequestDate + "'  Then 1 END AND CostSharingRequests.ProgressStatus='" + ProgressStatus + "' " +
                                       " AND  ((CostSharingRequests.CurrentApprover = '" + CurrentUser().Id + "') or (CostSharingRequests.CurrentApproverPosition = '" + CurrentUser().EmployeePosition.Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by CostSharingRequests.Id DESC ";
            }
            else if (ProgressStatus == "Not Retired" || ProgressStatus == "Retired")

                filterExpression = " SELECT * FROM CostSharingRequests INNER JOIN AppUsers ON (AppUsers.Id = CostSharingRequests.CurrentApprover) OR (AppUsers.EmployeePosition_Id = CostSharingRequests.CurrentApproverPosition AND AppUsers.Id = '" + CurrentUser().Id + "') Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When CostSharingRequests.VoucherNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When CostSharingRequests.RequestDate = '" + RequestDate + "'  Then 1 END AND CostSharingRequests.ProgressStatus='Completed' AND CostSharingRequests.PaymentReimbursementStatus = '" + ProgressStatus + "'" +
                                       " AND  (CostSharingRequests.CurrentApprover = '" + CurrentUser().Id + "') or (CostSharingRequests.CurrentApproverPosition = '" + CurrentUser().EmployeePosition.Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "') order by CostSharingRequests.Id DESC ";
            else if (ProgressStatus == "Completed")
            {
                filterExpression = " SELECT * FROM CostSharingRequests INNER JOIN AppUsers ON (AppUsers.Id = CostSharingRequests.CurrentApprover) OR (AppUsers.EmployeePosition_Id = CostSharingRequests.CurrentApproverPosition AND AppUsers.Id = '" + CurrentUser().Id + "') INNER JOIN CostSharingRequestStatuses on CostSharingRequestStatuses.CostSharingRequest_Id = CostSharingRequests.Id  Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When CostSharingRequests.VoucherNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When CostSharingRequests.RequestDate = '" + RequestDate + "'  Then 1 END AND CostSharingRequests.ProgressStatus='" + ProgressStatus + "' " +
                                           " AND  (CostSharingRequestStatuses.ApprovalStatus Is not null  AND (CostSharingRequestStatuses.Approver = '" + CurrentUser().Id + "') OR (CostSharingRequestStatuses.ApproverPosition = '" + CurrentUser().EmployeePosition.Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by CostSharingRequests.Id DESC";
            }
            return _workspace.SqlQuery<CostSharingRequest>(filterExpression).ToList();
        }
        #endregion
        #region Bank Payment Approval
        public IList<BankPaymentRequest> ListBankPaymentRequests(string RequestNo, string RequestDate, string ProgressStatus)
        {
            string filterExpression = "";

            filterExpression = " SELECT * FROM BankPaymentRequests INNER JOIN AppUsers on AppUsers.Id = BankPaymentRequests.CurrentApprover Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When BankPaymentRequests.RequestNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When BankPaymentRequests.ProcessDate = '" + RequestDate + "'  Then 1 END AND BankPaymentRequests.ProgressStatus='" + ProgressStatus + "' " +
                                   " AND  ((BankPaymentRequests.CurrentApprover = '" + CurrentUser().Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by BankPaymentRequests.Id DESC";

            return _workspace.SqlQuery<BankPaymentRequest>(filterExpression).ToList();
        }
        #endregion
        #region Operational Control Approval
        public IList<OperationalControlRequest> ListOperationalControlRequests(string RequestNo, string RequestDate, string ProgressStatus, string Supplier)
        {
            string filterExpression = "";
            if (ProgressStatus == "InProgress")
            {
                filterExpression = " SELECT * FROM OperationalControlRequests " +
                                   " LEFT JOIN AppUsers ON AppUsers.Id = OperationalControlRequests.CurrentApprover " +
                                   " LEFT JOIN AssignJobs ON AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 " +
                                   " WHERE 1 = Case when '" + RequestNo + "' = '' Then 1 When OperationalControlRequests.VoucherNo = '" + RequestNo + "' Then 1 END " +
                                       " AND 1 = Case when '" + RequestDate + "' = '' Then 1 When OperationalControlRequests.RequestDate = '" + RequestDate + "'  Then 1 END " +
                                       " AND 1 = CASE WHEN '" + Supplier + "' = '' THEN 1 WHEN OperationalControlRequests.Supplier_Id = '" + Supplier + "' THEN 1 END " +
                                       " AND OperationalControlRequests.ProgressStatus='" + ProgressStatus + "' " +
                                       " AND ((OperationalControlRequests.CurrentApprover = '" + CurrentUser().Id + "') " +
                                       " OR (OperationalControlRequests.CurrentApproverPosition = '" + CurrentUser().EmployeePosition.Id + "') " +
                                       " OR (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) " +
                                       " ORDER BY OperationalControlRequests.Id DESC ";
            }
            else if (ProgressStatus == "Not Retired" || ProgressStatus == "Retired")
            {
                filterExpression = " SELECT * FROM OperationalControlRequests " +
                                   " LEFT JOIN AppUsers ON AppUsers.Id = OperationalControlRequests.CurrentApprover " +
                                   " LEFT JOIN AssignJobs ON AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 " +
                                   " WHERE 1 = Case when '" + RequestNo + "' = '' Then 1 When OperationalControlRequests.VoucherNo = '" + RequestNo + "' Then 1 END " +
                                       " AND 1 = Case when '" + RequestDate + "' = '' Then 1 When OperationalControlRequests.RequestDate = '" + RequestDate + "'  Then 1 END " +
                                       " AND 1 = CASE WHEN '" + Supplier + "' = '' THEN 1 WHEN OperationalControlRequests.Supplier_Id = '" + Supplier + "' THEN 1 END " +
                                       " AND OperationalControlRequests.ProgressStatus = 'Completed' " +
                                       " AND OperationalControlRequests.PaymentReimbursementStatus = '" + ProgressStatus + "' " +
                                       " AND ((OperationalControlRequests.CurrentApprover = '" + CurrentUser().Id + "') " +
                                       " OR (OperationalControlRequests.CurrentApproverPosition = '" + CurrentUser().EmployeePosition.Id + "') " +
                                       " OR (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) " +
                                       " ORDER BY OperationalControlRequests.Id DESC ";
            }
            else if (ProgressStatus == "Completed")
            {
                filterExpression = " SELECT * FROM OperationalControlRequests " +
                                   " LEFT JOIN AppUsers ON AppUsers.Id = OperationalControlRequests.CurrentApprover " +
                                   " LEFT JOIN AssignJobs ON AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 " +
                                   " WHERE 1 = Case when '" + RequestNo + "' = '' Then 1 When OperationalControlRequests.VoucherNo = '" + RequestNo + "' Then 1 END " +
                                       " AND 1 = Case when '" + RequestDate + "' = '' Then 1 When OperationalControlRequests.RequestDate = '" + RequestDate + "'  Then 1 END " +
                                       " AND 1 = CASE WHEN '" + Supplier + "' = '' THEN 1 WHEN OperationalControlRequests.Supplier_Id = '" + Supplier + "' THEN 1 END " +
                                       " AND OperationalControlRequests.ProgressStatus='" + ProgressStatus + "' " +
                                       " AND ((OperationalControlRequests.CurrentApprover = '" + CurrentUser().Id + "') " +
                                       " OR (OperationalControlRequests.CurrentApproverPosition = '" + CurrentUser().EmployeePosition.Id + "') " +
                                       " OR (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) " +
                                       " ORDER BY OperationalControlRequests.Id DESC ";
            }
            return _workspace.SqlQuery<OperationalControlRequest>(filterExpression).ToList();
        }
        #endregion
        #region Payment Reimbursement Approval
        public IList<PaymentReimbursementRequest> ListPaymentReimbursementRequests(string RequestDate, string ProgressStatus)
        {
            string filterExpression = "";

            filterExpression = " SELECT * FROM PaymentReimbursementRequests INNER JOIN AppUsers ON AppUsers.Id = PaymentReimbursementRequests.CurrentApprover LEFT JOIN AssignJobs ON AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 WHERE 1 = CASE WHEN '" + RequestDate + "' = '' THEN 1 WHEN PaymentReimbursementRequests.RequestDate = '" + RequestDate + "'  THEN 1 END AND PaymentReimbursementRequests.ProgressStatus='" + ProgressStatus + "' " +
                                   " AND  ((PaymentReimbursementRequests.CurrentApprover = '" + CurrentUser().Id + "') OR (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) ORDER BY PaymentReimbursementRequests.Id DESC";

            return _workspace.SqlQuery<PaymentReimbursementRequest>(filterExpression).ToList();
        }
        #endregion
        #region Travel Advance Approval
        public IList<TravelAdvanceRequest> ListTravelAdvanceRequests(string RequestNo, string Requester, string RequestDate, string ProgressStatus)
        {
            string filterExpression = "";
            if (!ProgressStatus.Equals("Completed"))
            {

                filterExpression = " SELECT * FROM TravelAdvanceRequests " +
                                   " INNER JOIN AppUsers ON (AppUsers.Id = TravelAdvanceRequests.CurrentApprover) OR (AppUsers.EmployeePosition_Id = TravelAdvanceRequests.CurrentApproverPosition AND AppUsers.Id = '" + CurrentUser().Id + "') " +
                                   " LEFT JOIN AssignJobs ON AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 " +
                                   " WHERE 1 = CASE WHEN '" + RequestNo + "' = '' THEN 1 WHEN TravelAdvanceRequests.TravelAdvanceNo = '" + RequestNo + "'  Then 1 END " +
                                   " AND 1 = CASE WHEN '" + Requester + "' = '' THEN 1 WHEN TravelAdvanceRequests.AppUser_Id = '" + Requester + "'  THEN 1 END " +
                                   " AND 1 = CASE WHEN '" + RequestDate + "' = '' THEN 1 WHEN TravelAdvanceRequests.RequestDate = '" + RequestDate + "'  Then 1 END " +
                                   " AND TravelAdvanceRequests.ProgressStatus = '" + ProgressStatus + "' " +
                                   " AND ((TravelAdvanceRequests.CurrentApprover = '" + CurrentUser().Id + "') OR (TravelAdvanceRequests.CurrentApproverPosition = '" + CurrentUser().EmployeePosition.Id + "') OR (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) " +
                                   " ORDER BY TravelAdvanceRequests.Id Desc";

            }
            else
            {
                filterExpression = " SELECT * FROM TravelAdvanceRequests " +
                                   " INNER JOIN AppUsers ON (AppUsers.Id = TravelAdvanceRequests.CurrentApprover) OR (AppUsers.EmployeePosition_Id = TravelAdvanceRequests.CurrentApproverPosition AND AppUsers.Id = '" + CurrentUser().Id + "') " +
                                   " INNER JOIN TravelAdvanceRequestStatuses ON TravelAdvanceRequestStatuses.TravelAdvanceRequest_Id = TravelAdvanceRequests.Id " +
                                   " LEFT JOIN AssignJobs ON AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 " +
                                   " WHERE 1 = CASE WHEN '" + RequestNo + "' = '' THEN 1 WHEN TravelAdvanceRequests.TravelAdvanceNo = '" + RequestNo + "'  THEN 1 END " +
                                   " AND 1 = CASE WHEN '" + Requester + "' = '' THEN 1 WHEN TravelAdvanceRequests.AppUser_Id = '" + Requester + "'  THEN 1 END " +
                                   " AND 1 = CASE WHEN '" + RequestDate + "' = '' THEN 1 WHEN TravelAdvanceRequests.RequestDate = '" + RequestDate + "'  THEN 1 END " +
                                   " AND TravelAdvanceRequests.ProgressStatus = '" + ProgressStatus + "' " +
                                   " AND (TravelAdvanceRequestStatuses.ApprovalStatus IS NOT NULL AND (TravelAdvanceRequestStatuses.Approver = '" + CurrentUser().Id + "') OR (TravelAdvanceRequestStatuses.ApproverPosition = '" + CurrentUser().EmployeePosition.Id + "') OR (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) " +
                                   " ORDER BY TravelAdvanceRequests.Id Desc ";
            }
            return _workspace.SqlQuery<TravelAdvanceRequest>(filterExpression).ToList();
        }
        #endregion
        #region Expense Liquidation Approval
        public IList<ExpenseLiquidationRequest> ListExpenseLiquidationRequests(string travelAdvNo, string requestDate, string progressStatus)
        {
            string filterExpression = " SELECT * FROM ExpenseLiquidationRequests " +
                   " INNER JOIN AppUsers ON (AppUsers.Id = ExpenseLiquidationRequests.CurrentApprover) OR (AppUsers.EmployeePosition_Id = ExpenseLiquidationRequests.CurrentApproverPosition AND AppUsers.Id = '" + CurrentUser().Id + "') " +
                   " INNER JOIN TravelAdvanceRequests ON TravelAdvanceRequests.Id = ExpenseLiquidationRequests.Id " +
                   " LEFT JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 " +
                   " WHERE 1 = CASE WHEN '" + travelAdvNo + "' = '' THEN 1 WHEN TravelAdvanceRequests.TravelAdvanceNo = '" + travelAdvNo + "'  THEN 1 END " +
                   " AND 1 = CASE WHEN '" + requestDate + "' = '' THEN 1 WHEN ExpenseLiquidationRequests.RequestDate = '" + requestDate + "'  Then 1 END " +
                   " AND AppUsers.UserName != 'bmukono' " +
                   " AND ExpenseLiquidationRequests.ProgressStatus='" + progressStatus + "' " +
                   " AND (ExpenseLiquidationRequests.CurrentStatus != 'Rejected' OR ExpenseLiquidationRequests.CurrentStatus IS NULL) " +
                   " AND ((ExpenseLiquidationRequests.CurrentApprover = '" + CurrentUser().Id + "') OR (ExpenseLiquidationRequests.CurrentApproverPosition = '" + CurrentUser().EmployeePosition.Id + "') OR (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) " +
                   " ORDER BY ExpenseLiquidationRequests.Id DESC";
            return _workspace.SqlQuery<ExpenseLiquidationRequest>(filterExpression).ToList();
        }
        public DataSet ExportTravelAdvance(int LiquidationId)
        {
            ReportDao re = new ReportDao();
            return re.ExportLiquidationReport(LiquidationId);
        }
        #endregion
        #region LeaveApproval
        public LeaveRequest GetLeaveRequest(int LeaveRequestId)
        {
            return _workspace.Single<LeaveRequest>(x => x.Id == LeaveRequestId);
        }
        public IList<LeaveRequest> ListLeaveRequests(string RequestNo, string RequestDate, string ProgressStatus)
        {
            string filterExpression = "";
            if (ProgressStatus != "Completed")
            {
                filterExpression = " SELECT  *  FROM LeaveRequests INNER JOIN AppUsers on AppUsers.Id=LeaveRequests.CurrentApprover  Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When LeaveRequests.RequestNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When LeaveRequests.RequestedDate = '" + RequestDate + "'  Then 1 END AND LeaveRequests.ProgressStatus='" + ProgressStatus + "' " +
                                       " AND  ((LeaveRequests.CurrentApprover = '" + CurrentUser().Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by LeaveRequests.Id DESC ";
            }
            else
            {
                filterExpression = " SELECT  *  FROM LeaveRequests INNER JOIN AppUsers on AppUsers.Id=LeaveRequests.CurrentApprover INNER JOIN LeaveRequestStatuses on LeaveRequestStatuses.LeaveRequest_Id = LeaveRequests.Id Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When LeaveRequests.RequestNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When LeaveRequests.RequestedDate = '" + RequestDate + "'  Then 1 END AND LeaveRequests.ProgressStatus='" + ProgressStatus + "'  " +
                                          " AND  ( LeaveRequestStatuses.ApprovalStatus Is not null AND (LeaveRequestStatuses.Approver = '" + CurrentUser().Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by LeaveRequests.Id DESC ";
            }

            return _workspace.SqlQuery<LeaveRequest>(filterExpression).ToList();
        }
        #endregion
        #region AssignJob
        public int GetAssignedUserbycurrentuser()
        {
            int userId = GetCurrentUser().Id;
            IList<AssignJob> AJ = _workspace.All<AssignJob>(x => x.AssignedTo == userId && x.Status == true).ToList();
            if (AJ.Count != 0)
            { return AJ[0].AssignedTo; }
            else
                return 0;
        }
        public int GetAssignedUserbycurrentuser(int userId)
        {
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
            if (AJ.Count != 0)
            {
                return AJ[0];
            }
            else
            { return null; }

        }
        public AssignJob GetAssignedJobbycurrentuser(int UserId)
        {
            // int userId = GetCurrentUser().Id;
            IList<AssignJob> AJ = _workspace.All<AssignJob>(x => x.AppUser.Id == UserId && x.Status == true).ToList();
            if (AJ.Count != 0)
            {
                return AJ[0];
            }
            else
            { return null; }
        }
        #endregion
        #region BidAnalysisApproval
        public BidAnalysisRequest GetBidAnalysisRequest(int BidAnalysisId)
        {
            return _workspace.Single<BidAnalysisRequest>(x => x.Id == BidAnalysisId);
        }
        public IList<BidAnalysisRequest> ListBidAnalysisRequests(string RequestNo, string RequestDate, string ProgressStatus)
        {
            string filterExpression;
            if (ProgressStatus != "Completed")
            {
                filterExpression = " SELECT  *  FROM BidAnalysisRequests INNER JOIN AppUsers on AppUsers.Id=BidAnalysisRequests.CurrentApprover  Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When BidAnalysisRequests.RequestNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When BidAnalysisRequests.RequestDate = '" + RequestDate + "'  Then 1 END AND BidAnalysisRequests.ProgressStatus='" + ProgressStatus + "' " +
                                       " AND  ((BidAnalysisRequests.CurrentApprover = '" + CurrentUser().Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by BidAnalysisRequests.Id DESC";
            }
            else
            {
                filterExpression = " SELECT  *  FROM BidAnalysisRequests INNER JOIN AppUsers on AppUsers.Id=BidAnalysisRequests.CurrentApprover INNER JOIN BidAnalysisRequestStatuses on BidAnalysisRequestStatuses.BidAnalysisRequest_Id = BidAnalysisRequests.Id Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When BidAnalysisRequests.RequestNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When BidAnalysisRequests.RequestDate = '" + RequestDate + "'  Then 1 END AND BidAnalysisRequests.ProgressStatus='" + ProgressStatus + "' AND " +
                                           "   (BidAnalysisRequestStatuses.ApprovalStatus Is not null AND (BidAnalysisRequestStatuses.Approver = '" + CurrentUser().Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by BidAnalysisRequests.Id DESC ";
            }
            return _workspace.SqlQuery<BidAnalysisRequest>(filterExpression).ToList();

        }
        public BAAttachment GetBAAttachment(int attachmentId)
        {
            return _workspace.Single<BAAttachment>(x => x.Id == attachmentId);
        }
        public int GetLastPurchaseOrderId()
        {
            if (_workspace.Last<PurchaseOrder>() != null)
            {
                return _workspace.Last<PurchaseOrder>().Id;
            }
            else { return 0; }
        }
        #endregion
        #region PurchaseApproval
        public PurchaseRequest GetPurchaseRequest(int PurchaseId)
        {
            return _workspace.Single<PurchaseRequest>(x => x.Id == PurchaseId);
            //x => x.Bidders.Select(y => y.ItemAccount), x => x.PurchaseRequestDetails.Select(z => z.project), x => x.Bidders.Select(z => z.Supplier), x => x.BidAnalysises.Bidders.Select(z => z.BidderItemDetails.Select(y => y.ItemAccount)), x => x.PurchaseOrders.PurchaseOrderDetails);
        }
        public IList<PurchaseRequest> ListPurchaseRequests(string RequestNo, string RequestDate, string ProgressStatus)
        {
            string filterExpression = "";

            if (ProgressStatus != "Completed")
            {
                filterExpression = " SELECT  *  FROM PurchaseRequests INNER JOIN AppUsers on AppUsers.Id=PurchaseRequests.CurrentApprover  Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When PurchaseRequests.RequestNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When PurchaseRequests.RequestedDate = '" + RequestDate + "'  Then 1 END AND PurchaseRequests.ProgressStatus='" + ProgressStatus + "' " +
                                       " AND  ((PurchaseRequests.CurrentApprover = '" + CurrentUser().Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by PurchaseRequests.Id DESC";
            }
            else
            {
                filterExpression = " SELECT  *  FROM PurchaseRequests INNER JOIN AppUsers on AppUsers.Id=PurchaseRequests.CurrentApprover INNER JOIN PurchaseRequestStatuses on PurchaseRequestStatuses.PurchaseRequest_Id = PurchaseRequests.Id Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When PurchaseRequests.RequestNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When PurchaseRequests.RequestedDate = '" + RequestDate + "'  Then 1 END AND PurchaseRequests.ProgressStatus='" + ProgressStatus + "' AND " +
                                           "   (PurchaseRequestStatuses.ApprovalStatus Is not null AND (PurchaseRequestStatuses.Approver = '" + CurrentUser().Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by PurchaseRequests.Id DESC ";
            }
            return _workspace.SqlQuery<PurchaseRequest>(filterExpression).ToList();
        }

        #endregion
        #region SoleVendorApproval
        public SoleVendorRequest GetSoleVendorRequest(int SoleVendorRequestId)
        {
            return _workspace.Single<SoleVendorRequest>(x => x.Id == SoleVendorRequestId);
        }
        public IList<SoleVendorRequest> ListSoleVendorRequests(string RequestNo, string RequestDate, string ProgressStatus)
        {
            string filterExpression = "";
            if (ProgressStatus != "Completed")
            {
                filterExpression = " SELECT  *  FROM SoleVendorRequests INNER JOIN AppUsers on AppUsers.Id=SoleVendorRequests.CurrentApprover  Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When SoleVendorRequests.RequestNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When SoleVendorRequests.RequestDate = '" + RequestDate + "'  Then 1 END AND SoleVendorRequests.ProgressStatus='" + ProgressStatus + "' " +
                                       " AND  ((SoleVendorRequests.CurrentApprover = '" + CurrentUser().Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by SoleVendorRequests.Id DESC ";
            }
            else
            {
                filterExpression = " SELECT  *  FROM SoleVendorRequests INNER JOIN AppUsers on AppUsers.Id=SoleVendorRequests.CurrentApprover INNER JOIN SoleVendorRequestStatuses on SoleVendorRequestStatuses.SoleVendorRequest_Id = SoleVendorRequests.Id Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When SoleVendorRequests.RequestNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When SoleVendorRequests.RequestDate = '" + RequestDate + "'  Then 1 END AND SoleVendorRequests.ProgressStatus='" + ProgressStatus + "'  " +
                                          " AND  ( SoleVendorRequestStatuses.ApprovalStatus Is not null AND (SoleVendorRequestStatuses.Approver = '" + CurrentUser().Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by SoleVendorRequests.Id DESC ";
            }

            return _workspace.SqlQuery<SoleVendorRequest>(filterExpression).ToList();
        }
        public int GetLastPurchaseOrderSoleVendorId()
        {
            if (_workspace.Last<PurchaseOrderSoleVendor>() != null)
            {
                return _workspace.Last<PurchaseOrderSoleVendor>().Id;
            }
            else { return 0; }
        }
        #endregion
        #region InventoryApproval
        public InventoryRequest GetInventoryRequest(int inventoryId)
        {
            return _workspace.Single<InventoryRequest>(x => x.Id == inventoryId);
        }
        public IList<InventoryRequest> ListInventoryRequests(string RequestNo, string RequestDate, string ProgressStatus)
        {
            string filterExpression;

            if (ProgressStatus != "Completed")
            {
                filterExpression = " SELECT  *  FROM InventoryRequests " +
                                   " INNER JOIN AppUsers on AppUsers.Id = InventoryRequests.CurrentApprover " +
                                   " LEFT JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 " +
                                   " WHERE 1 = Case when '" + RequestNo + "' = '' THEN 1 WHEN InventoryRequests.RequestNo = '" + RequestNo + "'  THEN 1 END " +
                                   " AND  1 = CASE WHEN '" + RequestDate + "' = '' THEN 1 WHEN InventoryRequests.RequestedDate = '" + RequestDate + "'  THEN 1 END " +
                                   " AND InventoryRequests.ProgressStatus='" + ProgressStatus + "' " +
                                   " AND ((InventoryRequests.CurrentApprover = '" + CurrentUser().Id + "') OR (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) " +
                                   " ORDER BY InventoryRequests.Id DESC";
            }
            else
            {
                filterExpression = " SELECT  *  FROM InventoryRequests " +
                                   " INNER JOIN AppUsers on AppUsers.Id = InventoryRequests.CurrentApprover " +
                                   " INNER JOIN InventoryRequestStatuses on InventoryRequestStatuses.InventoryRequest_Id = InventoryRequests.Id " +
                                   " LEFT JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 " +
                                   " WHERE 1 = Case when '" + RequestNo + "' = '' Then 1 When InventoryRequests.RequestNo = '" + RequestNo + "'  Then 1 END " +
                                   " AND 1 = CASE WHEN '" + RequestDate + "' = '' Then 1 When InventoryRequests.RequestedDate = '" + RequestDate + "'  Then 1 END " +
                                   " AND InventoryRequests.ProgressStatus = '" + ProgressStatus + "' AND " +
                                   " (InventoryRequestStatuses.ApprovalStatus IS NOT NULL AND " +
                                   " (InventoryRequestStatuses.Approver = '" + CurrentUser().Id + "') OR (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) " +
                                   " ORDER BY InventoryRequests.Id DESC ";
            }
            return _workspace.SqlQuery<InventoryRequest>(filterExpression).ToList();
        }

        #endregion


        #region FuelCardApproval
        public FuelCardRequest GetFuelCardRequest(int FuelCardRequestId)
        {
            return _workspace.Single<FuelCardRequest>(x => x.Id == FuelCardRequestId);
        }
        public IList<FuelCardRequest> ListFuelCardRequests(string RequestNo, string RequestDate, string ProgressStatus)
        {
            string filterExpression = "";
            if (ProgressStatus != "Completed")
            {
                filterExpression = " SELECT  *  FROM FuelCardRequests INNER JOIN AppUsers on AppUsers.Id=FuelCardRequests.CurrentApprover  Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When FuelCardRequests.RequestNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When FuelCardRequests.RequestDate = '" + RequestDate + "'  Then 1 END AND FuelCardRequests.ProgressStatus='" + ProgressStatus + "' " +
                                       " AND  ((FuelCardRequests.CurrentApprover = '" + CurrentUser().Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by FuelCardRequests.Id DESC ";
            }
            else
            {
                filterExpression = " SELECT  *  FROM FuelCardRequests INNER JOIN AppUsers on AppUsers.Id=FuelCardRequests.CurrentApprover INNER JOIN FuelCardRequestStatuses on FuelCardRequestStatuses.FuelCardRequest_Id = FuelCardRequests.Id Left JOIN AssignJobs on AssignJobs.AppUser_Id = AppUsers.Id AND AssignJobs.Status = 1 Where 1 = Case when '" + RequestNo + "' = '' Then 1 When FuelCardRequests.RequestNo = '" + RequestNo + "'  Then 1 END And  1 = Case when '" + RequestDate + "' = '' Then 1 When FuelCardRequests.RequestDate = '" + RequestDate + "'  Then 1 END AND FuelCardRequests.ProgressStatus='" + ProgressStatus + "'  " +
                                          " AND  (FuelCardRequestStatuses.ApprovalStatus Is not null AND (FuelCardRequestStatuses.Approver = '" + CurrentUser().Id + "') or (AssignJobs.AssignedTo = '" + GetAssignedUserbycurrentuser() + "')) order by FuelCardRequests.Id DESC ";
            }

            return _workspace.SqlQuery<FuelCardRequest>(filterExpression).ToList();
        }

        #endregion
        #region Entity Manipulation
        public void SaveOrUpdateEntity<T>(T item) where T : class
        {
            IEntity entity = (IEntity)item;
            if (entity.Id == 0)
                _workspace.Add<T>(item);
            else
                _workspace.Update<T>(item);

            _workspace.CommitChanges();

            _workspace.Refresh(item);

        }
        public void DeleteEntity<T>(T item) where T : class
        {
            _workspace.Delete<T>(item);
            _workspace.CommitChanges();
            _workspace.Refresh(item);
        }

        public void Commit()
        {
            _workspace.CommitChanges();
        }
        #endregion        
    }
}
