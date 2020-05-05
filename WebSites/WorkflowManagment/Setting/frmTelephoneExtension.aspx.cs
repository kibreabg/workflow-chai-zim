using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared;
using Microsoft.Practices.ObjectBuilder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Chai.WorkflowManagment.Modules.Setting.Views
{
    public partial class frmTelephoneExtension : POCBasePage, ITelephoneExtensionView
    {
        private TelephoneExtensionPresenter _presenter;
        private IList<TelephoneExtension> _telexts;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                BindTelephoneExtension();
            }
            
            this._presenter.OnViewLoaded();
            

        }

        [CreateNew]
        public TelephoneExtensionPresenter Presenter
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
                return "{969246EF-65E7-4E32-87A9-86B481C365B1}";
            }
        }

        void BindTelephoneExtension()
        {
            dgTelExt.DataSource = _presenter.ListTelephoneExtensions(txtName.Text,txtExtension.Text);
            dgTelExt.DataBind();
        }
        #region interface
        

        public IList<CoreDomain.Setting.TelephoneExtension> telextension
        {
            get
            {
                return _telexts;
            }
            set
            {
                _telexts = value;
            }
        }
        #endregion
        protected void btnFind_Click(object sender, EventArgs e)
        {
            //_presenter.ListTelephoneExtensions(Name,Extension);
            BindTelephoneExtension();
        }
        protected void dgTelExt_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgTelExt.EditItemIndex = -1;
        }
        protected void dgTelExt_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            int id = (int)dgTelExt.DataKeys[e.Item.ItemIndex];
            Chai.WorkflowManagment.CoreDomain.Setting.TelephoneExtension telext = _presenter.GetTelephoneExtensionById(id);
            try
            {

                _presenter.SaveOrUpdateTelephoneExtension(telext);
                
                BindTelephoneExtension();

                Master.ShowMessage(new AppMessage("Telephone Extension was Removed Successfully", Chai.WorkflowManagment.Enums.RMessageType.Info));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to delete Telephone Extension. " + ex.Message, Chai.WorkflowManagment.Enums.RMessageType.Error));
            }
        }
        protected void dgTelExt_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            Chai.WorkflowManagment.CoreDomain.Setting.TelephoneExtension telext = new Chai.WorkflowManagment.CoreDomain.Setting.TelephoneExtension();
            if (e.CommandName == "AddNew")
            {
                try
                {

                    TextBox txtFName = e.Item.FindControl("txtFName") as TextBox;
                    telext.Name = txtFName.Text;
                    TextBox txtFExtension = e.Item.FindControl("txtFExtension") as TextBox;
                    telext.Extension = txtFExtension.Text;
                    TextBox txtFCellphone = e.Item.FindControl("txtFCellphone") as TextBox;
                    telext.Cellphone = txtFCellphone.Text;
                   
                    SaveTelephoneExtension(telext);
                    dgTelExt.EditItemIndex = -1;
                    BindTelephoneExtension();
                }
                catch (Exception ex)
                {
                    Master.ShowMessage(new AppMessage("Error: Unable to Add Telephone Extension " + ex.Message, Chai.WorkflowManagment.Enums.RMessageType.Error));
                }
            }
        }

        private void SaveTelephoneExtension(Chai.WorkflowManagment.CoreDomain.Setting.TelephoneExtension textExt)
        {
            try
            {
                if(textExt.Id  <= 0)
                {
                    _presenter.SaveOrUpdateTelephoneExtension(textExt);
                    Master.ShowMessage(new AppMessage("Telephone Extension saved", RMessageType.Info));
                    //_presenter.CancelPage();
                }
                else
                {
                    _presenter.SaveOrUpdateTelephoneExtension(textExt);
                    Master.ShowMessage(new AppMessage("Telephone Extension  Updated", RMessageType.Info));
                   // _presenter.CancelPage();
                }
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage(ex.Message, RMessageType.Error));
            }
        }
        protected void dgTelExt_EditCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgTelExt.EditItemIndex = e.Item.ItemIndex;

            BindTelephoneExtension();
        }
        protected void dgTelExt_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

        }
        protected void dgTelExt_UpdateCommand(object source, DataGridCommandEventArgs e)
        {

            int id = (int)dgTelExt.DataKeys[e.Item.ItemIndex];
            Chai.WorkflowManagment.CoreDomain.Setting.TelephoneExtension telext = _presenter.GetTelephoneExtensionById(id);

            try
            {

                TextBox txtName = e.Item.FindControl("txtName") as TextBox;
                telext.Name = txtName.Text;
                TextBox txtExtension = e.Item.FindControl("txtExtension") as TextBox;
                telext.Extension = txtExtension.Text;
                TextBox txtCellphone = e.Item.FindControl("txtCellphone") as TextBox;
                telext.Cellphone = txtCellphone.Text;

                SaveTelephoneExtension(telext);
                dgTelExt.EditItemIndex = -1;
                BindTelephoneExtension();
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to Update Telephone Extension " + ex.Message, Chai.WorkflowManagment.Enums.RMessageType.Error));
            }
           
        }


        public string Name
        {
            get { return txtName.Text; }
        }

        public string Extension
        {
            get { return txtExtension.Text; }
        }

      

        public IList<TelephoneExtension> telephoneextension
        {
            get
            {
                return _telexts;
            }
            set
            {
                _telexts = value;
            }
        }
    }
}