using Chai.WorkflowManagment.CoreDomain.Approval;
using Chai.WorkflowManagment.CoreDomain.Request;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.Shared.MailSender;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Chai.WorkflowManagment.Modules.Approval.Views
{
    public partial class frmPurchaseOrder : POCBasePage, IPurchaseOrderView
    {
        private PurchaseOrderPresenter _presenter;
        private PurchaseRequest _purchaserequest;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                if (_presenter.CurrentPurchaseRequest.PurchaseOrders == null)
                {
                    _presenter.CurrentPurchaseRequest.PurchaseOrders = new PurchaseOrder();

                }
                BindPurchaseOrder();
                //BindRepeater();  
            }
            this._presenter.OnViewLoaded();
            if (_presenter.CurrentPurchaseRequest.PurchaseOrders != null)
            {
                if (_presenter.CurrentPurchaseRequest.PurchaseOrders.Id != 0)
                {
                    PrintTransaction();
                    BindRepeater();
                }
            }
            //btnPrintworksheet.Attributes.Add("onclick", "javascript:Clickheretoprint('divprint'); return false;");
            //BindJS();
        }

        [CreateNew]
        public PurchaseOrderPresenter Presenter
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
                return "{64D3AC5F-DD78-414C-98F8-63EC02CB9673}";
            }
        }
        public CoreDomain.Request.PurchaseRequest PurchaseRequest
        {
            get
            {
                return _purchaserequest;
            }
            set
            {
                _purchaserequest = value;
            }
        }
        public string RequestNo
        {
            get { return string.Empty; }
        }
        public string RequestDate
        {
            get { return string.Empty; }
        }
        public int PurchaseRequestId
        {
            get
            {
                if (Convert.ToInt32(Request.QueryString["PurchaseRequestId"]) != 0)
                {
                    return Convert.ToInt32(Request.QueryString["PurchaseRequestId"]);
                }
                return 0;



            }
        }
        private void BindRepeater()
        {

            Repeater1.DataSource = _presenter.CurrentPurchaseRequest.PurchaseOrders.PurchaseOrderDetails;
            Repeater1.DataBind();

            Label lblPONumberP = Repeater1.Controls[0].Controls[0].FindControl("lblPONumberP") as Label;
            lblPONumberP.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.PoNumber;
            Label lblRequesterP = Repeater1.Controls[0].Controls[0].FindControl("lblRequesterP") as Label;
            lblRequesterP.Text = _presenter.GetUser(_presenter.CurrentPurchaseRequest.Requester).FullName;
            Label lblDateP = Repeater1.Controls[0].Controls[0].FindControl("lblDateP") as Label;
            lblDateP.Text = _presenter.CurrentPurchaseRequest.RequestedDate.ToShortDateString();
            if (_presenter.CurrentPurchaseRequest.PurchaseOrders.Supplier != null)
            {
                Label lblSupplierName = Repeater1.Controls[0].Controls[0].FindControl("lblSupplierName") as Label;
                lblSupplierName.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.Supplier.SupplierName;
                Label lblSupplierAddress = Repeater1.Controls[0].Controls[0].FindControl("lblSupplierAddress") as Label;
                lblSupplierAddress.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.Supplier.SupplierAddress;
                Label lblSupplierContactP = Repeater1.Controls[0].Controls[0].FindControl("lblSupplierContactP") as Label;
                lblSupplierContactP.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.Supplier.SupplierContact;
            }
            Label lblBillToP = Repeater1.Controls[0].Controls[0].FindControl("lblBillToP") as Label;
            lblBillToP.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.Billto;
            Label lblShipTo = Repeater1.Controls[0].Controls[0].FindControl("lblShipTo") as Label;
            lblShipTo.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.ShipTo;
            Label lblPaymentTermsP = Repeater1.Controls[0].Controls[0].FindControl("lblPaymentTermsP") as Label;
            lblPaymentTermsP.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.PaymentTerms;
            Label lblDeliveryFeesP = Repeater1.Controls[Repeater1.Controls.Count - 1].FindControl("lblDeliveryFeesP") as Label;
            lblDeliveryFeesP.Text = Convert.ToString(_presenter.CurrentPurchaseRequest.PurchaseOrders.DeliveryFees);
            Label lblItemTotalP = Repeater1.Controls[Repeater1.Controls.Count - 1].FindControl("lblItemTotalP") as Label;
            Label lblVatP = Repeater1.Controls[Repeater1.Controls.Count - 1].FindControl("lblVatP") as Label;
            Label lblTotalP = Repeater1.Controls[Repeater1.Controls.Count - 1].FindControl("lblTotalP") as Label;
            foreach (PurchaseOrderDetail POD in _presenter.CurrentPurchaseRequest.PurchaseOrders.PurchaseOrderDetails)
            {
                lblItemTotalP.Text = ((lblItemTotalP.Text != "" ? Convert.ToDecimal(lblItemTotalP.Text) : 0) + POD.TotalCost).ToString();
            }
            lblVatP.Text = Convert.ToString(0);
            lblTotalP.Text = (Convert.ToDecimal(lblItemTotalP.Text) + Convert.ToDecimal(lblVatP.Text) + Convert.ToDecimal(lblDeliveryFeesP.Text)).ToString();
            //lblAuthorizedBy.
        }
        private void AutoNumber()
        {
            txtPONo.Text = "PO-" + (_presenter.GetLastPurchaseOrderId() + 1);
        }
        private void BindPurchaseOrder()
        {
            this._presenter.OnViewLoaded();
            txtDate.Text = DateTime.Today.ToString();
            txtRequester.Text = _presenter.GetUser(_presenter.CurrentPurchaseRequest.Requester).FullName;
            if (_presenter.CurrentPurchaseRequest.BidAnalysises != null)
            {
                Bidder bider = _presenter.CurrentPurchaseRequest.BidAnalysises.GetBidderbyRank();

                if (bider != null)
                {
                    txtSupplierName.Text = bider.Supplier.SupplierName;
                    txtSupplierAddress.Text = bider.Supplier.SupplierAddress;
                    txtSupplierContact.Text = bider.Supplier.SupplierContact;
                }
            }

            if (_presenter.CurrentPurchaseRequest.PurchaseOrders != null)
            {
                if (_presenter.CurrentPurchaseRequest.PurchaseOrders.Id > 0)
                {
                    txtPONo.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.PoNumber;
                    txtDate.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.PODate.ToString();
                    txtShipTo.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.ShipTo;
                    txtBillto.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.Billto;
                    txtDeliveeryFees.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.DeliveryFees.ToString();
                    txtPaymentTerms.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.PaymentTerms;
                    btnPrintPurchaseOrder.Enabled = true;
                    btnPrintPurchaseForm.Enabled = true;
                }
                else
                {
                    txtDate.Text = DateTime.Today.ToString();
                    AutoNumber();
                }
            }

            AddPurchasingItem();
        }
        private void SavePurchaseOrder()
        {

            try
            {

                _presenter.CurrentPurchaseRequest.PurchaseOrders.PoNumber = txtPONo.Text;
                _presenter.CurrentPurchaseRequest.PurchaseOrders.PODate = Convert.ToDateTime(txtDate.Text);
                _presenter.CurrentPurchaseRequest.PurchaseOrders.Billto = txtBillto.Text;
                _presenter.CurrentPurchaseRequest.PurchaseOrders.ShipTo = txtShipTo.Text;
                _presenter.CurrentPurchaseRequest.PurchaseOrders.DeliveryFees = Convert.ToDecimal(txtDeliveeryFees.Text);
                _presenter.CurrentPurchaseRequest.PurchaseOrders.PaymentTerms = txtPaymentTerms.Text;
                _presenter.CurrentPurchaseRequest.PurchaseOrders.PurchaseRequest = _presenter.CurrentPurchaseRequest;
                if (_presenter.CurrentPurchaseRequest.BidAnalysises != null)
                {
                    _presenter.CurrentPurchaseRequest.PurchaseOrders.Supplier = _presenter.CurrentPurchaseRequest.BidAnalysises.GetBidderbyRank().Supplier;
                }
                //_presenter.CurrentPurchaseRequest.PurchaseOrders.Status = "Completed";       
                Master.ShowMessage(new AppMessage("Purchase Order Successfully Approved", Chai.WorkflowManagment.Enums.RMessageType.Info));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("There was an error Saving Purchase Order", Chai.WorkflowManagment.Enums.RMessageType.Error));

            }

        }
        private void BindPODetail()
        {

            dgPODetail.DataSource = _presenter.CurrentPurchaseRequest.PurchaseOrders.PurchaseOrderDetails;
            dgPODetail.DataBind();

        }

        #region PurchaseOrderDetail
        private void AddPurchasingItem()
        {
            if (_presenter.CurrentPurchaseRequest.PurchaseOrders.Id <= 0)
            {

                if (_presenter.CurrentPurchaseRequest.BidAnalysises != null)
                {

                    foreach (BidderItemDetail PR in _presenter.CurrentPurchaseRequest.BidAnalysises.GetBidderbyRank().BidderItemDetails)
                    {
                        PurchaseOrderDetail POD = new PurchaseOrderDetail();
                        POD.ItemAccount = _presenter.GetItemAccount(PR.ItemAccount.Id);
                        POD.Qty = PR.Qty;
                        POD.UnitCost = PR.UnitCost;
                        POD.TotalCost = PR.TotalCost;
                        _presenter.CurrentPurchaseRequest.PurchaseOrders.PurchaseOrderDetails.Add(POD);

                    }
                }
                else
                {
                    foreach (PurchaseRequestDetail PR in _presenter.CurrentPurchaseRequest.PurchaseRequestDetails)
                    {
                        PurchaseOrderDetail POD = new PurchaseOrderDetail();
                        POD.ItemAccount = _presenter.GetItemAccount(PR.ItemAccount.Id);
                        POD.Qty = PR.Qty;
                        POD.UnitCost = PR.Priceperunit;
                        POD.TotalCost = PR.EstimatedCost;
                        _presenter.CurrentPurchaseRequest.PurchaseOrders.PurchaseOrderDetails.Add(POD);

                    }
                }
            }

            BindPODetail();
        }
        #endregion
        protected void btnRequest_Click(object sender, EventArgs e)
        {
            try
            {
                SavePurchaseOrder();
                _presenter.SaveOrUpdatePurchaseRequest(_presenter.CurrentPurchaseRequest);
                BindRepeater();
                PrintTransaction();
                btnPrintPurchaseOrder.Enabled = true;
                btnPrintPurchaseForm.Enabled = true;
                // Response.Redirect(String.Format("frmPurchaseApproval.aspx?PurchaseRequestId={0}&PnlStatus={1}", _presenter.CurrentPurchaseRequest.Id, "Enabled"));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Unable to save Purchase order", Chai.WorkflowManagment.Enums.RMessageType.Error));
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("frmPurchaseApproval.aspx?PurchaseRequestId={0}&PnlStatus={1}", _presenter.CurrentPurchaseRequest.Id, "Enabled"));
        }
        private void PrintTransaction()
        {
            lblRequestNoResult.Text = _presenter.CurrentPurchaseRequest.RequestNo;
            lblRequesterResult.Text = _presenter.GetUser(_presenter.CurrentPurchaseRequest.Requester).FullName;
            lblRequestedDateResult.Text = _presenter.CurrentPurchaseRequest.RequestedDate.ToShortDateString();
            lblDeliverToResult.Text = _presenter.CurrentPurchaseRequest.DeliverTo;
            lblPurchaseOrderNo.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.PoNumber;
            lblDeliveryDateresult.Text = _presenter.CurrentPurchaseRequest.Requireddateofdelivery.ToShortDateString();
            lblPurposeResult.Text = _presenter.CurrentPurchaseRequest.Neededfor;
            lblBillToResult.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.Billto;
            lblShipToResult.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.ShipTo;
            lblPaymentTerms.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.PaymentTerms;
            lblDeliveryFeesResult.Text = _presenter.CurrentPurchaseRequest.PurchaseOrders.DeliveryFees.ToString();
            
            lblSuggestedSupplierResult.Text = _presenter.CurrentPurchaseRequest.SuggestedSupplier;
            if (_presenter.CurrentPurchaseRequest.BidAnalysises != null)
            {
                lblReasonforSelectionResult.Text = _presenter.CurrentPurchaseRequest.BidAnalysises.ReasonforSelection;
                lblSelectedbyResult.Text = _presenter.GetUser(_presenter.CurrentPurchaseRequest.BidAnalysises.SelectedBy).FullName;

                grvDetails.DataSource = _presenter.CurrentPurchaseRequest.BidAnalysises.GetAllBidderItemDetails();
                grvDetails.DataBind();
            }
            grvStatuses.DataSource = _presenter.CurrentPurchaseRequest.PurchaseRequestStatuses;
            grvStatuses.DataBind();
        }
        protected void grvStatuses_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (_presenter.CurrentPurchaseRequest.PurchaseRequestStatuses != null)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[1].Text = _presenter.GetUser(_presenter.CurrentPurchaseRequest.PurchaseRequestStatuses[e.Row.RowIndex].Approver).FullName;
                }
            }
        }
    }
}