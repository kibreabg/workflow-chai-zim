using Chai.WorkflowManagment.CoreDomain.Library;

namespace Chai.WorkflowManagment.Modules.Library.Views
{
    public interface IBookEditView
    {
        int GetBookId { get; }
        string GetTitle { get; }
        string GetISBN { get; }
        int GetPublishedYear { get; }
        int GetCopiesAvailable { get; }
        Author Author { get; }
        Genre Genre { get; }

    }
}




