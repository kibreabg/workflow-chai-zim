using Chai.WorkflowManagment.Shared;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Web.UI.WebControls;

namespace Chai.WorkflowManagment.Modules.Library.Views
{
    public partial class BookList : POCBasePage, IBooksView
    {
        private BooksPresenter _presenter;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                BindAuthors();
                BindGenres();
                GrvBookList.DataSource = _presenter.ListBooks(ddlSrchAuthors.SelectedValue, ddlSrchGenres.SelectedValue, txtSrchTitle.Text);
                GrvBookList.DataBind();

            }
            this._presenter.OnViewLoaded();

        }
        [CreateNew]
        public BooksPresenter Presenter
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
                return "{30b6d941-bf37-4b4e-bfb7-00e9d105f99c}";
            }
        }
        public void BindAuthors()
        {
            ddlSrchAuthors.DataSource = BooksPresenter.GetAuthors();
            ddlSrchAuthors.DataBind();
        }
        public void BindGenres()
        {
            ddlSrchGenres.DataSource = BooksPresenter.GetGenres();
            ddlSrchGenres.DataBind();
        }
        protected void GrvBookList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        }
        protected void GrvBookList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrvBookList.PageIndex = e.NewPageIndex;
            GrvBookList.DataSource = _presenter.ListBooks(ddlSrchAuthors.SelectedValue, ddlSrchGenres.SelectedValue, txtSrchTitle.Text);
            GrvBookList.DataBind();
        }
        protected void BtnFind_Click(object sender, EventArgs e)
        {
            GrvBookList.DataSource = _presenter.ListBooks(ddlSrchAuthors.SelectedValue, ddlSrchGenres.SelectedValue, txtSrchTitle.Text);
            GrvBookList.DataBind();
        }

        protected void GrvBookList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.RedirectPage(string.Format("BookEdit.aspx?BookId={0}&{1}=0", GrvBookList.SelectedDataKey.Value, AppConstants.TABID));
        }

        protected void GrvBookList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int bookId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Loan")
            {
                _presenter.RedirectPage(string.Format("BookLoan.aspx?BookId={0}&{1}=0", bookId, AppConstants.TABID));
            }
            else if (e.CommandName == "Reserve")
            {
                _presenter.RedirectPage(string.Format("BookReserve.aspx?BookId={0}&{1}=0", bookId, AppConstants.TABID));
            }
            else if (e.CommandName == "Review")
            {
                _presenter.RedirectPage(string.Format("BookReview.aspx?BookId={0}&{1}=0", bookId, AppConstants.TABID));
            }
        }
    }
}

