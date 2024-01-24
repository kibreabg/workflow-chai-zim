using Chai.WorkflowManagment.CoreDomain.Setting;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.Modules.Setting.Views
{
    public interface IInventoryView
    {
        IList<Inventory> Inventories { get; set; }
        string ItemName { get; }

    }
}




