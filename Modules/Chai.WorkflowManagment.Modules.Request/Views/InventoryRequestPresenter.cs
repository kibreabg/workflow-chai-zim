using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Modules.Setting;
using Chai.WorkflowManagment.Shared;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.Modules.Request.Views
{
    public class InventoryRequestPresenter : Presenter<IInventoryRequestView>
    {
        private readonly RequestController _controller;
        private readonly SettingController _settingcontroller;
        private InventoryRequest _InventoryRequest;
        public InventoryRequestPresenter([CreateNew] RequestController controller, [CreateNew] SettingController settingcontroller)
        {
            _controller = controller;
            _settingcontroller = settingcontroller;
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
                if (_InventoryRequest == null)
                {
                    int id = View.InventoryRequestId;
                    if (id > 0)
                        _InventoryRequest = _controller.GetInventoryRequest(id);
                    else
                        _InventoryRequest = new InventoryRequest();
                }
                return _InventoryRequest;
            }
            set { _InventoryRequest = value; }
        }
        public override void OnViewInitialized()
        {
            if (_InventoryRequest == null)
            {
                int id = View.InventoryRequestId;
                if (id > 0)
                    _controller.CurrentObject = _controller.GetInventoryRequest(id);
                else
                    _controller.CurrentObject = new InventoryRequest();
            }
        }
        public IList<InventoryRequest> GetInventoryRequests()
        {
            return _controller.GetInventoryRequests();
        }
        public AppUser Approver(int Position)
        {
            return _controller.Approver(Position);
        }
        public AppUser GetUser(int UserId)
        {
            return _controller.GetSuperviser(UserId);
        }
        public AppUser GetSuperviser(int superviser)
        {
            return _controller.GetSuperviser(superviser);
        }
        public void SaveOrUpdateInventoryRequest(InventoryRequest InventoryRequest)
        {
            _controller.SaveOrUpdateEntity(InventoryRequest);
        }
        public int GetLastInventoryRequestId()
        {
            return _controller.GetLastInventoryRequestId();
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
        public ApprovalSetting GetApprovalSetting(string RequestType, decimal value)
        {
            return _settingcontroller.GetApprovalSettingforProcess(RequestType, value);
        }
        public AssignJob GetAssignedJobbycurrentuser()
        {
            return _controller.GetAssignedJobbycurrentuser();
        }
        public AssignJob GetAssignedJobbycurrentuser(int UserId)
        {
            return _controller.GetAssignedJobbycurrentuser(UserId);
        }
        public IList<InventoryRequest> ListInventoryRequests(string requestNo, string RequestDate)
        {
            return _controller.ListInventoryRequests(requestNo, RequestDate);
        }
        public IList<Inventory> GetInventories()
        {
            return _settingcontroller.GetInventories();
        }
        public Inventory GetInventory(int Id)
        {
            return _settingcontroller.GetInventory(Id);
        }
        public IList<Project> GetProjects()
        {
            return _settingcontroller.GetProjects();
        }
        public InventoryRequestDetail GetInventoryRequestDetail(int Id)
        {
            return _controller.GetInventoryRequestDetail(Id);
        }
        public AppUser CurrentUser()
        {
            return _controller.GetCurrentUser();
        }
        public void DeleteInventoryRequestDetail(InventoryRequestDetail InventoryRequestDetail)
        {
            _controller.DeleteEntity(InventoryRequestDetail);
        }
        public void Commit()
        {
            _controller.Commit();
        }
        public IList<Grant> GetGrantbyprojectId(int projectId)
        {
            return _settingcontroller.GetProjectGrantsByprojectId(projectId);
        }
    }
}




