using Chai.WorkflowManagment.CoreDomain.Setting;
using System.ComponentModel.DataAnnotations;

namespace Chai.WorkflowManagment.CoreDomain.Requests
{
    [Table("InventoryRequestDetails")]
    public partial class InventoryRequestDetail : IEntity
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public int Qty { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual InventoryRequest InventoryRequest { get; set; }
    }
}