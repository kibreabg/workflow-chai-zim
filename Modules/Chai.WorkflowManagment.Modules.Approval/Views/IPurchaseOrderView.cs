using Chai.WorkflowManagment.CoreDomain.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chai.WorkflowManagment.Modules.Approval.Views
{
    public interface IPurchaseOrderView
    {
        PurchaseRequest PurchaseRequest { get; set; }
        string RequestNo { get; }
        string RequestDate { get; }
        int PurchaseRequestId { get; }
    }
}




