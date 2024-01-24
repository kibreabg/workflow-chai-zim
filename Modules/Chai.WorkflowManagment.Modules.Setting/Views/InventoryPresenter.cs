using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.Shared;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.Modules.Setting.Views
{
    public class InventoryPresenter : Presenter<IInventoryView>
    {
        private readonly SettingController _controller;
        public InventoryPresenter([CreateNew] SettingController controller)
        {
            _controller = controller;
        }

        public override void OnViewLoaded()
        {
            View.Inventories = _controller.ListInventories(View.ItemName);
        }

        public override void OnViewInitialized()
        {

        }
        public IList<Inventory> GetInventories()
        {
            return _controller.GetInventories();
        }
        public void SaveOrUpdateInventory(Inventory inventory)
        {
            _controller.SaveOrUpdateEntity(inventory);
        }
        public void CancelPage()
        {
            _controller.Navigate(String.Format("~/Setting/Default.aspx?{0}=3", AppConstants.TABID));
        }
        public void DeleteInventory(Inventory inventory)
        {
            _controller.DeleteEntity(inventory);
        }
        public Inventory GetInventoryById(int id)
        {
            return _controller.GetInventory(id);
        }
        public IList<Inventory> ListInventories(string itemName)
        {
            return _controller.ListInventories(itemName);
        }
        public void Commit()
        {
            _controller.Commit();
        }
    }
}




