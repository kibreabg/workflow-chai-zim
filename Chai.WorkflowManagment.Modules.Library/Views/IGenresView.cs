using Chai.WorkflowManagment.CoreDomain.Library;
using System.Collections.Generic;

namespace Chai.WorkflowManagment.Modules.Library.Views
{
    public interface IGenresView
    {
        string GetName { get; }
        IList<Genre> GetGenres { get; set; }
    }
}
