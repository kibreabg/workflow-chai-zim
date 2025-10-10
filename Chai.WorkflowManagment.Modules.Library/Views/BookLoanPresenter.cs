using Chai.WorkflowManagment.CoreDomain.Library;
using Chai.WorkflowManagment.CoreDomain.Users;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.ObjectBuilder;
using System;

namespace Chai.WorkflowManagment.Modules.Library.Views
{
    public class BookLoanPresenter : Presenter<IBookLoanView>
    {
        private readonly LibraryController _libraryController;

        public BookLoanPresenter([CreateNew] LibraryController libraryController)
        {
            _libraryController = libraryController;
        }

        public void LoadBook(int bookId)
        {
            var book = _libraryController.GetBook(bookId);
            if (book != null)
            {
                View.ShowBookDetails(book);
            }
            else
            {
                View.ShowMessage("Book not found.");
            }
        }

        public void SubmitLoan(int bookId, DateTime loanDate, DateTime dueDate, AppUser user)
        {
            var book = _libraryController.GetBook(bookId);
            if (book == null)
            {
                View.ShowMessage("Book not found.");
                return;
            }
            if (book.CopiesAvailable <= 0)
            {
                View.ShowMessage("No copies available for loan.");
                return;
            }

            // Create and save the loan
            var loan = new Loan
            {
                Book = book,
                AppUser = user,
                LoanDate = loanDate,
                DueDate = dueDate
            };

            // Decrement available copies
            book.CopiesAvailable -= 1;

            _libraryController.SaveOrUpdateEntity(loan);
            _libraryController.SaveOrUpdateBook(book);

            View.ShowMessage("Book loaned successfully.", true);
        }
    }
}







