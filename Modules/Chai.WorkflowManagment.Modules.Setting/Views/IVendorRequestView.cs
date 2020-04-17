
using System;

namespace Chai.WorkflowManagment.Modules.Setting.Views
{
    public interface IVendorRequestView
    {
        int GetSupplierId { get; }
        string GetRequestNo { get; }
        DateTime GetRequestDate { get; }
        string GetVendorLegalEntityName { get; }
        string GetTradeName { get; }
        string GetCertificate { get; }
        string GetTaxNo { get; }
        string GetBusinessAddressLine1 { get; }
        string GetBusinessAddressLine2 { get; }
        string GetBusinessCity { get; }
        string GetBusinessPostalCode { get; }
        string GetBusinessStateProvince { get; }
        string GetBusinessCountry { get; }
        string GetPostalAddressLine1 { get; }
        string GetPostalAddressLine2 { get; }
        string GetPostalCity { get; }
        string GetPostalPostalCode { get; }
        string GetPostalStateProvince { get; }
        string GetPostalCountry { get; }
        string GetContactName { get; }
        string GetEmail { get; }
        string GetWebsite { get; }
        string GetPosition { get; }
        string GetPhoneNumber { get; }
        string GetCellNumber { get; }
        string GetFaxNumber { get; }
        string GetTechnicalCapacity { get; }
        string GetTradeRef1 { get; }
        string GetTradeRef2 { get; }
    }
}




