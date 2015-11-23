using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chai.WorkflowManagment.CoreDomain.Approval
{
    public partial class BAAttachment : IEntity
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public virtual BidAnalysis BidAnalisis { get; set; }
    }
}
