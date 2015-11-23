using System;
using Microsoft.Practices.ObjectBuilder;

namespace Chai.WorkflowManagment.Modules.Shell.Views
{
    public partial class UserLogin : Microsoft.Practices.CompositeWeb.Web.UI.Page, IUserLoginView
    {
        private UserLoginPresenter _presenter;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.User != null)
            {
                if (Context.User.Identity.IsAuthenticated)
                    Context.Response.Redirect("~/");
            }

            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                txtUsername.Focus();
            }
            this._presenter.OnViewLoaded();
        }

        [CreateNew]
        public UserLoginPresenter Presenter
        {
            get
            {
                return this._presenter;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this._presenter = value;
                this._presenter.View = this;
            }
        }
        protected void btnLogin_Click(object sender, EventArgs e)   
        {
            if (this.txtUsername.Text.Trim().Length > 0 && this.txtPassword.Text.Trim().Length > 0)
            {
                try
                {
                    if (_presenter.AuthenticateUser())
                    {
                        //_presenter.RedirectToRowUrl();
                        Context.Response.Redirect("Default.aspx");
                    }
                    else
                    {
                        this.lblLoginError.Text = "User name or password incorrect";
                        this.lblLoginError.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    this.lblLoginError.Text = ex.Message + " The user may be not active user";
                    this.lblLoginError.Visible = true;
                }
            }
            else
            {
                this.lblLoginError.Text = "Please enter both a username and password";
                this.lblLoginError.Visible = true;
            }

        }


        #region IUserLoginView Members

        public string GetUserName
        {
            get { return txtUsername.Text; }
        }

        public string GetPassword
        {
            get { return txtPassword.Text; }
        }

        public bool PersistLogin
        {
            get { return chkPersistLogin.Checked; }
        }

        #endregion
    }
}

