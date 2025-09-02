using Chai.WorkflowManagment.CoreDomain;
using Chai.WorkflowManagment.Services;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.CompositeWeb.Interfaces;
using Microsoft.Practices.ObjectBuilder;

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
    }
}
