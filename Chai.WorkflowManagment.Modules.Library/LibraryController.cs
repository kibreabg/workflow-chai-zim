using Chai.WorkflowManagment.CoreDomain;
using Chai.WorkflowManagment.CoreDomain.DataAccess;
using Chai.WorkflowManagment.CoreDomain.Library;
using Chai.WorkflowManagment.Services;
using Chai.WorkflowManagment.Shared.Navigation;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.CompositeWeb.Interfaces;
using Microsoft.Practices.ObjectBuilder;
using System.Collections.Generic;
using System.Linq;

namespace Chai.WorkflowManagment.Modules.Library
{
    public class LibraryController : ControllerBase
    {
        private IWorkspace _workspace;

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
        public IList<Book> GetBooks()
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
        public IList<Book> ListBooks(string authorId)
        {
            string filterExpression = "SELECT * FROM Books WHERE 1 = CASE WHEN '" + authorId + "' = '' THEN 1 WHEN Books.Author_Id = '" + authorId + "' THEN 1 END ORDER BY Books.Id Desc";
            return _workspace.SqlQuery<Book>(filterExpression).ToList();
        }
        #endregion
    }
}
