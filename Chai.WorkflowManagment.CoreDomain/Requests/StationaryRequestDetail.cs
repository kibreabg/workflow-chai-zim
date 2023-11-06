using Chai.WorkflowManagment.CoreDomain.Setting;
using System.ComponentModel.DataAnnotations;

namespace Chai.WorkflowManagment.CoreDomain.Requests
{
    [Table("StationaryRequestDetails")]
    public partial class StationaryRequestDetail : IEntity
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public int Qty { get; set; }
        public virtual ItemAccount ItemAccount { get; set; }
        public string AccountCode { get; set; }
        public virtual StationaryRequest StationaryRequest { get; set; }
    }
}