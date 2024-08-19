using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Modules.Admin;
using Chai.WorkflowManagment.Modules.Setting;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.Shared.MailSender;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.Modules.Request.Views
{
    public class ExpenseLiquidationRequestPresenter : Presenter<IExpenseLiquidationRequestView>
    {
        private readonly RequestController _controller;
        private readonly AdminController _adminController;
        private readonly SettingController _settingController;
        private TravelAdvanceRequest _travelAdvanceRequest;
        public ExpenseLiquidationRequestPresenter([CreateNew] RequestController controller, AdminController adminController, SettingController settingController)
        {
            _controller = controller;
            _adminController = adminController;
            _settingController = settingController;
        }
        public override void OnViewLoaded()
        {
            if (View.GetTARequestId > 0)
            {
                _controller.CurrentObject = _controller.GetTravelAdvanceRequest(View.GetTARequestId);
            }
            CurrentTravelAdvanceRequest = _controller.CurrentObject as TravelAdvanceRequest;
            if (CurrentTravelAdvanceRequest != null && CurrentTravelAdvanceRequest.ExpenseLiquidationRequest == null)
            {
                CurrentTravelAdvanceRequest.ExpenseLiquidationRequest = new ExpenseLiquidationRequest();
            }
        }
        public override void OnViewInitialized()
        {
            // Nothing to implement here
        }
        public TravelAdvanceRequest CurrentTravelAdvanceRequest
        {
            get
            {
                if (_travelAdvanceRequest == null)
                {
                    int id = View.GetTARequestId;
                    if (id > 0)
                        _travelAdvanceRequest = _controller.GetTravelAdvanceRequest(id);
                    else
                        _travelAdvanceRequest = new TravelAdvanceRequest();
                }
                return _travelAdvanceRequest;
            }
            set
            {
                _travelAdvanceRequest = value;
            }
        }
        public IList<ExpenseLiquidationRequest> GetExpenseLiquidationRequests()
        {
            return _controller.GetExpenseLiquidationRequests();
        }
        private void SaveExpenseLiquidationRequestStatus()
        {
            if (GetApprovalSetting(RequestType.ExpenseLiquidation_Request.ToString().Replace('_', ' '), 0) != null)
            {
                int i = 1;
                foreach (ApprovalLevel AL in GetApprovalSetting(RequestType.ExpenseLiquidation_Request.ToString().Replace('_', ' '), 0).ApprovalLevels)
                {
                    ExpenseLiquidationRequestStatus ELRS = new ExpenseLiquidationRequestStatus();
                    ELRS.ExpenseLiquidationRequest = _travelAdvanceRequest.ExpenseLiquidationRequest;

                    if (AL.EmployeePosition.PositionName == "Superviser/Line Manager")
                    {
                        if (CurrentUser().Superviser != 0)
                            ELRS.Approver = CurrentUser().Superviser.Value;
                        else
                        {
                            ELRS.ApprovalStatus = ApprovalStatus.Approved.ToString();
                            ELRS.Date = Convert.ToDateTime(DateTime.Today.Date.ToShortDateString());
                        }
                    }
                    else if (AL.EmployeePosition.PositionName == "Program Manager")
                    {
                        if (_travelAdvanceRequest.ExpenseLiquidationRequest.ExpenseLiquidationRequestDetails[0].Project.Id != 0)
                        {
                            ELRS.Approver = GetProject(_travelAdvanceRequest.ExpenseLiquidationRequest.ExpenseLiquidationRequestDetails[0].Project.Id).AppUser.Id;
                        }
                    }
                    else
                    {
                        if (Approver(AL.EmployeePosition.Id) != null)
                        {
                            if (AL.EmployeePosition.PositionName == "Analyst, Finance")
                            {
                                ELRS.ApproverPosition = AL.EmployeePosition.Id; //So that we can entertain more than one finance manager to handle the request
                            }
                            else
                            {
                                ELRS.Approver = Approver(AL.EmployeePosition.Id).Id;
                            }
                        }
                        else
                            ELRS.Approver = 0;
                    }
                    ELRS.WorkflowLevel = i;
                    i++;
                    _travelAdvanceRequest.ExpenseLiquidationRequest.ExpenseLiquidationRequestStatuses.Add(ELRS);
                }
            }
        }
        private void GetCurrentApprover()
        {
            if (_travelAdvanceRequest.ExpenseLiquidationRequest.ExpenseLiquidationRequestStatuses != null)
            {
                foreach (ExpenseLiquidationRequestStatus ELRS in _travelAdvanceRequest.ExpenseLiquidationRequest.ExpenseLiquidationRequestStatuses)
                {
                    if (ELRS.ApprovalStatus == null || ELRS.ApprovalStatus == "Rejected")
                    {
                        SendEmail(ELRS);
                        CurrentTravelAdvanceRequest.ExpenseLiquidationRequest.CurrentApprover = ELRS.Approver;
                        CurrentTravelAdvanceRequest.ExpenseLiquidationRequest.CurrentLevel = ELRS.WorkflowLevel;
                        //CurrentTravelAdvanceRequest.ExpenseLiquidationRequest.CurrentStatus = ELRS.ApprovalStatus;
                        break;
                    }
                }
            }
        }
        public void SaveOrUpdateExpenseLiquidationRequest(int tarId)
        {

            CurrentTravelAdvanceRequest.ExpenseLiquidationRequest.RequestDate = Convert.ToDateTime(DateTime.Today.ToShortDateString());
            CurrentTravelAdvanceRequest.ExpenseLiquidationRequest.TravelAdvRequestDate = Convert.ToDateTime(View.GetTravelAdvReqDate);
            CurrentTravelAdvanceRequest.ExpenseLiquidationRequest.ExpenseType = View.GetExpenseType;
            CurrentTravelAdvanceRequest.ExpenseLiquidationRequest.Comment = View.GetComment;
            CurrentTravelAdvanceRequest.ExpenseLiquidationRequest.AdditionalComment = View.GetAdditionalComment;
            CurrentTravelAdvanceRequest.ExpenseLiquidationRequest.PaymentMethod = View.GetPaymentMethod;
            CurrentTravelAdvanceRequest.ExpenseLiquidationRequest.ProgressStatus = ProgressStatus.InProgress.ToString();
            CurrentTravelAdvanceRequest.ExpenseLiquidationRequest.CurrentStatus = null;
            CurrentTravelAdvanceRequest.ExpenseLiquidationRequest.TravelAdvanceRequest = _controller.GetTravelAdvanceRequest(tarId);
            CurrentTravelAdvanceRequest.ExportStatus = "Not Exported";
            if (CurrentTravelAdvanceRequest.ExpenseLiquidationRequest.ExpenseLiquidationRequestStatuses.Count == 0)
                SaveExpenseLiquidationRequestStatus();
            GetCurrentApprover();
            _controller.SaveOrUpdateEntity(CurrentTravelAdvanceRequest);
        }
        public void CancelPage()
        {
            _controller.Navigate(String.Format("~/Request/Default.aspx?{0}=3", AppConstants.TABID));
        }
        public void DeleteExpenseLiquidationRequest(ExpenseLiquidationRequest ExpenseLiquidationRequest)
        {
            _controller.DeleteEntity(ExpenseLiquidationRequest);
        }
        public ExpenseLiquidationRequest GetExpenseLiquidationRequest(int id)
        {
            return _controller.GetExpenseLiquidationRequest(id);
        }
        public IList<ExpenseLiquidationRequest> ListExpenseLiquidationRequests(string ExpenseType, string RequestDate)
        {
            return _controller.ListExpenseLiquidationRequests(ExpenseType, RequestDate);
        }
        public IList<TravelAdvanceRequest> ListTravelAdvancesNotExpensed()
        {
            return _controller.ListTravelAdvancesNotExpensed();
        }
        public AppUser Approver(int Position)
        {
            return _controller.Approver(Position);
        }
        public IList<Project> ListProjects()
        {
            return _settingController.GetProjects();
        }
        public IList<AppUser> GetUsers()
        {
            return _adminController.GetUsers();
        }
        public AppUser GetUser(int id)
        {
            return _adminController.GetUser(id);
        }
        public AppUser CurrentUser()
        {
            return _controller.GetCurrentUser();
        }
        public AppUser GetSuperviser(int superviser)
        {
            return _controller.GetSuperviser(superviser);
        }
        public ApprovalSetting GetApprovalSetting(string RequestType, int value)
        {
            return _settingController.GetApprovalSettingforProcess(RequestType, value);
        }
        public Project GetProject(int projectId)
        {
            return _settingController.GetProject(projectId);
        }
        public Grant GetGrant(int GrantId)
        {
            return _settingController.GetGrant(GrantId);
        }
        public IList<Grant> GetGrantbyprojectId(int projectId)
        {
            return _settingController.GetProjectGrantsByprojectId(projectId);

        }
        public ItemAccount GetItemAccount(int ItemAccountId)
        {
            return _settingController.GetItemAccount(ItemAccountId);
        }
        public ExpenseType GetExpenseType(int Id)
        {
            return _settingController.GetExpenseType(Id);
        }
        public IList<ExpenseType> GetExpenseTypes()
        {
            return _settingController.GetExpenseTypes();
        }
        public IList<ItemAccount> ListItemAccounts()
        {
            return _settingController.GetItemAccounts();
        }
        private void SendEmail(ExpenseLiquidationRequestStatus ELRS)
        {
            if (GetSuperviser(ELRS.Approver).IsAssignedJob != true)
            {
                EmailSender.Send(GetSuperviser(ELRS.Approver).Email, "Expense Liquidation Request", (CurrentTravelAdvanceRequest.AppUser.FullName).ToUpper() + " Requests for Expense Liquidation for Travel Advance No. '" + (CurrentTravelAdvanceRequest.TravelAdvanceNo).ToUpper() + "'");
            }
            else
            {
                EmailSender.Send(GetSuperviser(_controller.GetAssignedJobbycurrentuser(ELRS.Approver).AssignedTo).Email, "Expense Liquidation Request", (CurrentTravelAdvanceRequest.AppUser.FullName).ToUpper() + " Requests for Expense Liquidation  for Travel Advance No. '" + (CurrentTravelAdvanceRequest.TravelAdvanceNo).ToUpper() + "'");
            }
        }
        public void Commit()
        {
            _controller.Commit();
        }
    }
}




