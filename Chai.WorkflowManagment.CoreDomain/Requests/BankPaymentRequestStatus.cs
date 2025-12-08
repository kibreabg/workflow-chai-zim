using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chai.WorkflowManagment.CoreDomain.Requests
{
    [Table("BankPaymentRequestStatuses")]
    public partial class BankPaymentRequestStatus : IEntity
    {
        public int Id { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public int Approver { get; set; }
        public string AssignedBy { get; set; }
        public string ApprovalStatus { get; set; }
        public string RejectedReason { get; set; }
        public int WorkflowLevel { get; set; }
        public virtual BankPaymentRequest BankPaymentRequest { get; set; }
    }
}
