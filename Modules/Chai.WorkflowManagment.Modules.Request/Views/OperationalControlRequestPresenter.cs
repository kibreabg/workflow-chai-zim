﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeWeb;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.Modules.Admin;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Modules.Setting;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared.MailSender;

namespace Chai.WorkflowManagment.Modules.Request.Views
{
    public class OperationalControlRequestPresenter : Presenter<IOperationalControlRequestView>
    {

        // NOTE: Uncomment the following code if you want ObjectBuilder to inject the module controller
        //       The code will not work in the Shell module, as a module controller is not created by default
        //
        private RequestController _controller;
        private AdminController _adminController;
        private SettingController _settingController;
        private OperationalControlRequest _OperationalControlRequest;
        public OperationalControlRequestPresenter([CreateNew] RequestController controller, AdminController adminController, SettingController settingController)
        {
            _controller = controller;
            _adminController = adminController;
            _settingController = settingController;
        }
        public override void OnViewLoaded()
        {
            if (View.GetOperationalControlRequestId > 0)
            {
                _controller.CurrentObject = _controller.GetOperationalControlRequest(View.GetOperationalControlRequestId);
            }
            CurrentOperationalControlRequest = _controller.CurrentObject as OperationalControlRequest;
        }
        public override void OnViewInitialized()
        {
            if (_OperationalControlRequest == null)
            {
                int id = View.GetOperationalControlRequestId;
                if (id > 0)
                    _controller.CurrentObject = _controller.GetOperationalControlRequest(id);
                else
                    _controller.CurrentObject = new OperationalControlRequest();
            }
        }
        public OperationalControlRequest CurrentOperationalControlRequest
        {
            get
            {
                if (_OperationalControlRequest == null)
                {
                    int id = View.GetOperationalControlRequestId;
                    if (id > 0)
                        _OperationalControlRequest = _controller.GetOperationalControlRequest(id);
                    else
                        _OperationalControlRequest = new OperationalControlRequest();
                }
                return _OperationalControlRequest;
            }
            set { _OperationalControlRequest = value; }


        }
        public IList<OperationalControlRequest> GetOperationalControlRequests()
        {
            return _controller.GetOperationalControlRequests();
        }
        private void SaveOperationalControlRequestStatus()
        {
            if (GetApprovalSetting(RequestType.OperationalControl_Request.ToString().Replace('_', ' '), 0) != null)
            {
                int i = 1;
                foreach (ApprovalLevel AL in GetApprovalSetting(RequestType.OperationalControl_Request.ToString().Replace('_', ' '), 0).ApprovalLevels)
                {
                    OperationalControlRequestStatus OCRS = new OperationalControlRequestStatus();
                    OCRS.OperationalControlRequest = CurrentOperationalControlRequest;
                    //All Approver positions must be entered into the database before the approval workflow could run effectively!
                    if (AL.EmployeePosition.PositionName == "Superviser/Line Manager")
                    {
                        if (CurrentUser().Superviser != 0)
                            OCRS.Approver = CurrentUser().Superviser.Value;
                        else
                        {
                            OCRS.ApprovalStatus = ApprovalStatus.Approved.ToString();
                            OCRS.Date = Convert.ToDateTime(DateTime.Today.Date.ToShortDateString());
                        }
                    }
                    else if (AL.EmployeePosition.PositionName == "Program Manager")
                    {
                        if (CurrentOperationalControlRequest.OperationalControlRequestDetails[0].Project.Id != 0)
                        {
                            OCRS.Approver = GetProject(CurrentOperationalControlRequest.OperationalControlRequestDetails[0].Project.Id).AppUser.Id;
                        }
                    }
                    else
                    {
                        if (Approver(AL.EmployeePosition.Id) != null)
                        {
                            if (AL.EmployeePosition.PositionName == "Analyst, Finance")
                            {
                                OCRS.ApproverPosition = AL.EmployeePosition.Id; //So that we can entertain more than one finance manager to handle the request
                            }
                            else
                            {
                                OCRS.Approver = Approver(AL.EmployeePosition.Id).Id;
                            }
                        }
                        else
                            OCRS.Approver = 0;
                    }
                    OCRS.WorkflowLevel = i;
                    i++;
                    CurrentOperationalControlRequest.OperationalControlRequestStatuses.Add(OCRS);
                }
            }
        }
        private void GetCurrentApprover()
        {
            if (CurrentOperationalControlRequest.OperationalControlRequestStatuses != null)
            {
                foreach (OperationalControlRequestStatus OCRS in CurrentOperationalControlRequest.OperationalControlRequestStatuses)
                {
                    if (OCRS.ApprovalStatus == null)
                    {
                        SendEmail(OCRS);
                        CurrentOperationalControlRequest.CurrentApprover = OCRS.Approver;
                        CurrentOperationalControlRequest.CurrentLevel = OCRS.WorkflowLevel;
                        CurrentOperationalControlRequest.CurrentStatus = OCRS.ApprovalStatus;
                        break;
                    }
                }
            }
        }
        public void SaveOrUpdateOperationalControlRequest()
        {
            OperationalControlRequest OperationalControlRequest = CurrentOperationalControlRequest;
            OperationalControlRequest.RequestNo = View.GetRequestNo;
            OperationalControlRequest.RequestDate = Convert.ToDateTime(DateTime.Today.ToShortDateString());
            OperationalControlRequest.Account = _settingController.GetAccount(View.GetBankAccountId);
            //OperationalControlRequest.Payee = View.GetPayee;
            OperationalControlRequest.Description = View.GetDescription;
            OperationalControlRequest.Supplier = _settingController.GetSupplier(View.GetSupplierId);
            
            OperationalControlRequest.PaymentMethod = View.GetPaymentMethod;
            OperationalControlRequest.VoucherNo = View.GetVoucherNo;
            OperationalControlRequest.ProgressStatus = ProgressStatus.InProgress.ToString();
            OperationalControlRequest.AppUser = _adminController.GetUser(CurrentUser().Id);
            OperationalControlRequest.PaymentReimbursementStatus = "Retired";
            OperationalControlRequest.ExportStatus = "Not Exported";
            OperationalControlRequest.TotalActualExpendture = OperationalControlRequest.TotalActualExpendture;
            OperationalControlRequest.PageType = View.GetPageType;
            if (CurrentOperationalControlRequest.OperationalControlRequestStatuses.Count == 0)
                SaveOperationalControlRequestStatus();
            GetCurrentApprover();

            _controller.SaveOrUpdateEntity(OperationalControlRequest);
        }
        public void SaveOrUpdateOperationalControlRequest(OperationalControlRequest OperationalControlRequest)
        {
            _controller.SaveOrUpdateEntity(OperationalControlRequest);
        }
        public void CancelPage()
        {
            _controller.Navigate(String.Format("~/Request/Default.aspx?{0}=3", AppConstants.TABID));
        }
        public void DeleteOperationalControlRequest(OperationalControlRequest OperationalControlRequest)
        {
            _controller.DeleteEntity(OperationalControlRequest);
        }
        public void DeleteOperationalControlRequestDetail(OperationalControlRequestDetail OperationalControlRequestDetail)
        {
            _controller.DeleteEntity(OperationalControlRequestDetail);
        }
        public OperationalControlRequest GetOperationalControlRequest(int id)
        {
            return _controller.GetOperationalControlRequest(id);
        }
        public int GetLastOperationalControlRequestId()
        {
            return _controller.GetLastOperationalControlRequestId();
        }
        public IList<OperationalControlRequest> ListOperationalControlRequests(string RequestNo, string RequestDate)
        {
            return _controller.ListOperationalControlRequests(RequestNo, RequestDate);
        }
        public OperationalControlRequestDetail GetOperationalControlRequestDetail(int CPRDId)
        {
            return _controller.GetOperationalControlRequestDetail(CPRDId);
        }
        public AppUser Approver(int Position)
        {
            return _controller.Approver(Position);
        }
        public IList<AppUser> GetUsers()
        {
            return _adminController.GetUsers();
        }
        public AppUser GetUser(int id)
        {
            return _adminController.GetUser(id);
        }

        public ItemAccount GetItemAccount(int ItemAccountId)
        {
            return _settingController.GetItemAccount(ItemAccountId);
        }
        public Project GetProject(int ProjectId)
        {
            return _settingController.GetProject(ProjectId);
        }
        public Grant GetGrant(int GrantId)
        {
            return _settingController.GetGrant(GrantId);
        }
        public IList<Project> ListProjects()
        {
            return _settingController.GetProjects();
        }
        public IList<Grant> ListGrants()
        {
            return _settingController.GetGrants();
        }
        public IList<Grant> GetGrantbyprojectId(int projectId)
        {
            return _settingController.GetProjectGrantsByprojectId(projectId);
        }
        public IList<ItemAccount> ListItemAccounts()
        {
            return _settingController.GetItemAccounts();
        }
        public IList<Account> GetBankAccounts()
        {
            return _settingController.GetAccounts();
        }
        public Account GetBankAccount(int id)
        {
            return _settingController.GetAccount(id);
        }
        public IList<Supplier> GetSuppliers()
        {
            return _settingController.GetSuppliers();
        }
        public IList<Beneficiary> GetBeneficiaries()
        {
            return _settingController.GetBeneficiaries();
        }


        public Beneficiary GetBeneficiary(int id)
        {
            return _settingController.GetBeneficiary(id);
        }

        public AppUser CurrentUser()
        {
            return _controller.GetCurrentUser();
        }
        public AppUser GetSuperviser(int superviser)
        {
            return _controller.GetSuperviser(superviser);
        }
        public ApprovalSetting GetApprovalSetting(string RequestType, decimal value)
        {
            return _settingController.GetApprovalSettingforProcess(RequestType, value);
        }
        private void SendEmail(OperationalControlRequestStatus OCRS)
        {
            if (GetSuperviser(OCRS.Approver).IsAssignedJob != true)
            {
                EmailSender.Send(GetSuperviser(OCRS.Approver).Email, "Bank Payment Request", (CurrentOperationalControlRequest.AppUser.FullName).ToUpper() + " Requests for Bank Payment");
            }
            else
            {
                EmailSender.Send(GetSuperviser(_controller.GetAssignedJobbycurrentuser(OCRS.Approver).AssignedTo).Email, "Bank Payment Request", (CurrentOperationalControlRequest.AppUser.FullName).ToUpper() + " Requests for Bank Payment");
            }
        }
        public CashPaymentRequest GetCashPaymentRequest(int paymentRequest)
        {
            return _controller.GetCashPaymentRequest(paymentRequest);
        }
        public CostSharingRequest GetCostSharingPaymentRequest(int paymentRequest)
        {
            return _controller.GetCostSharingRequest(paymentRequest);
        }
        #region Beneficary


        public void SaveOrUpdateBeneficiary(Beneficiary beneficiary)
        {
            _controller.SaveOrUpdateEntity(beneficiary);
        }
        public void DeleteBeneficiary(Beneficiary beneficiary)
        {
            _controller.DeleteEntity(beneficiary);
        }
        public Beneficiary GetBeneficiaryById(int id)
        {
            return _settingController.GetBeneficiary(id);
        }

        public IList<Beneficiary> ListBeneficiaries(string BeneficiaryName)
        {
            return _settingController.ListBeneficiaries(BeneficiaryName);

        }
        #endregion
        public void Commit()
        {
            _controller.Commit();
        }
    }
}




