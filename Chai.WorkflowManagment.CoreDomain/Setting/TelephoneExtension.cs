using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chai.WorkflowManagment.CoreDomain.Setting
{
    public partial class TelephoneExtension : IEntity 
    {
        public TelephoneExtension()
        {
           
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Cellphone { get; set; }
        
       
    }
}
