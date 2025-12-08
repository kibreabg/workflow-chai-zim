using Chai.WorkflowManagment.CoreDomain.Setting;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chai.WorkflowManagment.CoreDomain.Users
{
    [Table("AssignJobs")]
    public class AssignJob : IEntity
    {
        public AssignJob()
        {
           
        }
        public int Id { get; set; }       
        public virtual EmployeePosition EmployeePosition { get; set; }
        public virtual AppUser AppUser { get; set; }
        public int AssignedTo { get; set; }
        public bool Status { get; set; }
        

        

    }

}
