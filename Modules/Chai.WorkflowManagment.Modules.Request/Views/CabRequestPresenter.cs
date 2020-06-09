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
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared.MailSender;

namespace Chai.WorkflowManagment.Modules.Request.Views
{
    public class CabRequestPresenter : Presenter<ICabRequestView>
    {

        // NOTE: Uncomment the following code if you want ObjectBuilder to inject the module controller
        //       The code will not work in the Shell module, as a module controller is not created by default
        //
        private RequestController _controller;
        private AdminController _adminController;
        private SettingController _settingController;
        private CabRequest _CabRequest;
        public CabRequestPresenter([CreateNew] RequestController controller, AdminController adminController, SettingController settingController)
        {
            _controller = controller;
            _adminController = adminController;
            _settingController = settingController;
        }
        public override void OnViewLoaded()
        {

            View.telephoneextension = _settingController.GetTelephoneExtensions();
        }
        public override void OnViewInitialized()
        {
           
        }
      
        public IList<CabRequest> GetCabRequests()
        {
            return _controller.GetCabRequests();
        }
      
        public void SaveOrUpdateTARequest(CabRequest CabRequest)
        {
            _controller.SaveOrUpdateEntity(CabRequest);
        }
        public void CancelPage()
        {
            _controller.Navigate(String.Format("~/Request/Default.aspx?{0}=3", AppConstants.TABID));
        }
        public void DeleteCabRequest(CabRequest CabRequest)
        {
            _controller.DeleteEntity(CabRequest);
        }
        public CabRequest GetCabRequest(int id)
        {
            return _controller.GetCabRequest(id);
        }
        public CabCost GetCabCost(int id)
        {
            return _controller.GetCabCost(id);
        }
        public IList<TelephoneExtension> ListTelephoneExtensions()
        {
            return _settingController.GetTelephoneExtensions();
        }
        public CabRequestDetail GetCabRequestDetail(int id)
        {
            return _controller.GetCabRequestDetail(id);
        }
        public void DeleteCabRequestDetail(CabRequestDetail CabRequestDetail)
        {
            _controller.DeleteEntity(CabRequestDetail);
        }
        public void DeleteCabCost(CabCost CabCost)
        {
            _controller.DeleteEntity(CabCost);
        }
        public AppUser Approver(int Position)
        {
            return _controller.Approver(Position);
        }
        public IList<AppUser> GetDrivers()
        {
            return _adminController.GetDrivers();
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
        public Project GetProject(int Id)
        {
            return _settingController.GetProject(Id);
        }
        public IList<Project> GetProjects()
        {
            return _settingController.GetProjects();
        }
        public IList<Grant> GetGrants()
        {
            return _settingController.GetGrants();
        }
        public IList<Grant> GetGrantbyprojectId(int projectId)
        {
            return _settingController.GetProjectGrantsByprojectId(projectId);
        }
        public ItemAccount GetItemAccount(int itemAccountId)
        {
            return _settingController.GetItemAccount(itemAccountId);
        }
        public ItemAccount GetDefaultItemAccount()
        {
            return _settingController.GetDefaultItemAccount();
        }
        public IList<ItemAccount> GetItemAccounts()
        {
            return _settingController.GetItemAccounts();
        }
        public ExpenseType GetExpenseType(int Id)
        {
            return _settingController.GetExpenseType(Id);
        }
        public IList<ExpenseType> GetExpenseTypes()
        {
            return _settingController.GetExpenseTypes();
        }
        public ApprovalSetting GetApprovalSetting(string RequestType, decimal value)
        {
            return _settingController.GetApprovalSettingforProcess(RequestType, value);
        }
        public int GetLastCabRequestId()
        {
            return _controller.GetLastCabRequestId();
        }
    
        public void Commit()
        {
            _controller.Commit();
        }
    }
}




