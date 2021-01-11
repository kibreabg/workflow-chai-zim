using System;
using System.ComponentModel.DataAnnotations;

namespace Chai.WorkflowManagment.CoreDomain.Report
{
    [ComplexType]
    public class FuelCardReport
    {
        public DateTime Date { get; set; }

        public string Card_Holder_Name { get; set; }

        public string Year { get; set; }

        public string Month { get; set; }

        private Decimal Total_Reimbursement { get; set; }

        public string Vehicle_Reg_Number { get; set; }
    }
}