﻿using Chai.WorkflowManagment.CoreDomain.Request;
using Chai.WorkflowManagment.CoreDomain.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chai.WorkflowManagment.Modules.Approval.Views
{
    public interface IFuelCardApprovalView
    {
        FuelCardRequest FuelCardRequest { get; set; }
        string RequestNo { get; }
        string RequestDate { get; }
        int FuelCardRequestId { get; }
    }
}




