using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.CoreDomain.Setting;

using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared;
using log4net;
using log4net.Config;
using Microsoft.Practices.ObjectBuilder;


namespace Chai.WorkflowManagment.Modules.Request.Views
{
    public partial class frmCabRequest : POCBasePage, ICabRequestView
    {
        private CabRequestPresenter _presenter;
        private static readonly ILog Log = LogManager.GetLogger("AuditTrailLog");
        CabRequestDetail tac;
        private IList<TelephoneExtension> _telexts;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                XmlConfigurator.Configure();
                CheckApprovalSettings();
                
               
                BindCabRequests();
                //if (_presenter.CurrentCabRequest.Id <= 0)
                //{
                //    AutoNumber();
                //}
            }
           
            this._presenter.OnViewLoaded();
            BindCabRequests();

        }
        [CreateNew]
        public CabRequestPresenter Presenter
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
        public override string PageID
        {
            get
            {
                return "{3FB48884-3B42-47CB-B5DA-10D330CAED92}";
            }
        }

      
        #region Field Getters

     

        public IList<TelephoneExtension> telephoneextension
        {
            get
            {
                return _telexts;
            }
            set
            {
                _telexts = value;
            }
        }
        #endregion
        private string AutoNumber()
        {
            return "CAB-" + _presenter.CurrentUser().Id.ToString() + "-" + (_presenter.GetLastCabRequestId() + 1).ToString();
        }
        private void CheckApprovalSettings()
        {
            if (_presenter.GetApprovalSetting(RequestType.Cab_Request.ToString().Replace('_', ' '), 0) == null)
            {
               
            }
        }
     
        private void PopItemAccounts(DropDownList ddlAccountDescription)
        {
            ddlAccountDescription.DataSource = _presenter.GetItemAccounts();
            ddlAccountDescription.DataBind();
            ddlAccountDescription.SelectedValue = _presenter.GetDefaultItemAccount().Id.ToString();

            //ddlAccountDescription.Items.Insert(0, new ListItem("---Select Account Description---", "0"));
            //ddlAccountDescription.SelectedIndex = 0;
        }
        private void PopExpenseTypes(DropDownList ddlExpenseType)
        {
            ddlExpenseType.DataSource = _presenter.GetExpenseTypes();
            ddlExpenseType.DataBind();


            //ddlAccountDescription.Items.Insert(0, new ListItem("---Select Account Description---", "0"));
            //ddlAccountDescription.SelectedIndex = 0;
        }
       
       
      
        private void BindCabRequests()
        {
            grvPurchaseRequestList.DataSource = _presenter.ListTelephoneExtensions();
            grvPurchaseRequestList.DataBind();
        }
       
       
    }
}