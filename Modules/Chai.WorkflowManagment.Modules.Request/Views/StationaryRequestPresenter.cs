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
    public class StationaryRequestPresenter : Presenter<IStationaryRequestView>
    {

        // NOTE: Uncomment the following code if you want ObjectBuilder to inject the module controller
        //       The code will not work in the Shell module, as a module controller is not created by default
        //
        private RequestController _controller;
        private SettingController _settingcontroller;
        private StationaryRequest _stationaryRequest;
        public StationaryRequestPresenter([CreateNew] RequestController controller, [CreateNew] SettingController settingcontroller)
        {
            _controller = controller;
            _settingcontroller = settingcontroller;
        }
        public override void OnViewLoaded()
        {
            if (View.StationaryRequestId > 0)
            {
                _controller.CurrentObject = _controller.GetStationaryRequest(View.StationaryRequestId);
            }
            CurrentStationaryRequest = _controller.CurrentObject as StationaryRequest;
        }
        public StationaryRequest CurrentStationaryRequest
        {
            get
            {
                if (_stationaryRequest == null)
                {
                    int id = View.StationaryRequestId;
                    if (id > 0)
                        _stationaryRequest = _controller.GetStationaryRequest(id);
                    else
                        _stationaryRequest = new StationaryRequest();
                }
                return _stationaryRequest;
            }
            set { _stationaryRequest = value; }
        }
        public override void OnViewInitialized()
        {
            if (_stationaryRequest == null)
            {
                int id = View.StationaryRequestId;
                if (id > 0)
                    _controller.CurrentObject = _controller.GetStationaryRequest(id);
                else
                    _controller.CurrentObject = new StationaryRequest();
            }
        }
        public IList<StationaryRequest> GetStationaryRequests()
        {
            return _controller.GetStationaryRequests();
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
        public void SaveOrUpdateStationaryRequest(StationaryRequest StationaryRequest)
        {
            _controller.SaveOrUpdateEntity(StationaryRequest);
        }
        public int GetLastStationaryRequestId()
        {
            return _controller.GetLastStationaryRequestId();
        }
        public void CancelPage()
        {
            _controller.Navigate(String.Format("~/Setting/Default.aspx?{0}=3", AppConstants.TABID));
        }
        public void DeleteStationaryRequest(StationaryRequest StationaryRequest)
        {
            _controller.DeleteEntity(StationaryRequest);
        }
        public StationaryRequest GetStationaryRequestById(int id)
        {
            return _controller.GetStationaryRequest(id);
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
        public IList<StationaryRequest> ListStationaryRequests(string requestNo, string RequestDate)
        {
            return _controller.ListStationaryRequests(requestNo, RequestDate);
        }
        public IList<ItemAccount> GetItemAccounts()
        {
            return _settingcontroller.GetItemAccounts();
        }
        public ItemAccount GetItemAccount(int Id)
        {
            return _settingcontroller.GetItemAccount(Id);
        }
        public IList<Project> GetProjects()
        {
            return _settingcontroller.GetProjects();
        }
        public StationaryRequestDetail GetStationaryRequestDetail(int Id)
        {
            return _controller.GetStationaryRequestDetail(Id);
        }
        public AppUser CurrentUser()
        {
            return _controller.GetCurrentUser();
        }
        public void DeleteStationaryRequestDetail(StationaryRequestDetail StationaryRequestDetail)
        {
            _controller.DeleteEntity(StationaryRequestDetail);
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




