using Chai.WorkflowManagment.CoreDomain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chai.WorkflowManagment.CoreDomain.Setting
{
    public partial class Supplier : IEntity 
    {
        public Supplier()
        {
            this.VendorRequestStatuses = new List<VendorRequestStatus>();
            this.VendorAttachments = new List<VendorAttachment>();
        }
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierContact { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string Status { get; set; }
        public string RequestNo { get; set; }
        public Nullable<DateTime> RequestDate { get; set; }
        public string VendorLegalEntityName { get; set; }
        public string TradeName { get; set; }
        public string Certificate { get; set; }
        public string TaxNo { get; set; }
        public string BusinessAddressLine1 { get; set; }
        public string BusinessAddressLine2 { get; set; }
        public string BusinessCity { get; set; }
        public string BusinessPostalCode { get; set; }
        public string BusinessStateProvince { get; set; }
        public string BusinessCountry { get; set; }
        public string PostalAddressLine1 { get; set; }
        public string PostalAddressLine2 { get; set; }
        public string PostalCity { get; set; }
        public string PostalPostalCode { get; set; }
        public string PostalStateProvince { get; set; }
        public string PostalCountry { get; set; }
        public string ContactName { get; set; }
        public string Website { get; set; }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }
        public string CellNumber { get; set; }
        public string FaxNumber { get; set; }
        public string TechnicalCapacity { get; set; }
        public string TradeRef1 { get; set; }
        public string TradeRef2 { get; set; }
        public int CurrentApprover { get; set; }
        public int CurrentApproverPosition { get; set; }
        public int CurrentLevel { get; set; }
        public string CurrentStatus { get; set; }
        public string ProgressStatus { get; set; }
        public virtual AppUser AppUser { get; set; }
        public virtual SupplierType SupplierType { get; set; }
        public virtual IList<VendorRequestStatus> VendorRequestStatuses { get; set; }
        public virtual IList<VendorAttachment> VendorAttachments { get; set; }

        #region VendorRequestStatus
        public virtual VendorRequestStatus GetVendorRequestStatus(int Id)
        {
            foreach (VendorRequestStatus VRS in VendorRequestStatuses)
            {
                if (VRS.Id == Id)
                    return VRS;
            }
            return null;
        }
        public virtual VendorRequestStatus GetVendorRequestStatusworkflowLevel(int workflowLevel)
        {
            foreach (VendorRequestStatus VRS in VendorRequestStatuses)
            {
                if (VRS.WorkflowLevel == workflowLevel)
                    return VRS;
            }
            return null;
        }
        public virtual IList<VendorRequestStatus> GetVendorRequestStatusBySupplierId(int SupplierId)
        {
            IList<VendorRequestStatus> LRS = new List<VendorRequestStatus>();
            foreach (VendorRequestStatus VRS in VendorRequestStatuses)
            {
                if (VRS.Supplier.Id == SupplierId)
                    LRS.Add(VRS);
            }
            return LRS;
        }
        public virtual void RemoveVendorRequestStatus(int Id)
        {
            foreach (VendorRequestStatus VRS in VendorRequestStatuses)
            {
                if (VRS.Id == Id)
                {
                    VendorRequestStatuses.Remove(VRS);
                    break;
                }
            }
        }
        #endregion
        #region VendorAttachment
        public virtual void RemoveVendorAttachment(string FilePath)
        {
            foreach (VendorAttachment va in VendorAttachments)
            {
                if (va.FilePath == FilePath)
                {
                    VendorAttachments.Remove(va);
                    break;
                }
            }
        }
        #endregion
        
       
    }
}
