using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chai.WorkflowManagment.CoreDomain.Requests
{
    [Table("PurchaseRequestStatuses")]
    public partial class PurchaseRequestStatus : IEntity
    {
        public int Id { get; set; }
        public string ApprovalStatus { get; set; }
        public int Approver { get; set; }
        public string AssignedBy { get; set; }
        public string RejectedReason { get; set; }
        public int WorkflowLevel { get; set; }
        public Nullable<DateTime> ApprovalDate { get; set; }
        public PurchaseRequest PurchaseRequest { get; set; }

    }
}
