using Chai.WorkflowManagment.CoreDomain;
using Chai.WorkflowManagment.CoreDomain.DataAccess;
using System.Web;

namespace Chai.WorkflowManagment.Services
{
    public static class WorkflowManagmentServices
    {
		private const string WorkspaceKey = "WorkflowManagmentServices.Workspace";

		public static IWorkspace Workspace
		{
			get
			{
				var ctx = HttpContext.Current;
				if (ctx != null)
				{
					var existing = ctx.Items[WorkspaceKey] as IWorkspace;
					if (existing == null)
					{
						existing = WorkspaceFactory.Create();
						ctx.Items[WorkspaceKey] = existing;
					}
					return existing;
				}

				// Non-HTTP fallback — short lived
				return WorkspaceFactory.Create();
			}
			set
			{
				var ctx = HttpContext.Current;
				if (ctx != null)
					ctx.Items[WorkspaceKey] = value;
				// else ignore - non-HTTP callers should manage their own workspace lifetime
			}
		}

		private static AdminServices _adminServices;
        public static AdminServices AdminServices
        {
            get { return _adminServices ?? (_adminServices = new AdminServices()); }
            set { _adminServices = value; }
        }
    }
}
