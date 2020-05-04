using Chai.WorkflowManagment.CoreDomain.Setting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chai.WorkflowManagment.Modules.Setting.Views
{
    public interface ITelephoneExtensionView
    {
        IList<TelephoneExtension> telephoneextension { get; set; }
        string Name { get; }
        string Extension { get; }
       
     
    }
}




