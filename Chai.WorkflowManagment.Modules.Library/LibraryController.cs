using Chai.WorkflowManagment.CoreDomain;
using Chai.WorkflowManagment.CoreDomain.DataAccess;
using Chai.WorkflowManagment.CoreDomain.Library;
using Chai.WorkflowManagment.Services;
using Chai.WorkflowManagment.Shared.Navigation;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.CompositeWeb.Interfaces;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chai.WorkflowManagment.Modules.Library
{
    public class LibraryController : ControllerBase
    {
        private readonly IWorkspace _workspace;

        [InjectionConstructor]
        public LibraryController([ServiceDependency] IHttpContextLocatorService httpContextLocatorService, [ServiceDependency] INavigationService navigationService)
            : base(httpContextLocatorService, navigationService)
        {
            _workspace = ZadsServices.Workspace;
        }

        #region CurrenrObject
        public object CurrentObject
        {
            get
            {
                return GetCurrentContext().Session["CurrentObject"];
            }
            set
            {
                GetCurrentContext().Session["CurrentObject"] = value;
            }
        }
        #endregion

        #region Book
        public static IList<Book> GetBooks()
        {
            return WorkspaceFactory.CreateReadOnly().Query<Book>(null).OrderBy(x => x.Id).ToList();
        }
        public Book GetBookByAuthor(int authorId)
        {
            return _workspace.Single<Book>(x => x.Author.Id == authorId, y => y.Author, y => y.Genre);
        }
        public Book GetBook(int id)
        {
            return _workspace.Single<Book>(x => x.Id == id, y => y.Author, y => y.Genre);
        }
        public IList<Book> ListBooks(string authorId, string genreId, string title)
        {
            string filterExpression = "SELECT * FROM Books " +
                                      "WHERE 1 = CASE WHEN '" + authorId + "' = '' THEN 1 WHEN Books.Author_Id = '" + authorId + "' THEN 1 END " +
                                      "AND 1 = CASE WHEN '" + genreId + "' = '' THEN 1 WHEN Books.Genre_Id = '" + genreId + "' THEN 1 END " +
                                      "AND 1 = CASE WHEN '" + title + "' = '' THEN 1 WHEN Books.Title LIKE '%" + title + "%' THEN 1 END " +
                                      "ORDER BY Books.Id Desc";
            return _workspace.SqlQuery<Book>(filterExpression).ToList();
        }
        public void SaveOrUpdateBook(Book book)
        {
            if (book.Id <= 0)
            {
                using (var wr = WorkspaceFactory.CreateReadOnly())
                {
                    if (wr.Single<Book>(x => x.ISBN == book.ISBN) != null)
                        throw new InvalidOperationException("A book with the same ISBN already exists.");
                }
            }

            SaveOrUpdateEntity<Book>(book);
        }
        #endregion

        #region Author
        public static IList<Author> GetAuthors()
        {
            return WorkspaceFactory.CreateReadOnly().Query<Author>(null).OrderBy(x => x.Name).ToList();
        }
        public Author GetAuthor(int id)
        {
            return _workspace.Single<Author>(x => x.Id == id);
        }
        #endregion

        #region Genre
        public static IList<Genre> GetGenres()
        {
            return WorkspaceFactory.CreateReadOnly().Query<Genre>(null).OrderBy(x => x.Name).ToList();
        }
        public Genre GetGenre(int id)
        {
            return _workspace.Single<Genre>(x => x.Id == id);
        }
        #endregion

        #region Entity Manipulation
        public void SaveOrUpdateEntity<T>(T item) where T : class
        {
            IEntity entity = (IEntity)item;
            if (entity.Id == 0)
                _workspace.Add<T>(item);
            else
                _workspace.Update<T>(item);

            _workspace.CommitChanges();
            _workspace.Refresh(item);
        }
        public void DeleteEntity<T>(T item) where T : class
        {
            _workspace.Delete<T>(item);
            _workspace.CommitChanges();
        }
        #endregion
    }
}
