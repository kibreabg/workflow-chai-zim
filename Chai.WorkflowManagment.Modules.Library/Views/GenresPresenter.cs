using Chai.WorkflowManagment.CoreDomain.Library;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.ObjectBuilder;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.Modules.Library.Views
{
    public class GenresPresenter : Presenter<IGenresView>
    {
        private readonly LibraryController _libraryController;
        public GenresPresenter([CreateNew] LibraryController libraryController)
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
        public IList<Genre> ListGenres(string name)
        {
            return _libraryController.ListGenres(name);
        }
        public Genre GetGenre(int id)
        {
            return _libraryController.GetGenre(id);
        }
        public void SaveOrUpdateGenre(Genre Genre)
        {
            _libraryController.SaveOrUpdateEntity(Genre);
        }
        public void RedirectPage(string url)
        {
            _libraryController.Navigate(url);
        }
    }

}
