using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chai.WorkflowManagment.CoreDomain.Report
{
    [ComplexType]
    public class PurchaseReport
    {
        public DateTime Date { get; set; }
        public string Purchase_Request_Ref { get; set; }
        public string Purchase_Order { get; set; }
        public string AccountCode { get; set; }
        public string Description { get; set; }
        public decimal TotalCost { get; set; }

        public string Project_ID { get; set; }
        public string Grant_ID { get; set; }
       
    }
}
