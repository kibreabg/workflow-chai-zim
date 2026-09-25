using System;
using System.Web;
using Microsoft.Practices.CompositeWeb;
using Chai.WorkflowManagment.Services;
using Chai.WorkflowManagment.Modules.Shell.Impersonation;

namespace Chai.WorkflowManagment.Modules.Shell.Impersonation
{
    /// <summary>
    /// Module initializer for the Impersonation module
    /// </summary>
    public class ImpersonationModuleInitializer : ModuleInitializer
    {
        public override void Load(CompositionContainer container)
        {
            base.Load(container);
            
            // Register impersonation services
            WorkflowManagmentServices.ImpersonationManager = new ImpersonationManager();
            
            // Register impersonation controller
            WorkflowManagmentServices.ImpersonationController = new ImpersonationController(
                WorkflowManagmentServices.HttpContextLocatorService,
                WorkflowManagmentServices.NavigationService
            );
            
            // Register impersonation-related routes if needed
            // This depends on your routing configuration
        }
    }
}