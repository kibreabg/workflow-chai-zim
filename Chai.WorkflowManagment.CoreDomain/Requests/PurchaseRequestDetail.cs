using Chai.WorkflowManagment.CoreDomain.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Chai.WorkflowManagment.CoreDomain.Request
{
    [Table("PurchaseRequestDetails")]
    public partial class PurchaseRequestDetail : IEntity
    {
        public int Id { get; set; }
        public int Qty { get; set; }
        public decimal Priceperunit { get; set; }
        public decimal EstimatedCost { get; set; }
        public ItemAccount ItemAccount { get; set; }
        public string AccountCode { get; set; }
        public Project project { get; set; }
        public Grant Grant { get; set; }
     
        public PurchaseRequest PurchaseRequest { get; set; }

    }
}
