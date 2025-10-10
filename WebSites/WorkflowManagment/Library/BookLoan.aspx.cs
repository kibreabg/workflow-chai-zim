using Chai.WorkflowManagment.CoreDomain.Library;
using Microsoft.Practices.ObjectBuilder;
using System;

namespace Chai.WorkflowManagment.Modules.Library.Views
{
    public partial class BookLoan : POCBasePage, IBookLoanView
    {
        private BookLoanPresenter _presenter;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this._presenter.OnViewInitialized();
                int bookId;
                if (int.TryParse(Request.QueryString["BookId"], out bookId))
                {
                    _presenter.LoadBook(bookId);
                }
                else
                {
                    LblMessage.Text = "Invalid Book ID.";
                    BtnSubmitLoan.Enabled = false;
                }
                TxtLoanDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
            this._presenter.OnViewLoaded();
        }

        [CreateNew]
        public BookLoanPresenter Presenter
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
                return "{b7fe32e4-0076-4a22-8e97-fa5bccd0030d}";
            }
        }

        protected void BtnSubmitLoan_Click(object sender, EventArgs e)
        {
            int bookId;
            if (!int.TryParse(Request.QueryString["BookId"], out bookId))
            {
                LblMessage.Text = "Invalid Book ID.";
                return;
            }
            DateTime loanDate, dueDate;
            if (!DateTime.TryParse(TxtLoanDate.Text, out loanDate) || !DateTime.TryParse(TxtDueDate.Text, out dueDate))
            {
                LblMessage.Text = "Please enter valid dates.";
                return;
            }
            if (dueDate <= loanDate)
            {
                LblMessage.Text = "Due date must be after loan date.";
                return;
            }
            _presenter.SubmitLoan(bookId, loanDate, dueDate, BMaster.CurrentUser);
        }

        public void ShowBookDetails(Book book)
        {
            LblBookTitle.Text = book.Title;
            LblBookAuthor.Text = book.Author != null ? book.Author.Name : string.Empty;
            LblBookISBN.Text = book.ISBN;
            LblBookCopies.Text = book.CopiesAvailable.ToString();
        }

        public void ShowMessage(string message, bool isSuccess = false)
        {
            LblMessage.CssClass = isSuccess ? "text-success" : "text-danger";
            LblMessage.Text = message;
            BtnSubmitLoan.Enabled = !isSuccess;
        }
    }
}