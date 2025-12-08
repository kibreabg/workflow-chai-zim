using Chai.WorkflowManagment.CoreDomain.Setting;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chai.WorkflowManagment.CoreDomain.Requests
{
    [Table("PurchaseRequestDetails")]
    public partial class PurchaseRequestDetail : IEntity
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public int Qty { get; set; }
        public decimal Priceperunit { get; set; }
        public decimal EstimatedCost { get; set; }
        public virtual ItemAccount ItemAccount { get; set; }
        public string AccountCode { get; set; }
        public virtual Project Project { get; set; }
        public virtual Grant Grant { get; set; }
        public virtual PurchaseRequest PurchaseRequest { get; set; }

    }
}
