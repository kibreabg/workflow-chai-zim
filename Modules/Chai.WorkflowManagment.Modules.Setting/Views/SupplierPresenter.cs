using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeWeb;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Modules.Admin;
using Chai.WorkflowManagment.Shared.MailSender;

namespace Chai.WorkflowManagment.Modules.Setting.Views
{
    public class SupplierPresenter : Presenter<IVendorRequestView>
    {

        // NOTE: Uncomment the following code if you want ObjectBuilder to inject the module controller
        //       The code will not work in the Shell module, as a module controller is not created by default
        //
        private Supplier _supplier;
        private SettingController _controller;
        private AdminController _adminController;
        public SupplierPresenter([CreateNew] SettingController controller, AdminController adminController)
        {
            _controller = controller;
            _adminController = adminController;
        }
        public override void OnViewLoaded()
        {
            if (View.GetSupplierId > 0)
            {
                _controller.CurrentObject = _controller.GetSupplier(View.GetSupplierId);
            }
            CurrentSupplier = _controller.CurrentObject as Supplier;
        }
        public override void OnViewInitialized()
        {
            if (_supplier == null)
            {
                int id = View.GetSupplierId;
                if (id > 0)
                    _controller.CurrentObject = _controller.GetSupplier(id);
                else
                    _controller.CurrentObject = new Supplier();
            }
        }
        public Supplier CurrentSupplier
        {
            get
            {
                if (_supplier == null)
                {
                    int id = View.GetSupplierId;
                    if (id > 0)
                        _supplier = _controller.GetSupplier(id);
                    else
                        _supplier = new Supplier();
                }
                return _supplier;
            }
            set { _supplier = value; }


        }
        public AppUser CurrentUser()
        {
            return _controller.GetCurrentUser();
        }
        public AppUser Approver(int Position)
        {
            return _controller.Approver(Position);
        }
        private void SendEmail(VendorRequestStatus VRS)
        {
            if (GetSuperviser(VRS.Approver).IsAssignedJob != true)
            {
                EmailSender.Send(GetSuperviser(VRS.Approver).Email, "Vendor Request", (CurrentSupplier.AppUser.FullName).ToUpper() + "Requests for Vendor Registration with Request No. - '" + (CurrentSupplier.RequestNo).ToUpper() + "'");
            }
            else
            {
                EmailSender.Send(GetSuperviser(_controller.GetAssignedJobbycurrentuser(VRS.Approver).AssignedTo).Email, "Vendor Request", (CurrentSupplier.AppUser.FullName).ToUpper() + " Requests for Vendor Registration with Request No. - '" + (CurrentSupplier.RequestNo).ToUpper() + "'");
            }
        }
        public AppUser GetSuperviser(int superviser)
        {
            return _controller.GetSuperviser(superviser);
        }
        public Supplier GetSupplierById(int id)
        {
            return _controller.GetSupplier(id);
        }
        public IList<Supplier> ListSuppliers(string SupplierName)
        {
            return _controller.ListSuppliers(SupplierName);
        }
        public IList<SupplierType> GetSupplierTypes()
        {
            return _controller.GetSupplierTypes();
        }
        public SupplierType GetSupplierTypeById(int id)
        {
            return _controller.GetSupplierType(id);
        }
        public IList<Supplier> GetSuppliers()
        {
            return _controller.GetSuppliers();
        }
        public int GetLastSupplierId()
        {
            return _controller.GetLastSupplierId();
        }
        private void GetCurrentApprover()
        {
            if (CurrentSupplier.VendorRequestStatuses != null)
            {
                foreach (VendorRequestStatus VRS in CurrentSupplier.VendorRequestStatuses)
                {
                    if (VRS.ApprovalStatus == null)
                    {
                        SendEmail(VRS);
                        CurrentSupplier.CurrentApprover = VRS.Approver;
                        CurrentSupplier.CurrentLevel = VRS.WorkflowLevel;
                        CurrentSupplier.CurrentStatus = VRS.ApprovalStatus;
                        break;
                    }
                }
            }
        }
        public ApprovalSetting GetApprovalSetting(string RequestType, decimal value)
        {
            return _controller.GetApprovalSettingforProcess(RequestType, value);
        }
        private void SaveVendorRequestStatus()
        {
            if (GetApprovalSetting(RequestType.Vendor_Request.ToString().Replace('_', ' '), 0) != null)
            {
                int i = 1;
                foreach (ApprovalLevel AL in GetApprovalSetting(RequestType.Vendor_Request.ToString().Replace('_', ' '), 0).ApprovalLevels)
                {
                    VendorRequestStatus VRS = new VendorRequestStatus();
                    VRS.Supplier = CurrentSupplier;
                    //All Approver positions must be entered into the database before the approval workflow could run effectively!

                    if (Approver(AL.EmployeePosition.Id) != null)
                    {
                        if (AL.EmployeePosition.PositionName == "Finance Officer")
                        {
                            VRS.ApproverPosition = AL.EmployeePosition.Id; //So that we can entertain more than one finance manager to handle the request
                        }
                        else
                        {
                            VRS.Approver = Approver(AL.EmployeePosition.Id).Id;
                        }
                    }
                    else
                        VRS.Approver = 0;
                    VRS.WorkflowLevel = i;
                    i++;
                    CurrentSupplier.VendorRequestStatuses.Add(VRS);
                }
            }
        }
        public void SaveOrUpdateSupplier(Supplier Supplier)
        {
            _controller.SaveOrUpdateEntity(Supplier);
        }
        public void SaveOrUpdateVendorRequest()
        {
            Supplier supplier = CurrentSupplier;
            if (supplier.Id <= 0)
            {
                supplier.RequestNo = View.GetRequestNo;
            }
            supplier.RequestDate = Convert.ToDateTime(DateTime.Today.ToShortDateString());
            supplier.Email = View.GetEmail;
            supplier.VendorLegalEntityName = View.GetVendorLegalEntityName;
            supplier.TradeName = View.GetTradeName;
            supplier.Certificate = View.GetCertificate;
            supplier.TaxNo = View.GetTaxNo;
            supplier.BusinessAddressLine1 = View.GetBusinessAddressLine1;
            supplier.BusinessAddressLine2 = View.GetBusinessAddressLine2;
            supplier.BusinessCity = View.GetBusinessCity;
            supplier.BusinessPostalCode = View.GetBusinessPostalCode;
            supplier.BusinessStateProvince = View.GetBusinessStateProvince;
            supplier.BusinessCountry = View.GetBusinessCountry;
            supplier.PostalAddressLine1 = View.GetPostalAddressLine1;
            supplier.PostalAddressLine2 = View.GetPostalAddressLine2;
            supplier.PostalCity = View.GetPostalCity;
            supplier.PostalPostalCode = View.GetPostalPostalCode;
            supplier.PostalStateProvince = View.GetPostalStateProvince;
            supplier.PostalCountry = View.GetPostalCountry;
            supplier.ContactName = View.GetContactName;
            supplier.Website = View.GetWebsite;
            supplier.Position = View.GetPosition;
            supplier.PhoneNumber = View.GetPhoneNumber;
            supplier.CellNumber = View.GetCellNumber;
            supplier.FaxNumber = View.GetFaxNumber;
            supplier.TechnicalCapacity = View.GetTechnicalCapacity;
            supplier.TradeRef1 = View.GetTradeRef1;
            supplier.TradeRef2 = View.GetTradeRef2;
            supplier.ProgressStatus = ProgressStatus.InProgress.ToString();
            supplier.AppUser = _adminController.GetUser(CurrentUser().Id);
            if (CurrentSupplier.VendorRequestStatuses.Count == 0)
                SaveVendorRequestStatus();

            GetCurrentApprover();

            _controller.SaveOrUpdateEntity(supplier);
        }
        public void CancelPage()
        {
            _controller.Navigate(String.Format("~/Setting/Default.aspx?{0}=3", AppConstants.TABID));
        }
        public void DeleteSupplier(Supplier Supplier)
        {
            _controller.DeleteEntity(Supplier);
        }
        public void Commit()
        {
            _controller.Commit();
        }
    }
}




