﻿using Chai.WorkflowManagment.CoreDomain.Setting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chai.WorkflowManagment.Modules.Setting.Views
{
    public interface ISupplierTypeView
    {
        IList<SupplierType> SupplierType { get; set; }
        string SupplierTypeEmail { get; }
      
     
    }
}




