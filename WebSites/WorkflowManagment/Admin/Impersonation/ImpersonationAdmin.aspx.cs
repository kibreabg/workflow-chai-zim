using System;
using System.Web.UI;
using Chai.WorkflowManagment.Modules.Shell.Impersonation;
using Chai.WorkflowManagment.Services;
using Chai.WorkflowManagment.CoreDomain.Users;

namespace Chai.WorkflowManagment.Modules.Shell.Impersonation
{
    public partial class ImpersonationAdmin : Page
    {
        private ImpersonationController _impersonationController;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize the impersonation controller
                _impersonationController = new ImpersonationController(
                    WorkflowManagmentServices.HttpContextLocatorService,
                    WorkflowManagmentServices.NavigationService
                );
                
                // Load impersonation status and users
                LoadImpersonationStatus();
                LoadImpersonatableUsers();
            }
        }
        
        private void LoadImpersonationStatus()
        {
            var statusResult = _impersonationController.GetImpersonationStatus();
            if (statusResult.Success)
            {
                var data = statusResult.Data;
                string statusText = data.IsImpersonating ? 
                    "<strong>Impersonating:</strong> You are currently impersonating user ID " + data.ImpersonatedUserId + ". " :
                    "<strong>Status:</strong> You are not impersonating any user. ";
                
                string stopButton = data.IsImpersonating ? 
                    "<button class='stop-impersonation' onclick='stopImpersonation()'>Stop Impersonation</button>" : 
                    "";
                
                string impersonatedUserSpan = data.IsImpersonating ? 
                    "<span class='impersonated-user'>(Impersonated User: " + data.ImpersonatedUserId + "</span>)" : 
                    "";
                
                impersonationStatus.InnerHtml = statusText + impersonatedUserSpan;
                currentUserSpan.InnerText = data.CurrentUserName;
                
                if (data.IsImpersonating)
                {
                    stopImpersonationBtn.Style["display"] = "inline-block";
                }
                else
                {
                    stopImpersonationBtn.Style["display"] = "none";
                }
            }
        }
        
        private void LoadImpersonatableUsers()
        {
            var usersResult = _impersonationController.GetImpersonatableUsers();
            if (usersResult.Success)
            {
                string usersHtml = "";
                foreach (var user in usersResult.Users)
                {
                    usersHtml += "<div class='user-item'>" +
                                 "<span>" + user.FirstName + " " + user.LastName + " (" + user.UserName + ") - " + user.Email + "</span>" +
                                 "<button class='impersonate-btn' onclick=\"impersonateUser('" + user.Id + "', '\" + user.UserName + "')\">Impersonate</button>" +
                                 "</div>";
                }
                
                if (string.IsNullOrEmpty(usersHtml))
                {
                    userList.InnerHtml = "<div class='status not-impersonating'>No users available for impersonation.</div>";
                }
                else
                {
                    userList.InnerHtml = usersHtml;
                }
            }
            else
            {
                userList.InnerHtml = "<div class='status not-impersonating'>Error loading users: " + usersResult.Message + "</div>";
            }
        }
        
        protected void btnStartImpersonation_Click(object sender, EventArgs e)
        {
            // This method is kept for backward compatibility
            // The actual impersonation is handled via AJAX calls
        }
        
        protected void btnStopImpersonation_Click(object sender, EventArgs e)
        {
            // This method is kept for backward compatibility
            // The actual impersonation is handled via AJAX calls
        }
        
        // Helper method to handle AJAX callbacks
        protected void Page_Init(object sender, EventArgs e)
        {
            // Register the page for AJAX callbacks if needed
            this.Load += new EventHandler(Page_Load);
        }
    }
}