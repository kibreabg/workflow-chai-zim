﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Chai.WorkflowManagment.CoreDomain.Requests
{
    [Table("FuelCardRequestStatuses")]
    public partial class FuelCardRequestStatus : IEntity
    {
        public int Id { get; set; }
        public string ApprovalStatus { get; set; }
        public int Approver { get; set; }
        public string AssignedBy { get; set; }
        public string RejectedReason { get; set; }
        public int WorkflowLevel {get;set;}
        public Nullable<DateTime> ApprovalDate { get; set; }
        public virtual FuelCardRequest FuelCardRequest { get; set; }

    }
}
