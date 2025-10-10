using Chai.WorkflowManagment.CoreDomain.Library;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Chai.WorkflowManagment.Modules.Library.Views
{
    public partial class Genres : POCBasePage, IGenresView
    {
        private GenresPresenter _presenter;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                BindGenres();
            }

            this._presenter.OnViewLoaded();
        }

        [CreateNew]
        public GenresPresenter Presenter
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
                return "{ce3c5d5d-4b45-4b65-a8a3-6e5874629758}";
            }
        }

        #region Field Getters
        public string GetName
        {
            get { return TxtSrchName.Text; }
        }
        public IList<Genre> GetGenres { get; set; }
        #endregion
        void BindGenres()
        {
            DgGenre.DataSource = _presenter.ListGenres(GetName);
            DgGenre.DataBind();
        }
        protected void BtnFind_Click(object sender, EventArgs e)
        {
            BindGenres();
        }
        protected void DgGenre_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            this.DgGenre.EditItemIndex = -1;
        }
        protected void DgGenre_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            int id = (int)DgGenre.DataKeys[e.Item.ItemIndex];
            Genre Genre = _presenter.GetGenre(id);
            try
            {
                Genre.Status = "InActive";
                _presenter.SaveOrUpdateGenre(Genre);
                BindGenres();

                Master.ShowMessage(new AppMessage("Genre was deleted successfully", RMessageType.Info));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to delete Genre. " + ex.Message, RMessageType.Error));
            }
        }
        protected void DgGenre_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            Genre Genre = new Genre();
            if (e.CommandName == "AddNew")
            {
                try
                {
                    TextBox txtName = e.Item.FindControl("TxtName") as TextBox;
                    Genre.Name = txtName.Text;
                    Genre.Status = "Active";
                    SaveGenre(Genre);
                    DgGenre.EditItemIndex = -1;
                    BindGenres();
                }
                catch (Exception ex)
                {
                    Master.ShowMessage(new AppMessage("Error: Unable to add Genre " + ex.Message, RMessageType.Error));
                }
            }
        }
        private void SaveGenre(Genre Genre)
        {
            try
            {
                if (Genre.Id <= 0)
                {
                    _presenter.SaveOrUpdateGenre(Genre);
                    Master.ShowMessage(new AppMessage("Genre Saved", RMessageType.Info));
                }
                else
                {
                    _presenter.SaveOrUpdateGenre(Genre);
                    Master.ShowMessage(new AppMessage("Genre Updated", RMessageType.Info));
                }
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage(ex.Message, RMessageType.Error));
            }
        }
        protected void DgGenre_EditCommand(object source, DataGridCommandEventArgs e)
        {
            this.DgGenre.EditItemIndex = e.Item.ItemIndex;
            BindGenres();
        }
        protected void DgGenre_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

        }
        protected void DgGenre_UpdateCommand(object source, DataGridCommandEventArgs e)
        {

            int id = (int)DgGenre.DataKeys[e.Item.ItemIndex];
            Genre Genre = _presenter.GetGenre(id);

            try
            {
                TextBox txtName = e.Item.FindControl("TxtEdtName") as TextBox;
                Genre.Name = txtName.Text;
                SaveGenre(Genre);
                DgGenre.EditItemIndex = -1;
                BindGenres();
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to update Genre. " + ex.Message, RMessageType.Error));
            }
        }
    }
}