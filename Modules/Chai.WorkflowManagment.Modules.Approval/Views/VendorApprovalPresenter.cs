using System;
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
using Chai.WorkflowManagment.Modules.Request;

namespace Chai.WorkflowManagment.Modules.Approval.Views
{
    public class VendorApprovalPresenter : Presenter<IVendorApprovalView>
    {
        private ApprovalController _controller;
        private Supplier _Supplier;
        private SettingController _settingController;
        private AdminController _adminController;

        public VendorApprovalPresenter([CreateNew] ApprovalController controller, SettingController settingController, AdminController adminController)
        {
            _controller = controller;
            _settingController = settingController;
            _adminController = adminController;
        }
        public override void OnViewLoaded()
        {
            if (View.GetSupplierId > 0)
            {
                _controller.CurrentObject = _settingController.GetSupplier(View.GetSupplierId);
            }
            CurrentSupplier = _controller.CurrentObject as Supplier;
        }
        public override void OnViewInitialized()
        {
            if (_Supplier == null)
            {
                int id = View.GetSupplierId;
                if (id > 0)
                    _controller.CurrentObject = _settingController.GetSupplier(id);
                else
                    _controller.CurrentObject = new Supplier();
            }
        }
        public Supplier CurrentSupplier
        {
            get
            {
                if (_Supplier == null)
                {
                    int id = View.GetSupplierId;
                    if (id > 0)
                        _Supplier = _settingController.GetSupplier(id);
                    else
                        _Supplier = new Supplier();
                }
                return _Supplier;
            }
            set { _Supplier = value; }
        }
        public AssignJob GetAssignedJobbycurrentuser()
        {
            return _controller.GetAssignedJobbycurrentuser();
        }
        public AppUser GetUser(int UserId)
        {
            return _adminController.GetUser(UserId);
        }
        public AppUser GetSuperviser(int superviser)
        {
            return _controller.GetSuperviser(superviser);
        }
        public Supplier GetSupplier(int reqId)
        {
            return _settingController.GetSupplier(reqId);
        }
        public SupplierType GetSupplierType(int supTypeId)
        {
            return _settingController.GetSupplierType(supTypeId);
        }
        public IList<SupplierType> GetSupplierTypes()
        {
            return _settingController.GetSupplierTypes();
        }
        public void SaveOrUpdateSupplier(Supplier Supplier)
        {
            _controller.SaveOrUpdateEntity(Supplier);
        }
        public IList<Supplier> ListSuppliers(string RequestNo, string RequestDate, string ProgressStatus)
        {
            return _settingController.ListSuppliers(RequestNo, RequestDate, ProgressStatus);
        }
        public AppUser CurrentUser()
        {
            return _controller.GetCurrentUser();
        }
        public IList<AppUser> GetAppUsersByEmployeePosition(int employeePosition)
        {
            return _settingController.GetAppUsersByEmployeePosition(employeePosition);
        }
        public void navigate(string url)
        {
            _controller.Navigate(url);
        }
        public ApprovalSetting GetApprovalSettingforProcess(string Requesttype, decimal value)
        {
            return _settingController.GetApprovalSettingforProcess(Requesttype, value);
        }
        public IList<Account> GetAccounts()
        {
            return _settingController.GetAccounts();
        }
        public Account GetAccount(int Id)
        {
            return _settingController.GetAccount(Id);
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
        }        
    }
}




