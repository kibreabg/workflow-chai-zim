using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.Modules.Request.Views
{
	public class FuelCardRequestPresenter : Presenter<IFuelCardRequestView>
	{
		private RequestController _controller;
		private Setting.SettingController _settingcontroller;
		private FuelCardRequest _fuelcardrequest;
		public FuelCardRequestPresenter([CreateNew] RequestController controller, [CreateNew] Setting.SettingController settingcontroller)
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
			_controller.SaveOrUpdateEntity(FuelCardRequest);
		}

		public void SaveOrUpdateFuelCardRequest()
		{
			FuelCardRequest FuelCardRequest = CurrentFuelCardRequest;
			FuelCardRequest.Month = View.Month;
			FuelCardRequest.Year = View.Year;
			FuelCardRequest.ProgressStatus = ProgressStatus.InProgress.ToString();

			_controller.SaveOrUpdateEntity(FuelCardRequest);
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
		public IList<PurchaseRequest> ListPurchaseRequests(string requestNo, string RequestDate)
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