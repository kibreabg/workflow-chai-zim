using Chai.WorkflowManagment.CoreDomain.Users;
using System;

namespace Chai.WorkflowManagment.CoreDomain.Library
{
    public partial class Review : IEntity
    {
        public Review()
        {

        }
        public int Id { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Comment { get; set; }
        public virtual Book Book { get; set; }
        public virtual AppUser AppUser { get; set; }


    }
}
