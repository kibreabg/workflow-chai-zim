using System.Collections.Generic;
using System.Linq;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Services;
using Chai.WorkflowManagment.Shared;
using Microsoft.Practices.CompositeWeb;
using Microsoft.Practices.CompositeWeb.Interfaces;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeWeb.Services;
using Microsoft.Practices.CompositeWeb.Configuration;
using Chai.WorkflowManagment.Shared.Navigation;

namespace Chai.WorkflowManagment.Modules.Shell.Impersonation
{
    /// <summary>
    /// Controller for handling user impersonation requests
    /// </summary>
    public class ImpersonationController : ControllerBase
    {
        private ImpersonationManager _impersonationManager;
        
        [InjectionConstructor]
        public ImpersonationController(
            [ServiceDependency] IHttpContextLocatorService httpContextLocatorService,
            [ServiceDependency] INavigationService navigationService
        ) : base(httpContextLocatorService, navigationService)
        {
            _impersonationManager = new ImpersonationManager();
        }
        
        /// <summary>
        /// Starts impersonating another user
        /// </summary>
        /// <param name="userId">The ID of the user to impersonate</param>
        /// <returns>JSON result indicating success or failure</returns>
        public string StartImpersonation(int userId)
        {
            try
            {
                bool result = _impersonationManager.StartImpersonation(userId);
                return "{\"success\": true, \"message\": \"Impersonation started successfully\", \"userId\": " + userId + "}";
            }
            catch (Exception ex)
            {
                return "{\"success\": false, \"message\": \"" + ex.Message.Replace("\\", "\\\\").Replace("\n", "\\n") + "\"}";
            }
        }
        
        /// <summary>
        /// Stops the current impersonation
        /// </summary>
        /// <returns>JSON result indicating success or failure</returns>
        public string StopImpersonation()
        {
            try
            {
                _impersonationManager.StopImpersonation();
                return "{\"success\": true, \"message\": \"Impersonation stopped successfully\"}";
            }
            catch (Exception ex)
            {
                return "{\"success\": false, \"message\": \"" + ex.Message.Replace("\\", "\\\\").Replace("\n", "\\n") + "\"}";
            }
        }
        
        /// <summary>
        /// Checks if impersonation is currently active
        /// </summary>
        /// <returns>JSON result with impersonation status</returns>
        public string GetImpersonationStatus()
        {
            try
            {
                bool isImpersonating = _impersonationManager.IsImpersonating();
                int impersonatedUserId = _impersonationManager.GetImpersonatedUserId();
                int originalUserId = _impersonationManager.GetOriginalUserId();
                
                var currentUser = GetCurrentUser();
                
                return "{\"success\": true, \"isImpersonating\": " + isImpersonating.ToString().ToLower() + ", \"impersonatedUserId\": " + impersonatedUserId + ", \"originalUserId\": " + originalUserId + ", \"currentUserId\": " + (currentUser != null ? currentUser.Id : 0) + ", \"currentUserName\": \"" + (currentUser != null ? currentUser.UserName.Replace("\\", "\\\\").Replace("\n", "\\n") : "") + "\"}";
            }
            catch (Exception ex)
            {
                return "{\"success\": false, \"message\": \"" + ex.Message.Replace("\\", "\\\\").Replace("\n", "\\n") + "\"}";
            }
        }
        
        /// <summary>
        /// Gets the list of users that can be impersonated (excluding current user and super admins)
        /// </summary>
        /// <returns>JSON result with list of users</returns>
        public string GetImpersonatableUsers()
        {
            try
            {
                var currentUser = GetCurrentUser();
                if (currentUser == null || !currentUser.IsInRole("SuperAdmin"))
                {
                    return "{\"success\": false, \"message\": \"Only Super Admins can view impersonatable users\"}";
                }
                
                var workspace = WorkflowManagmentServices.Workspace;
                var users = workspace.All<AppUser>(x => x.IsActive && x.Id != currentUser.Id)
                    .Select(u => new {
                        id = u.Id,
                        userName = u.UserName,
                        firstName = u.FirstName,
                        lastName = u.LastName,
                        email = u.Email,
                        employeeNo = u.EmployeeNo
                    })
                    .ToList();
                
                // Build JSON manually to avoid JsonResult dependency
                string usersJson = "[{";
                for (int i = 0; i < users.Count; i++)
                {
                    var user = users[i];
                    usersJson += "\"id\": " + user.Id + ", \"userName\": \"" + user.UserName.Replace("\\", "\\\\").Replace("\n", "\\n") + "\", \"firstName\": \"" + user.FirstName.Replace("\\", "\\\\").Replace("\n", "\\n") + "\", \"lastName\": \"" + user.LastName.Replace("\\", "\\\\").Replace("\n", "\\n") + "\", \"email\": \"" + user.Email.Replace("\\", "\\\\").Replace("\n", "\\n") + "\", \"employeeNo\": \"" + user.EmployeeNo.Replace("\\", "\\\\").Replace("\n", "\\n") + "\"";
                    if (i < users.Count - 1)
                        usersJson += "}, ";
                    else
                        usersJson += "}";
                }
                usersJson += "}]";
                
                return "{\"success\": true, \"users\": " + usersJson + "}";
            }
            catch (Exception ex)
            {
                return "{\"success\": false, \"message\": \"" + ex.Message.Replace("\\", "\\\\").Replace("\n", "\\n") + "\"}";
            }
        }
    }
}