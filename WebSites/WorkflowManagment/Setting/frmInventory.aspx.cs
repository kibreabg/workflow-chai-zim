using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Chai.WorkflowManagment.Modules.Setting.Views
{
    public partial class frmInventory : POCBasePage, IInventoryView
    {
        private InventoryPresenter _presenter;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                BindInventories();
            }

            this._presenter.OnViewLoaded();
        }

        [CreateNew]
        public InventoryPresenter Presenter
        {
            get
            {
                return this._presenter;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this._presenter = value;
                this._presenter.View = this;
            }
        }
        public override string PageID
        {
            get
            {
                return "{2eaf0197-41c4-4627-8fc5-8c5fa1cc0602}";
            }
        }

        void BindInventories()
        {
            dgInventory.DataSource = _presenter.ListInventories(txtItemName.Text);
            dgInventory.DataBind();
        }
        #region Interface
        public IList<Inventory> Inventories { get; set; }
        public string ItemName
        {
            get { return txtItemName.Text; }
        }
        #endregion
        protected void btnFind_Click(object sender, EventArgs e)
        {
            _presenter.ListInventories(ItemName);
            BindInventories();
        }
        protected void dgInventory_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgInventory.EditItemIndex = -1;
        }
        protected void dgInventory_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            int id = (int)dgInventory.DataKeys[e.Item.ItemIndex];
            Inventory inventory = _presenter.GetInventoryById(id);
            try
            {
                inventory.Status = "InActive";
                _presenter.SaveOrUpdateInventory(inventory);
                BindInventories();

                Master.ShowMessage(new AppMessage("Inventory was deactivated successfully!", RMessageType.Info));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to delete Inventory. " + ex.Message, RMessageType.Error));
            }
        }
        protected void dgInventory_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            Inventory inventory = new Inventory();
            if (e.CommandName == "AddNew")
            {
                try
                {
                    TextBox txtFItemName = e.Item.FindControl("txtFItemName") as TextBox;
                    inventory.ItemName = txtFItemName.Text;
                    TextBox txtFCategory = e.Item.FindControl("txtFCategory") as TextBox;
                    inventory.Category = txtFCategory.Text;
                    TextBox txtFUnit = e.Item.FindControl("txtFUnit") as TextBox;
                    inventory.Unit = txtFUnit.Text;
                    inventory.Status = "Active";

                    SaveInventory(inventory);
                    dgInventory.EditItemIndex = -1;
                    BindInventories();
                }
                catch (Exception ex)
                {
                    Master.ShowMessage(new AppMessage("Error: Unable to add Inventory " + ex.Message, RMessageType.Error));
                }
            }
        }

        private void SaveInventory(Inventory inventory)
        {
            try
            {
                if (inventory.Id <= 0)
                {
                    _presenter.SaveOrUpdateInventory(inventory);
                    Master.ShowMessage(new AppMessage("Inventory saved!", RMessageType.Info));
                }
                else
                {
                    _presenter.SaveOrUpdateInventory(inventory);
                    Master.ShowMessage(new AppMessage("Inventory updated!", RMessageType.Info));
                }
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage(ex.Message, RMessageType.Error));
            }
        }
        protected void dgInventory_EditCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgInventory.EditItemIndex = e.Item.ItemIndex;

            BindInventories();
        }
        protected void dgInventory_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

        }
        protected void dgInventory_UpdateCommand(object source, DataGridCommandEventArgs e)
        {

            int id = (int)dgInventory.DataKeys[e.Item.ItemIndex];
            Inventory inventory = _presenter.GetInventoryById(id);

            try
            {
                TextBox txtItemName = e.Item.FindControl("txtItemName") as TextBox;
                inventory.ItemName = txtItemName.Text;
                TextBox txtCategory = e.Item.FindControl("txtCategory") as TextBox;
                inventory.Category = txtCategory.Text;
                TextBox txtUnit = e.Item.FindControl("txtUnit") as TextBox;
                inventory.Unit = txtUnit.Text;

                SaveInventory(inventory);
                dgInventory.EditItemIndex = -1;
                BindInventories();
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to Update Inventory. " + ex.Message, Chai.WorkflowManagment.Enums.RMessageType.Error));
            }
        }
    }
}