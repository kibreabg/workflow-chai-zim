using Chai.WorkflowManagment.CoreDomain.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chai.WorkflowManagment.Modules.Request.Views
{
    public interface IFuelCardRequestView
    {
        FuelCardRequest FuelCardRequest { get; set; }
        string RequestNo { get; }
        string RequestDate { get; }

        int Month { get; }
        int Year { get; }
        int FuelCarddRequestId { get; }
      
    }
}




