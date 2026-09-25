using System;
using System.Web;
using System.Web.Security;
using Chai.WorkflowManagment.CoreDomain;
using Chai.WorkflowManagment.CoreDomain.Users;
using Chai.WorkflowManagment.Services;
using Microsoft.Practices.CompositeWeb;

namespace Chai.WorkflowManagment.Modules.Shell.Impersonation
{
    /// <summary>
    /// Manages user impersonation functionality for super admins
    /// </summary>
    public class ImpersonationManager
    {
        private const string IMPERSONATION_COOKIE_NAME = "SuperAdminImpersonationToken";
        private const string ORIGINAL_USER_ID_KEY = "OriginalUserId";
        private const string IMPERSONATED_USER_ID_KEY = "ImpersonatedUserId";
        
        private ShellController _controller;
        
        public ImpersonationManager()
        {
            _controller = WorkflowManagmentServices.ShellController;
        }
        
        /// <summary>
        /// Starts impersonating another user as the current super admin
        /// </summary>
        /// <param name="targetUserId">The ID of the user to impersonate</param>
        /// <returns>True if impersonation started successfully, false otherwise</param>
        public bool StartImpersonation(int targetUserId)
        {
            try
            {
                var currentUser = _controller.GetCurrentUser();
                if (currentUser == null || !currentUser.IsInRole("SuperAdmin"))
                {
                    throw new Exception("Only Super Admins can impersonate users");
                }
                
                var targetUser = _controller.GetUser(targetUserId);
                if (targetUser == null || !targetUser.IsActive)
                {
                    throw new Exception("Target user not found or is inactive");
                }
                
                // Store original user ID in session/cookie
                HttpContext.Current.Session[ORIGINAL_USER_ID_KEY] = currentUser.Id;
                HttpContext.Current.Session[IMPERSONATED_USER_ID_KEY] = targetUserId;
                
                // Create impersonation token
                string token = GenerateImpersonationToken(currentUser.Id, targetUserId);
                HttpCookie impersonationCookie = new HttpCookie(IMPERSONATION_COOKIE_NAME, token);
                impersonationCookie.Expires = DateTime.Now.AddHours(1); // Token expires in 1 hour
                HttpContext.Current.Response.Cookies.Add(impersonationCookie);
                
                // Update the current user context to be the impersonated user
                UpdateCurrentUserContext(targetUser);
                
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to start impersonation: " + ex.Message, ex);
            }
        }
        
        /// <summary>
        /// Stops the current impersonation and restores the original user
        /// </summary>
        public void StopImpersonation()
        {
            try
            {
                int originalUserId = HttpContext.Current.Session[ORIGINAL_USER_ID_KEY] != null ? 
                    (int)HttpContext.Current.Session[ORIGINAL_USER_ID_KEY] : 0;
                
                if (originalUserId > 0)
                {
                    var originalUser = _controller.GetUser(originalUserId);
                    if (originalUser != null)
                    {
                        UpdateCurrentUserContext(originalUser);
                    }
                }
                
                // Clear session variables
                HttpContext.Current.Session.Remove(ORIGINAL_USER_ID_KEY);
                HttpContext.Current.Session.Remove(IMPERSONATED_USER_ID_KEY);
                
                // Remove impersonation cookie
                if (HttpContext.Current.Request.Cookies[IMPERSONATION_COOKIE_NAME] != null)
                {
                    HttpCookie impersonationCookie = new HttpCookie(IMPERSONATION_COOKIE_NAME);
                    impersonationCookie.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(impersonationCookie);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to stop impersonation: " + ex.Message, ex);
            }
        }
        
        /// <summary>
        /// Checks if the current user is impersonating another user
        /// </summary>
        /// <returns>True if impersonation is active, false otherwise</returns>
        public bool IsImpersonating()
        {
            return HttpContext.Current.Session[IMPERSONATED_USER_ID_KEY] != null;
        }
        
        /// <summary>
        /// Gets the ID of the user being impersonated (if any)
        /// </summary>
        /// <returns>The ID of the impersonated user, or 0 if not impersonating</returns>
        public int GetImpersonatedUserId()
        {
            return HttpContext.Current.Session[IMPERSONATED_USER_ID_KEY] != null ? 
                (int)HttpContext.Current.Session[IMPERSONATED_USER_ID_KEY] : 0;
        }
        
        /// <summary>
        /// Gets the ID of the original user (if impersonation is active)
        /// </summary>
        /// <returns>The ID of the original user, or 0 if not impersonating</returns>
        public int GetOriginalUserId()
        {
            return HttpContext.Current.Session[ORIGINAL_USER_ID_KEY] != null ? 
                (int)HttpContext.Current.Session[ORIGINAL_USER_ID_KEY] : 0;
        }
        
        /// <summary>
        /// Updates the current user context with the specified user
        /// </summary>
        /// <param name="user">The user to set as current</param>
        private void UpdateCurrentUserContext(AppUser user)
        {
            // Create a new ChaiPrincipal with the impersonated user
            ChaiPrincipal principal = new ChaiPrincipal(user);
            HttpContext.Current.User = principal;
            
            // Update Forms Authentication cookie
            FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);
            
            // Update the workspace's current user context if needed
            // This depends on how your workspace factory works
        }
        
        /// <summary>
        /// Generates an impersonation token
        /// </summary>
        /// <param name="originalUserId">The ID of the original user</param>
        /// <param name="impersonatedUserId">The ID of the impersonated user</param>
        /// <returns>An impersonation token</returns>
        private string GenerateImpersonationToken(int originalUserId, int impersonatedUserId)
        {
            // Simple token generation - in production, use a more secure method
            string token = string.Format("{0}:{1}:{2}", originalUserId, impersonatedUserId, DateTime.Now.Ticks);
            return Encryption.StringToMD5Hash(token);
        }
        
        /// <summary>
        /// Validates an impersonation token
        /// </summary>
        /// <param name="token">The token to validate</param>
        /// <returns>True if the token is valid, false otherwise</returns>
        private bool ValidateImpersonationToken(string token)
        {
            // Implementation would validate the token format and expiration
            return !string.IsNullOrEmpty(token);
        }
    }
}