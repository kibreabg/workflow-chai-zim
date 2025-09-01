using Chai.WorkflowManagment.CoreDomain.Users;
using System;

namespace Chai.WorkflowManagment.CoreDomain.Library
{
    public partial class Loan : IEntity
    {
        public Loan()
        {

        }
        public int Id { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public virtual Book Book { get; set; }
        public virtual AppUser AppUser { get; set; }


    }
}
