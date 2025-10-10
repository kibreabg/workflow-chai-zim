using Chai.WorkflowManagment.CoreDomain.Library;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.ObjectBuilder;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.Modules.Library.Views
{
    public class AuthorsPresenter : Presenter<IAuthorsView>
    {
        private readonly LibraryController _libraryController;
        public AuthorsPresenter([CreateNew] LibraryController libraryController)
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
        public IList<Author> ListAuthors(string name)
        {
            return _libraryController.ListAuthors(name);
        }
        public Author GetAuthor(int id)
        {
            return _libraryController.GetAuthor(id);
        }
        public void SaveOrUpdateAuthor(Author author)
        {
            _libraryController.SaveOrUpdateEntity(author);
        }
        public void RedirectPage(string url)
        {
            _libraryController.Navigate(url);
        }
    }

}
