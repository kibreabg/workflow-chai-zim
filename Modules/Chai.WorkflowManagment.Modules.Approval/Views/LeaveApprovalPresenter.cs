﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeWeb;
using Chai.WorkflowManagment.CoreDomain.Request;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Modules.Admin;
using Chai.WorkflowManagment.Modules.Setting;
using Chai.WorkflowManagment.Modules.Request;

namespace Chai.WorkflowManagment.Modules.Approval.Views
{
    public class LeaveApprovalPresenter : Presenter<ILeaveApprovalView>
    {

        // NOTE: Uncomment the following code if you want ObjectBuilder to inject the module controller
        //       The code will not work in the Shell module, as a module controller is not created by default
        //
        private ApprovalController _controller;
        private RequestController _requestController;
        private SettingController _settingcontroller;
        private AdminController _admincontroller;
        private LeaveRequest _leaverequest;
        public LeaveApprovalPresenter([CreateNew] ApprovalController controller, [CreateNew] RequestController requestController, [CreateNew] SettingController settingController, [CreateNew] AdminController adminController)
        {
            _controller = controller;
            _settingcontroller = settingController;
            _admincontroller = adminController;
            _requestController = requestController;
        }

        public override void OnViewLoaded()
        {
            if (View.LeaveRequestId > 0)
            {
                _controller.CurrentObject = _controller.GetLeaveRequest(View.LeaveRequestId);
            }
            CurrentLeaveRequest = _controller.CurrentObject as LeaveRequest;
        }
        public LeaveRequest CurrentLeaveRequest
        {
            get
            {
                if (_leaverequest == null)
                {
                    int id = View.LeaveRequestId;
                    if (id > 0)
                        _leaverequest = _controller.GetLeaveRequest(id);
                    else
                        _leaverequest = new LeaveRequest();
                }
                return _leaverequest;
            }
            set { _leaverequest = value; }
        }
        public override void OnViewInitialized()
        {
            if (_leaverequest == null)
            {
                int id = View.LeaveRequestId;
                if (id > 0)
                    _controller.CurrentObject = _controller.GetLeaveRequest(id);
                else
                    _controller.CurrentObject = new LeaveRequest();
            }
        }
        public AppUser Approver(int Position)
        {
            return _controller.Approver(Position);
        }
        public AppUser GetUser(int UserId)
        {
            return _admincontroller.GetUser(UserId);
        }
        public EmployeeLeave GetEmployeeLeave(int userId)
        {
            return _settingcontroller.GetActiveEmployeeLeaveRequest(userId, true);
        }
        public EmployeeLeave GetEmployeeLeaveforEdit(int userId)
        {
            return _settingcontroller.GetActiveEmployeeLeave(userId, true);
        }
        public void SaveOrUpdateLeaveRequest(LeaveRequest LeaveRequest)
        {
            _controller.SaveOrUpdateEntity(LeaveRequest);
            _controller.CurrentObject = null;
        }
        public void SaveOrUpdateEmployeeLeave(EmployeeLeave EmployeeLeave)
        {
            _controller.SaveOrUpdateEntity(EmployeeLeave);
        }
        private void CalculateLeaveTaken()
        {
            if (CurrentLeaveRequest.CurrentLevel == CurrentLeaveRequest.LeaveRequestStatuses.Count && CurrentLeaveRequest.ProgressStatus == ProgressStatus.Completed.ToString())
            {
                if (CurrentLeaveRequest.LeaveType.LeaveTypeName.Contains("Annual"))
                {
                    EmployeeLeave EL = GetEmployeeLeave(CurrentLeaveRequest.Requester);
                    if (EL != null)
                    {
                        EL.LeaveTaken = EL.LeaveTaken + CurrentLeaveRequest.RequestedDays;
                        SaveOrUpdateEmployeeLeave(EL);
                    }
                }
            }
        }
        public void CancelPage()
        {
            _controller.Navigate(String.Format("~/Setting/Default.aspx?{0}=3", AppConstants.TABID));
        }
        public void DeleteLeaveRequestg(LeaveRequest LeaveRequest)
        {
            _controller.DeleteEntity(LeaveRequest);
        }
        public LeaveRequest GetLeaveRequestById(int id)
        {
            return _controller.GetLeaveRequest(id);
        }
        public IList<LeaveRequest> GetAnnualLeaveRequestsByRequester(int requesterId)
        {
            return _requestController.GetAnnualLeaveRequestsByRequester(requesterId);
        }
        public ApprovalSetting GetApprovalSetting(string RequestType, int value)
        {
            return _settingcontroller.GetApprovalSettingforProcess(RequestType, value);
        }
        public ApprovalSetting GetApprovalSettingforProcess(string Requesttype, decimal value)
        {
            return _settingcontroller.GetApprovalSettingforProcess(Requesttype, value);
        }

        public IList<LeaveRequest> ListLeaveRequests(string requestNo, string RequestDate, string ProgressStatus)
        {
            return _controller.ListLeaveRequests(requestNo, RequestDate, ProgressStatus);

        }
        public LeaveType GetLeaveType(int Id)
        {
            return _settingcontroller.GetLeaveType(Id);
        }
        public AssignJob GetAssignedJobbycurrentuser()
        {
            return _controller.GetAssignedJobbycurrentuser();
        }
        public AppUser CurrentUser()
        {
            return _controller.GetCurrentUser();
        }
        public AssignJob GetAssignedJobbycurrentuser(int userId)
        {
            return _controller.GetAssignedJobbycurrentuser(userId);
        }
        public int GetAssignedUserbycurrentuser()
        {
            return _controller.GetAssignedUserbycurrentuser();
        }
        public void Commit()
        {
            _controller.Commit();
        }// TODO: Handle other view events and set state in the view
    }
}




