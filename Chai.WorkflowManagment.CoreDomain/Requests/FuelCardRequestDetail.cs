using Chai.WorkflowManagment.CoreDomain.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Chai.WorkflowManagment.CoreDomain.Requests
{
  // [Table("FuelCardRequestDetails")]
    public partial class FuelCardRequestDetail : IEntity
    {
        
        public int Id { get; set; }
     
        public string CustomerNumber { get; set; }
        public string Customer { get; set; }
        public string Date { get; set; }
        public string Hour { get; set; }
        public string DriverCode { get; set; }
        public string RegistrationNo { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public string ReceiptNumber { get; set; }
        public string PastMileage { get; set; }
        public string CurrentMileage { get; set; }
        public string UnitPrice { get; set; }
        public string Quantity { get; set; }
        public string Amount { get; set; }
        public int CurencyNo { get; set; }
        public string Currency { get; set; }
        public string Balance { get; set; }
        public int StationNo { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string Location { get; set; }
        public string InvoiceNo { get; set; }

        public string BusinessPurposeofTrip { get; set; }
        public virtual Project Project { get; set; }
        public virtual Grant Grant { get; set; }     
        public virtual FuelCardRequest FuelCardRequest { get; set; }

        	

    }
}
