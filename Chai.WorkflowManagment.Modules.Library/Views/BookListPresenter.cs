using Chai.WorkflowManagment.CoreDomain.Library;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.ObjectBuilder;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.Modules.Library.Views
{
    public class BookListPresenter : Presenter<IBookListView>
    {
        private readonly LibraryController _libraryController;
        public BookListPresenter([CreateNew] LibraryController libraryController)
        {
            _libraryController = libraryController;
        }
        public override void OnViewLoaded()
        {
            // TODO: Implement code that will be executed every time the view loads
        }
        public override void OnViewInitialized()
        {
            // TODO: Implement code that will be executed the first time the view loads
        }
        public IList<Book> ListBooks(string authorId, string genreId, string title)
        {
            return _libraryController.ListBooks(authorId, genreId, title);
        }
        public IList<Author> GetAuthors()
        {
            return _libraryController.GetAuthors();
        }
        public IList<Genre> GetGenres()
        {
            return _libraryController.GetGenres();
        }
    }

}
