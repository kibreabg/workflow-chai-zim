using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeWeb;
using Chai.WorkflowManagment.CoreDomain.Request;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Modules.Admin;

namespace Chai.WorkflowManagment.Modules.Request.Views
{
    public class FuelCardRequestPresenter : Presenter<IFuelCardRequestView>
    {

        // NOTE: Uncomment the following code if you want ObjectBuilder to inject the module controller
        //       The code will not work in the Shell module, as a module controller is not created by default
        //
         private Chai.WorkflowManagment.Modules.Request.RequestController _controller;
         private Chai.WorkflowManagment.Modules.Setting.SettingController _settingcontroller;
         private FuelCardRequest _fuelcardrequest;
         public FuelCardRequestPresenter([CreateNew] Chai.WorkflowManagment.Modules.Request.RequestController controller, [CreateNew] Chai.WorkflowManagment.Modules.Setting.SettingController settingcontroller)
         {
         		_controller = controller;
                _settingcontroller = settingcontroller;
         }

         public override void OnViewLoaded()
         {
             if (View.FuelCarddRequestId > 0)
             {
                 _controller.CurrentObject = _controller.GetFuelCardRequest(View.FuelCarddRequestId);
             }
            CurrentFuelCardRequest = _controller.CurrentObject as FuelCardRequest;
         }
         public FuelCardRequest CurrentFuelCardRequest
        {
             get
             {
                 if (_fuelcardrequest == null)
                 {
                     int id = View.FuelCarddRequestId;
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
                 int id = View.FuelCarddRequestId;
                 if (id > 0)
                     _controller.CurrentObject = _controller.GetFuelCardRequest(id);
                 else
                     _controller.CurrentObject = new FuelCardRequest();
             }
         }
         public IList<FuelCardRequest> GetFuelCardRequest()
         {
             return _controller.GetFuelCardRequests();
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
        public void SaveOrUpdateFuelCardRequest(FuelCardRequest FuelCardRequest)
        {
            try
            {
                _controller.SaveOrUpdateEntity(FuelCardRequest);
            }
            
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting  
                        // the current instance as InnerException  
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }

        public void SaveOrUpdateFuelCardRequest()
        {
            try
            { 

            FuelCardRequest FuelCardRequest = CurrentFuelCardRequest;
            FuelCardRequest.Month = View.Month;
            FuelCardRequest.Year = View.Year;
            


            //  BidAnalysisRequest.Supplier.Id=View.GetSupplierId;

            //   BidAnalysisRequest.SelectedBy = View.GetSelectedBy;

            FuelCardRequest.ProgressStatus = ProgressStatus.InProgress.ToString();





            _controller.SaveOrUpdateEntity(FuelCardRequest);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
        // raise a new exception nesting  
        // the current instance as InnerException  
              raise = new InvalidOperationException(message, raise);
             }
          }
                throw raise;
            }
        }
         public int GetLastFuelCardRequestId()
         {
             return _controller.GetLastFuelCardRequestId();
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

         public ApprovalSetting GetApprovalSetting(string RequestType, decimal value)
         {
             return _settingcontroller.GetApprovalSettingforProcess(RequestType, value);
         }
         public ApprovalSetting GetApprovalSettingforFuelCardProcess(string RequestType, decimal value)
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
         public IList<PurchaseRequest> ListPurchaseRequests(string requestNo,string RequestDate)
         {
             return _controller.ListPurchaseRequests(requestNo, RequestDate);

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
         public Project GetProject(int Id)
         {
             return _settingcontroller.GetProject(Id);

         }
         public IList<Grant> GetGrants()
         {
             return _settingcontroller.GetGrants();

         }
       
         public Grant GetGrant(int Id)
         {
             return _settingcontroller.GetGrant(Id);

         }
         public FuelCardRequestDetail GetFuelCardRequestDetail(int Id)
         {
             return _controller.GetFuelCardRequestDetail(Id);

         }
         public AppUser CurrentUser()
         {
             return _controller.GetCurrentUser();
         }
         public void DeleteFuelCardRequestDetail(FuelCardRequestDetail FuelCardRequestDetail)
         {
             _controller.DeleteEntity(FuelCardRequestDetail);
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




