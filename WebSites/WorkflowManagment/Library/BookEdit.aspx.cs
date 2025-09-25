using Chai.WorkflowManagment.CoreDomain.Library;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared;
using log4net.Config;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Web.UI.WebControls;

namespace Chai.WorkflowManagment.Modules.Library.Views
{
    public partial class BookEdit : POCBasePage, IBookEditView
    {
        private BookEditPresenter _presenter;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                XmlConfigurator.Configure();
                BindAuthors();
                BindGenres();
                if (_presenter.CurrentBook.Id <= 0)
                {
                    BtnDelete.Visible = false;
                }
                else { BindBookFields(); }
            }
            this._presenter.OnViewLoaded();

        }
        [CreateNew]
        public BookEditPresenter Presenter
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
                return "{16f4d6fb-bcc0-4efa-bb41-97aa213b35d8}";
            }
        }

        #region Field Getters
        public int GetBookId
        {
            get
            {
                if (Request.QueryString[AppConstants.BOOKID] != null)
                {
                    return Convert.ToInt32(Request.QueryString[AppConstants.BOOKID]);
                }
                else
                {
                    return 0;
                }
            }
        }
        public string GetTitle
        {
            get { return TxtTitle.Text; }
        }
        public string GetISBN
        {
            get { return TxtIsbn.Text; }
        }
        public int GetPublishedYear
        {
            get { return Convert.ToInt32(TxtPublishedYear.Text); }
        }
        public int GetCopiesAvailable
        {
            get { return Convert.ToInt32(TxtCopiesAvailable.Text); }
        }
        public Author Author
        {
            get { return _presenter.GetAuthor(int.Parse(DdlAuthor.SelectedValue)); }
        }
        public Genre Genre
        {
            get { return _presenter.GetGenre(int.Parse(DdlGenre.SelectedValue)); }
        }
        #endregion

        public void BindAuthors()
        {
            DdlAuthor.DataSource = BookEditPresenter.GetAuthors();
            DdlAuthor.DataBind();
            DdlAuthor.Items.Insert(0, new ListItem("---Select Author---", "0"));
            DdlAuthor.SelectedIndex = 0;
        }
        public void BindGenres()
        {
            DdlGenre.DataSource = BookEditPresenter.GetGenres();
            DdlGenre.DataBind();
            DdlGenre.Items.Insert(0, new ListItem("---Select Genre---", "0"));
            DdlGenre.SelectedIndex = 0;
        }
        private void ClearFormFields()
        {
            TxtTitle.Text = String.Empty;
            TxtIsbn.Text = String.Empty;
            TxtPublishedYear.Text = String.Empty;
            TxtCopiesAvailable.Text = String.Empty;
            DdlAuthor.SelectedIndex = 0;
            DdlGenre.SelectedIndex = 0;
        }
        private void BindBookFields()
        {
            _presenter.OnViewLoaded();
            if (_presenter.CurrentBook != null)
            {
                TxtTitle.Text = _presenter.CurrentBook.Title;
                TxtIsbn.Text = _presenter.CurrentBook.ISBN;
                TxtPublishedYear.Text = _presenter.CurrentBook.PublishedYear.ToString();
                TxtCopiesAvailable.Text = _presenter.CurrentBook.CopiesAvailable.ToString();
                DdlAuthor.SelectedValue = _presenter.CurrentBook.Author.Id.ToString();
                DdlGenre.SelectedValue = _presenter.CurrentBook.Genre.Id.ToString();
            }
        }
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_presenter.CurrentBook.Id == 0)
                {
                    _presenter.SaveOrUpdateBook();
                    Master.TransferMessage(new AppMessage("Book created successfully", RMessageType.Info));
                    _presenter.RedirectToBooks();
                }
                else
                {
                    _presenter.SaveOrUpdateBook();
                    Master.ShowMessage(new AppMessage("Book updated successfully", RMessageType.Info));
                }
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: " + ex.Message, RMessageType.Error));
                ExceptionUtility.LogException(ex, ex.Source);
                ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            }
        }
        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                _presenter.DeleteBook();
                Master.TransferMessage(new AppMessage("Book deleted successfully", RMessageType.Info));
                _presenter.RedirectToBooks();
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: " + ex.Message, RMessageType.Error));
            }
        }
        protected void BtnNew_Click(object sender, EventArgs e)
        {
            ClearFormFields();
            BtnDelete.Visible = false;
        }
        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            _presenter.RedirectToBooks();
        }
    }
}