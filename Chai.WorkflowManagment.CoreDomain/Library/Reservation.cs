using Chai.WorkflowManagment.CoreDomain.Users;
using System;

namespace Chai.WorkflowManagment.CoreDomain.Library
{
    public partial class Reservation : IEntity
    {
        public Reservation()
        {

        }
        public int Id { get; set; }
        public DateTime ReservationDate { get; set; }
        public string Status { get; set; }
        public virtual Book Book { get; set; }
        public virtual AppUser AppUser { get; set; }


    }
}
