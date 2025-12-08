using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chai.WorkflowManagment.CoreDomain.Report
{
    [ComplexType]
    public class TravelAdvanceReport
    {
        public DateTime Date { get; set; }
        public string Travel_ID { get; set; }
        public string Description { get; set; }
        public string AccountCode { get; set; }
        public string Project_ID { get; set; }
        public string Grant_ID { get; set; }
        
        
       
    }
}
