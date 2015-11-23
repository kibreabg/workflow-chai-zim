using Chai.WorkflowManagment.CoreDomain.Request;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace Chai.WorkflowManagment.Modules.Approval.Views
{
    public partial class ucPrintRequestedItem : System.Web.UI.UserControl, IPrintRequestedItemView
    {
        private PrintRequestedItemPresenter _presenter;
        private PurchaseRequest _purchaserequest;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
            }
            this._presenter.OnViewLoaded();
        }

        [CreateNew]
        public PrintRequestedItemPresenter Presenter
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

        public PurchaseRequest PurchaseRequest
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string RequestNo
        {
            get { throw new NotImplementedException(); }
        }

        public string RequestDate
        {
            get { throw new NotImplementedException(); }
        }

        public int PurchaseRequestId
        {
            get { throw new NotImplementedException(); }
        }
    }
}