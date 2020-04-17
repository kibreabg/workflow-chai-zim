using System;
using System.ComponentModel.DataAnnotations;

namespace Chai.WorkflowManagment.CoreDomain.Setting
{
    [Table("VendorRequestStatuses")]
    public partial class VendorRequestStatus : IEntity
    {
        public int Id { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public int Approver { get; set; }
        public string ApprovalStatus { get; set; }
        public int WorkflowLevel { get; set; }
        public string RejectedReason { get; set; }
        public string AssignedBy { get; set; }
        public int ApproverPosition { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
