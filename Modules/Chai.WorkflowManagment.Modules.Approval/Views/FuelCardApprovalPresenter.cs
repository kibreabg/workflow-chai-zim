using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeWeb;
using Chai.WorkflowManagment.CoreDomain.Request;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.Modules.Setting;
using Chai.WorkflowManagment.Modules.Admin;

namespace Chai.WorkflowManagment.Modules.Approval.Views
{
    public class FuelCardApprovalPresenter : Presenter<IFuelCardApprovalView>
    {

        // NOTE: Uncomment the following code if you want ObjectBuilder to inject the module controller
        //       The code will not work in the Shell module, as a module controller is not created by default
        //
        private ApprovalController _controller;
        private SettingController _settingController;
        private AdminController _admincontroller;
        private FuelCardRequest _fuelcardrequest;
        public FuelCardApprovalPresenter([CreateNew] ApprovalController controller, [CreateNew] SettingController settingcontroller, [CreateNew] AdminController admincontroller)
        {
            _controller = controller;
            _settingController = settingcontroller;
            _admincontroller = admincontroller;
        }

        public override void OnViewLoaded()
        {
            if (View.FuelCardRequestId > 0)
            {
                _controller.CurrentObject = _controller.GetFuelCardRequest(View.FuelCardRequestId);
            }
          CurrentFuelCardRequest = _controller.CurrentObject as FuelCardRequest;
        }
        public FuelCardRequest CurrentFuelCardRequest
        {
            get
            {
                if (_fuelcardrequest == null)
                {
                    int id = View.FuelCardRequestId;
                    if (id > 0)
                        _fuelcardrequest = _controller.GetFuelCardRequest(id);
                    else
                        _fuelcardrequest = new FuelCardRequest();
                }
                return _fuelcardrequest;
            }
            set { _fuelcardrequest = value; }
        }
        public override void OnViewInitialized()
        {
            if (_fuelcardrequest == null)
            {
                int id = View.FuelCardRequestId;
                if (id > 0)
                    _controller.CurrentObject = _controller.GetFuelCardRequest(id);
                else
                    _controller.CurrentObject = new FuelCardRequest();
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
       
        public ItemAccount GetItemAccount(int ItemAccountId)
        {
            return _settingController.GetItemAccount(ItemAccountId);
        }
        public void SaveOrUpdateFuelCardRequest(FuelCardRequest FuelCardRequest)
        {
            _controller.SaveOrUpdateEntity(FuelCardRequest);
        }
        public void CancelPage()
        {
            _controller.Navigate(String.Format("~/Setting/Default.aspx?{0}=3", AppConstants.TABID));
        }
        public void DeleteFuelCardRequest(FuelCardRequest FuelCardRequest)
        {
            _controller.DeleteEntity(FuelCardRequest);
        }
        public FuelCardRequest GetFuelCardRequestById(int id)
        {
            return _controller.GetFuelCardRequest(id);
        }
        public ApprovalSetting GetApprovalSetting(string RequestType, int value)
        {
            return _settingController.GetApprovalSettingforProcess(RequestType, value);
        }
        public ApprovalSetting GetApprovalSettingforProcess(string Requesttype, decimal value)
        {
            return _settingController.GetApprovalSettingforProcess(Requesttype, value);
        }

        public IList<FuelCardRequest> ListFuelCardRequests(string requestNo, string RequestDate, string ProgressStatus)
        {
            return _controller.ListFuelCardRequests(requestNo, RequestDate, ProgressStatus);
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




