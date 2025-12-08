using Chai.WorkflowManagment.CoreDomain.Users;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chai.WorkflowManagment.CoreDomain.Setting
{
    [Table("EmployeeLeaves")]
    public partial class EmployeeLeave : IEntity
    {
        public EmployeeLeave()
        {

        }
       
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual AppUser AppUser { get; set; }
        public decimal LeaveTaken { get; set; }
        public decimal BeginingBalance { get; set; }
        public bool Status { get; set; }
        public decimal Rate { get; set; }
    }

}
