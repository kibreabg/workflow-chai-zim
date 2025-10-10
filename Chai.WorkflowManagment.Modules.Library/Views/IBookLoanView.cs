namespace Chai.WorkflowManagment.Modules.Library.Views
{
    using Chai.WorkflowManagment.CoreDomain.Library;

    public interface IBookLoanView
    {
        void ShowBookDetails(Book book);
        void ShowMessage(string message, bool isSuccess = false);
    }
}




