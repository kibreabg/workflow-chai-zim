using Chai.WorkflowManagment.CoreDomain.Library;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Chai.WorkflowManagment.Modules.Library.Views
{
    public partial class Authors : POCBasePage, IAuthorsView
    {
        private AuthorsPresenter _presenter;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                BindAuthors();
            }

            this._presenter.OnViewLoaded();
        }

        [CreateNew]
        public AuthorsPresenter Presenter
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
                return "{0669986b-0ac9-4d32-83c4-984c351c0261}";
            }
        }

        #region Field Getters
        public string GetName
        {
            get { return TxtSrchName.Text; }
        }
        public IList<Author> GetAuthors { get; set; }
        #endregion
        void BindAuthors()
        {
            DgAuthor.DataSource = _presenter.ListAuthors(GetName);
            DgAuthor.DataBind();
        }
        protected void BtnFind_Click(object sender, EventArgs e)
        {
            BindAuthors();
        }
        protected void DgAuthor_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            this.DgAuthor.EditItemIndex = -1;
        }
        protected void DgAuthor_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            int id = (int)DgAuthor.DataKeys[e.Item.ItemIndex];
            Author author = _presenter.GetAuthor(id);
            try
            {
                author.Status = "InActive";
                _presenter.SaveOrUpdateAuthor(author);
                BindAuthors();

                Master.ShowMessage(new AppMessage("Author was deleted successfully", RMessageType.Info));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to delete Author. " + ex.Message, RMessageType.Error));
            }
        }
        protected void DgAuthor_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            Author author = new Author();
            if (e.CommandName == "AddNew")
            {
                try
                {
                    TextBox txtName = e.Item.FindControl("TxtName") as TextBox;
                    author.Name = txtName.Text;
                    TextBox txtBio = e.Item.FindControl("TxtBio") as TextBox;
                    author.Bio = txtBio.Text;
                    author.Status = "Active";
                    SaveAuthor(author);
                    DgAuthor.EditItemIndex = -1;
                    BindAuthors();
                }
                catch (Exception ex)
                {
                    Master.ShowMessage(new AppMessage("Error: Unable to add Author " + ex.Message, RMessageType.Error));
                }
            }
        }
        private void SaveAuthor(Author author)
        {
            try
            {
                if (author.Id <= 0)
                {
                    _presenter.SaveOrUpdateAuthor(author);
                    Master.ShowMessage(new AppMessage("Author Saved", RMessageType.Info));
                }
                else
                {
                    _presenter.SaveOrUpdateAuthor(author);
                    Master.ShowMessage(new AppMessage("Author Updated", RMessageType.Info));
                }
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage(ex.Message, RMessageType.Error));
            }
        }
        protected void DgAuthor_EditCommand(object source, DataGridCommandEventArgs e)
        {
            this.DgAuthor.EditItemIndex = e.Item.ItemIndex;
            BindAuthors();
        }
        protected void DgAuthor_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

        }
        protected void DgAuthor_UpdateCommand(object source, DataGridCommandEventArgs e)
        {

            int id = (int)DgAuthor.DataKeys[e.Item.ItemIndex];
            Author author = _presenter.GetAuthor(id);

            try
            {
                TextBox txtName = e.Item.FindControl("TxtEdtName") as TextBox;
                author.Name = txtName.Text;
                TextBox txtBio = e.Item.FindControl("TxtEdtBio") as TextBox;
                author.Bio = txtBio.Text;
                SaveAuthor(author);
                DgAuthor.EditItemIndex = -1;
                BindAuthors();
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to update Author. " + ex.Message, RMessageType.Error));
            }
        }
    }
}