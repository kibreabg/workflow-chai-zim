using Chai.WorkflowManagment.CoreDomain.Requests;

namespace Chai.WorkflowManagment.Modules.Request.Views
{
    public interface IInventoryRequestView
    {
        InventoryRequest InventoryRequest { get; set; }
        string RequestNo { get; }
        string RequestDate { get; }
        int InventoryRequestId { get; }
    }
}




