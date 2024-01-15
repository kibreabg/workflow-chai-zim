using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Modules.Admin;
using Chai.WorkflowManagment.Modules.Setting;
using Chai.WorkflowManagment.Shared;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.Modules.Approval.Views
{
    public class InventoryApprovalPresenter : Presenter<IInventoryApprovalView>
    {
        private readonly ApprovalController _controller;
        private readonly SettingController _settingcontroller;
        private readonly AdminController _admincontroller;
        private InventoryRequest _purchaserequest;
        public InventoryApprovalPresenter([CreateNew] ApprovalController controller, [CreateNew] SettingController settingcontroller, [CreateNew] AdminController admincontroller)
        {
            _controller = controller;
            _settingcontroller = settingcontroller;
            _admincontroller = admincontroller;
        }

        public override void OnViewLoaded()
        {
            if (View.InventoryRequestId > 0)
            {
                _controller.CurrentObject = _controller.GetInventoryRequest(View.InventoryRequestId);
            }
            CurrentInventoryRequest = _controller.CurrentObject as InventoryRequest;
        }
        public InventoryRequest CurrentInventoryRequest
        {
            get
            {
                if (_purchaserequest == null)
                {
                    int id = View.InventoryRequestId;
                    if (id > 0)
                        _purchaserequest = _controller.GetInventoryRequest(id);
                    else
                        _purchaserequest = new InventoryRequest();
                }
                return _purchaserequest;
            }
            set { _purchaserequest = value; }
        }
        public override void OnViewInitialized()
        {
            if (_purchaserequest == null)
            {
                int id = View.InventoryRequestId;
                if (id > 0)
                    _controller.CurrentObject = _controller.GetInventoryRequest(id);
                else
                    _controller.CurrentObject = new InventoryRequest();
            }
        }
        public IList<ItemAccount> GetItemAccounts()
        {
            return _settingcontroller.GetItemAccounts();
        }
        public ItemAccount GetItemAccount(int Id)
        {
            return _settingcontroller.GetItemAccount(Id);
        }
        public AppUser Approver(int Position)
        {
            return _controller.Approver(Position);
        }
        public AppUser GetUser(int UserId)
        {
            return _admincontroller.GetUser(UserId);
        }
        public void SaveOrUpdateInventoryRequest(InventoryRequest InventoryRequest)
        {
            _controller.SaveOrUpdateEntity(InventoryRequest);
        }
        public AssignJob GetAssignedJobbycurrentuser(int userId)
        {
            return _controller.GetAssignedJobbycurrentuser(userId);
        }
        public int GetAssignedUserbycurrentuser()
        {
            return _controller.GetAssignedUserbycurrentuser();
        }
        public void CancelPage()
        {
            _controller.Navigate(String.Format("~/Setting/Default.aspx?{0}=3", AppConstants.TABID));
        }
        public void DeleteInventoryRequest(InventoryRequest InventoryRequest)
        {
            _controller.DeleteEntity(InventoryRequest);
        }
        public InventoryRequest GetInventoryRequestById(int id)
        {
            return _controller.GetInventoryRequest(id);
        }
        public ApprovalSetting GetApprovalSetting(string RequestType, int value)
        {
            return _settingcontroller.GetApprovalSettingforProcess(RequestType, value);
        }
        public IList<InventoryRequest> ListInventoryRequests(string requestNo, string RequestDate, string ProgressStatus)
        {
            return _controller.ListInventoryRequests(requestNo, RequestDate, ProgressStatus);
        }
        public IList<Supplier> GetSuppliers()
        {
            return _settingcontroller.GetSuppliers();
        }
        public IList<Supplier> GetSuppliers(int SupplierTypeId)
        {
            return _settingcontroller.GetSuppliers(SupplierTypeId);
        }
        public IList<SupplierType> GetSupplierTypes()
        {
            return _settingcontroller.GetSupplierTypes();
        }
        public Supplier GetSupplier(int Id)
        {
            return _settingcontroller.GetSupplier(Id);
        }
        public SupplierType GetSupplierType(int Id)
        {
            return _settingcontroller.GetSupplierType(Id);
        }
        public AppUser CurrentUser()
        {
            return _controller.GetCurrentUser();
        }
        public void Commit()
        {
            _controller.Commit();
        }
    }
}




