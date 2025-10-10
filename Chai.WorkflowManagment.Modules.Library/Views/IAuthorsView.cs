using Chai.WorkflowManagment.CoreDomain.Library;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.Modules.Library.Views
{
    public interface IAuthorsView
    {
        string GetName { get; }
        IList<Author> GetAuthors { get; set; }
    }
}
