using Chai.WorkflowManagment.CoreDomain.Approval;
using Chai.WorkflowManagment.CoreDomain.Request;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.Shared.MailSender;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Chai.WorkflowManagment.Modules.Approval.Views
{
    public partial class frmPurchaseApprovalDetail : POCBasePage, IPurchaseApprovalDetailView
    {
        private PurchaseApprovalDetailPresenter _presenter;
        private PurchaseRequest _purchaserequest;
        private Bidder bidder;
        protected void Page_Load(object sender, EventArgs e)
        
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                if (_presenter.CurrentPurchaseRequest.BidAnalysises == null)
                {
                    _presenter.CurrentPurchaseRequest.BidAnalysises = new BidAnalysis();
               
               
                }
                BindBidAnalysis(); 
                BindBidder();
                BindAttachments();
                BindRepeater();
               

            }
            this._presenter.OnViewLoaded();
            //btnPrintworksheet.Attributes.Add("onclick", "javascript:Clickheretoprint('divprint'); return false;");
            //BindJS();
        }
      
        [CreateNew]
        public PurchaseApprovalDetailPresenter Presenter
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
                return "{2E6C8715-C968-4B45-8B2E-E9068A737637}";
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
           
            Repeater1.DataSource = _presenter.CurrentPurchaseRequest.PurchaseRequestDetails;
            Repeater1.DataBind();
            Label lblrequestNo = Repeater1.Controls[0].Controls[0].FindControl("lblrequestNo") as Label;
            lblrequestNo.Text = _presenter.CurrentPurchaseRequest.RequestNo;
            Label lblRequester = Repeater1.Controls[0].Controls[0].FindControl("lblRequester") as Label;
            lblRequester.Text = _presenter.GetUser(_presenter.CurrentPurchaseRequest.Requester).FullName;
            Label lblRequestDate0 = Repeater1.Controls[0].Controls[0].FindControl("lblRequestDate0") as Label;
            lblRequestDate0.Text = _presenter.CurrentPurchaseRequest.RequestedDate.ToShortDateString();
            Label lblneededfor = Repeater1.Controls[0].Controls[0].FindControl("lblneededfor") as Label;
            lblneededfor.Text = _presenter.CurrentPurchaseRequest.Neededfor;
            Label lblSpecialNeed = Repeater1.Controls[0].Controls[0].FindControl("lblSpecialNeed") as Label;
            lblSpecialNeed.Text = _presenter.CurrentPurchaseRequest.SpecialNeed;
            Label lblEstimatedTotalCost = Repeater1.Controls[0].Controls[0].FindControl("lblEstimatedTotalCost") as Label;
            lblEstimatedTotalCost.Text = _presenter.CurrentPurchaseRequest.TotalPrice.ToString();
            Label lblApprover = Repeater1.Controls[0].Controls[0].FindControl("lblApprovedBy") as Label;
            lblApprover.Text = _presenter.GetUser(_presenter.CurrentPurchaseRequest.PurchaseRequestStatuses[0].Approver).FullName;
        
        }
        private void BindBidAnalysis()
        {
            
                txtRequestNo.Text = _presenter.CurrentPurchaseRequest.RequestNo;
                txtRequestDate.Text = _presenter.CurrentPurchaseRequest.RequestedDate.ToString();
                txtneededfor.Text = _presenter.CurrentPurchaseRequest.Neededfor;
                txtSpecialNeed.Text = _presenter.CurrentPurchaseRequest.SpecialNeed;
                
                txtRequestDate.Text = _presenter.CurrentPurchaseRequest.RequestedDate.ToString();
                txtRequester.Text = _presenter.GetUser(_presenter.CurrentPurchaseRequest.Requester).FullName;
                txtRequestNo.Text = _presenter.CurrentPurchaseRequest.RequestNo;
               
                if (_presenter.CurrentPurchaseRequest.BidAnalysises.Id > 0)
                {
                txtAnalyzedDate.Text = _presenter.CurrentPurchaseRequest.BidAnalysises.AnalyzedDate.ToString();

                txtselectionfor.Text = _presenter.CurrentPurchaseRequest.BidAnalysises.ReasonforSelection;
                if (_presenter.CurrentUser().EmployeePosition.PositionName != "Admin/HR Assisitance (Driver)")
                    btnRequest.Enabled = false;
            }
        }
        private void SaveBidAnalysis()
        {
            
            try
            {
                
                    _presenter.CurrentPurchaseRequest.BidAnalysises.AnalyzedDate = Convert.ToDateTime(txtAnalyzedDate.Text);
                    _presenter.CurrentPurchaseRequest.BidAnalysises.Neededfor = txtneededfor.Text;
                    _presenter.CurrentPurchaseRequest.BidAnalysises.SpecialNeed = txtSpecialNeed.Text;
                    _presenter.CurrentPurchaseRequest.BidAnalysises.ReasonforSelection = txtselectionfor.Text;
                    _presenter.CurrentPurchaseRequest.BidAnalysises.SelectedBy = _presenter.CurrentUser().Id;
                    if (_presenter.CurrentPurchaseRequest.BidAnalysises.GetBidderbyRank().Supplier != null)
                        _presenter.CurrentPurchaseRequest.BidAnalysises.Supplier = _presenter.CurrentPurchaseRequest.BidAnalysises.GetBidderbyRank().Supplier;

                    _presenter.CurrentPurchaseRequest.BidAnalysises.PurchaseRequest = _presenter.CurrentPurchaseRequest;

                    _presenter.CurrentPurchaseRequest.BidAnalysises.Status = "Completed";
               
            }
            catch (Exception ex)
            {


            }

        }
      
        #region Bidders
        protected void btnCancedetail_Click(object sender, EventArgs e)
        {
            PnlShowBidder.Visible = false;
        }
        private void BindSupplier(DropDownList ddlSupplier,int SupplierTypeId)
        {
            if (ddlSupplier.Items.Count > 0)
            {
                ddlSupplier.Items.Clear();
            }
            ddlSupplier.DataSource = _presenter.GetSuppliers(SupplierTypeId);
            ddlSupplier.DataBind();
        }
        private void BindSupplierType(DropDownList ddlSupplierType)
        {
            ddlSupplierType.DataSource = _presenter.GetSupplierTypes();
            ddlSupplierType.DataBind();
        }
        private void BindBidder()
        {
            dgBidders.DataSource = _presenter.CurrentPurchaseRequest.BidAnalysises.Bidders;
            dgBidders.DataBind();
        }
        protected void dgBidders_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgBidders.EditItemIndex = -1;
        }
        protected void dgBidders_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            int id = (int)dgBidders.DataKeys[e.Item.ItemIndex];
            Chai.WorkflowManagment.CoreDomain.Approval.Bidder bidder = _presenter.CurrentPurchaseRequest.BidAnalysises.GetBidder(id);
            try
            {
                _presenter.DeleteBidder(bidder);
                BindBidder();

                Master.ShowMessage(new AppMessage("Bidder was Removed Successfully", Chai.WorkflowManagment.Enums.RMessageType.Info));
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: Unable to delete Bidder. " + ex.Message, Chai.WorkflowManagment.Enums.RMessageType.Error));
            }
        }
        protected void dgBidders_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            Chai.WorkflowManagment.CoreDomain.Approval.Bidder bidder = new Chai.WorkflowManagment.CoreDomain.Approval.Bidder();
            if (e.CommandName == "AddNew")
            {
                try
                {
                    DropDownList ddlSupplierType = e.Item.FindControl("ddlFSupplierType") as DropDownList;
                    bidder.SupplierType = _presenter.GetSupplierType(Convert.ToInt32(ddlSupplierType.SelectedValue));
                    DropDownList ddlSupplier = e.Item.FindControl("ddlFSupplier") as DropDownList;
                    bidder.Supplier = _presenter.GetSupplier(Convert.ToInt32(ddlSupplier.SelectedValue));
                    TextBox txtFLeadTimefromSupplier = e.Item.FindControl("txtFLeadTimefromSupplier") as TextBox;
                    bidder.LeadTimefromSupplier = txtFLeadTimefromSupplier.Text;
                    TextBox txtFHistoricalPerformance = e.Item.FindControl("txtFHistoricalPerformance") as TextBox;
                    bidder.HistoricalPerformance = txtFHistoricalPerformance.Text;
                    TextBox txtFSpecialTermsDelivery = e.Item.FindControl("txtFSpecialTermsDelivery") as TextBox;
                    bidder.SpecialTermsDelivery = txtFSpecialTermsDelivery.Text;
                    TextBox txtFRank = e.Item.FindControl("txtFRank") as TextBox;
                    bidder.Rank = Convert.ToInt32(txtFRank.Text);
                    _presenter.CurrentPurchaseRequest.BidAnalysises.Bidders.Add(bidder);
                    dgBidders.EditItemIndex = -1;
                    BindBidder();
                }
                catch (Exception ex)
                {
                    Master.ShowMessage(new AppMessage("Error: Unable to Add Bidder " + ex.Message, Chai.WorkflowManagment.Enums.RMessageType.Error));
                }
            }
        }

        
        protected void dgBidders_EditCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgBidders.EditItemIndex = e.Item.ItemIndex;

            BindBidder();
        }
        protected void dgBidders_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                DropDownList ddlFSupplierType = e.Item.FindControl("ddlFSupplierType") as DropDownList;
                BindSupplierType(ddlFSupplierType);
                DropDownList ddlFSupplier = e.Item.FindControl("ddlFSupplier") as DropDownList;
                BindSupplier(ddlFSupplier, int.Parse(ddlFSupplierType.SelectedValue));

            }
            else
            {


                if (_presenter.CurrentPurchaseRequest.BidAnalysises.Bidders != null)
                {


                    DropDownList ddlSupplierType = e.Item.FindControl("ddlSupplierType") as DropDownList;
                    if (ddlSupplierType != null)
                    {
                        BindSupplierType(ddlSupplierType);
                        if ((_presenter.CurrentPurchaseRequest.BidAnalysises.Bidders[e.Item.DataSetIndex].SupplierType.Id != null))
                        {
                            ListItem li = ddlSupplierType.Items.FindByValue(_presenter.CurrentPurchaseRequest.BidAnalysises.Bidders[e.Item.DataSetIndex].SupplierType.Id.ToString());
                            if (li != null)
                                li.Selected = true;
                        }

                    }

                    DropDownList ddlSupplier = e.Item.FindControl("ddlSupplier") as DropDownList;
                    if (ddlSupplierType != null)
                    {
                        BindSupplier(ddlSupplier, int.Parse(ddlSupplierType.SelectedValue));
                        if ((_presenter.CurrentPurchaseRequest.BidAnalysises.Bidders[e.Item.DataSetIndex].Supplier.Id != null))
                        {
                            ListItem liI = ddlSupplier.Items.FindByValue(_presenter.CurrentPurchaseRequest.BidAnalysises.Bidders[e.Item.DataSetIndex].Supplier.Id.ToString());
                            if (liI != null)
                                liI.Selected = true;
                        }

                    }

                }

            }
        }
        protected void dgBidders_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            int id = (int)dgBidders.DataKeys[e.Item.ItemIndex];
            Chai.WorkflowManagment.CoreDomain.Approval.Bidder bidder = _presenter.CurrentPurchaseRequest.BidAnalysises.GetBidder(id);
            
                try
                {
                    DropDownList ddlSupplierType = e.Item.FindControl("ddlSupplierType") as DropDownList;
                    bidder.SupplierType = _presenter.GetSupplierType(Convert.ToInt32(ddlSupplierType.SelectedValue));
                    DropDownList ddlSupplier = e.Item.FindControl("ddlSupplier") as DropDownList;
                    bidder.Supplier = _presenter.GetSupplier(Convert.ToInt32(ddlSupplier.SelectedValue));
                    TextBox txtFLeadTimefromSupplier = e.Item.FindControl("txtLeadTimefromSupplier") as TextBox;
                    bidder.LeadTimefromSupplier = txtFLeadTimefromSupplier.Text;
                    TextBox txtFHistoricalPerformance = e.Item.FindControl("txtHistoricalPerformance") as TextBox;
                    bidder.HistoricalPerformance = txtFHistoricalPerformance.Text;
                    TextBox txtFSpecialTermsDelivery = e.Item.FindControl("txtSpecialTermsDelivery") as TextBox;
                    bidder.SpecialTermsDelivery = txtFSpecialTermsDelivery.Text;
                    TextBox txtFRank = e.Item.FindControl("txtRank") as TextBox;
                    bidder.Rank = Convert.ToInt32(txtFRank.Text);
                    dgBidders.EditItemIndex = -1;
                    BindBidder();
                }
                catch (Exception ex)
                {
                    Master.ShowMessage(new AppMessage("Error: Unable to Update Bidder " + ex.Message, Chai.WorkflowManagment.Enums.RMessageType.Error));
                }
            

            
            
        }

        protected void dgBidders_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            int BidderId = (int)dgBidders.DataKeys[dgBidders.SelectedIndex];
            //Session["BidderId"] = BidderId;
           
            if (BidderId > 0)
                bidder = _presenter.CurrentPurchaseRequest.BidAnalysises.GetBidder(BidderId);
            else
                bidder = (Bidder) _presenter.CurrentPurchaseRequest.BidAnalysises.Bidders[dgBidders.SelectedIndex];
           // bidder = _presenter.CurrentPurchaseRequest.BidAnalysises.GetBidder(BidderId);
            Session["bidder"] = bidder;
            dgBidders.SelectedItemStyle.BackColor = System.Drawing.Color.BurlyWood;
            PnlShowBidder.Visible = true;
            BindItemDetails();
        }
        #endregion
        #region BidderItemDetail
        protected void btnAddItemdetail_Click(object sender, EventArgs e)
        {
            SetBidderItemDetail();
        }
        private void AddRequestedItem()
        {
            foreach (PurchaseRequestDetail PR in _presenter.CurrentPurchaseRequest.PurchaseRequestDetails)
            {
                BidderItemDetail BID = new BidderItemDetail();
                BID.ItemAccount = _presenter.GetItemAccount(PR.ItemAccount.Id);
                BID.Qty = PR.Qty;
                bidder.BidderItemDetails.Add(BID);
            }
        }
        private void BindItemDetails()
        {
            if (bidder.BidderItemDetails.Count == 0)
            {
                AddRequestedItem();
            }
            dgItemDetail.DataSource = bidder.BidderItemDetails;
            dgItemDetail.DataBind();
        }
        private void SetBidderItemDetail()
        {
            bidder = Session["bidder"] as Bidder;
            int index = 0;
            foreach (DataGridItem dgi in dgItemDetail.Items)
            {
                int id = (int)dgItemDetail.DataKeys[dgi.ItemIndex];

                BidderItemDetail detail;
                if (id > 0)
                    detail = bidder.GetBidderItemDetail(id);
                else
                    detail = (BidderItemDetail)bidder.BidderItemDetails[index];

                TextBox txtUnitCost = dgi.FindControl("txtUnitCost") as TextBox;
                detail.UnitCost = Convert.ToDecimal(txtUnitCost.Text);
                TextBox txtTotalCost = dgi.FindControl("txtTotalCost") as TextBox;
                detail.TotalCost = Convert.ToDecimal(txtTotalCost.Text);
                index++;

            }
            Master.ShowMessage(new AppMessage("Bidder Items successfully saved!", Chai.WorkflowManagment.Enums.RMessageType.Info));
        }
        
                
        #endregion
        #region Attachments
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            UploadFile();
        }
        protected void DownloadFile(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            Response.ContentType = ContentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
            Response.WriteFile(filePath);
            Response.End();
        }
        protected void DeleteFile(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            _presenter.CurrentPurchaseRequest.BidAnalysises.RemoveBAAttachment(filePath);
            File.Delete(Server.MapPath(filePath));
            grvAttachments.DataSource = _presenter.CurrentPurchaseRequest.BidAnalysises.BAAttachments;
            grvAttachments.DataBind();
            //Response.Redirect(Request.Url.AbsoluteUri);


        }
        private void UploadFile()
        {
            string fileName = Path.GetFileName(fuReciept.PostedFile.FileName);

            if (fileName != String.Empty)
            {



                BAAttachment attachment = new BAAttachment();
                attachment.FilePath = "~/BAUploads/" + fileName;
                fuReciept.PostedFile.SaveAs(Server.MapPath("~/BAUploads/") + fileName);
                //Response.Redirect(Request.Url.AbsoluteUri);
                _presenter.CurrentPurchaseRequest.BidAnalysises.BAAttachments.Add(attachment);

                grvAttachments.DataSource = _presenter.CurrentPurchaseRequest.BidAnalysises.BAAttachments;
                grvAttachments.DataBind();


            }
            else
            {
                Master.ShowMessage(new AppMessage("Please select file ", Chai.WorkflowManagment.Enums.RMessageType.Error));
            }
        }
        private void BindAttachments()
        {
            if (_presenter.CurrentPurchaseRequest.BidAnalysises.Id > 0)
            {
                grvAttachments.DataSource = _presenter.CurrentPurchaseRequest.BidAnalysises.BAAttachments;
                grvAttachments.DataBind();
            }
        }
        #endregion

        protected void dgItemDetail_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

        }
        protected void btnRequest_Click(object sender, EventArgs e)
        {
            try
            {
                if (_presenter.CurrentPurchaseRequest.BidAnalysises.Bidders.Count > 0)

                {
                    if (_presenter.CurrentPurchaseRequest.BidAnalysises.BAAttachments.Count > 0)
                    {
                    SaveBidAnalysis();
                    _presenter.SaveOrUpdatePurchaseRequest(_presenter.CurrentPurchaseRequest);
                    Response.Redirect(String.Format("frmPurchaseApproval.aspx?PurchaseRequestId={0}&PnlStatus={1}", _presenter.CurrentPurchaseRequest.Id, "Enabled"));
                    }
                    else
                    {

                        Master.ShowMessage(new AppMessage("You have to attach Citation ", Chai.WorkflowManagment.Enums.RMessageType.Error));
                    }
                }
                else
                {

                    Master.ShowMessage(new AppMessage("You have to insert bidders with item detail ", Chai.WorkflowManagment.Enums.RMessageType.Error));
                }
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Unable to save Bid Analysis", Chai.WorkflowManagment.Enums.RMessageType.Error));
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("frmPurchaseApproval.aspx?PurchaseRequestId={0}&PnlStatus={1}", _presenter.CurrentPurchaseRequest.Id,"Enabled"));
        }
        protected void txtUnitCost_TextChanged(object sender, EventArgs e)
        {
            TextBox txt =(TextBox)sender;
            HiddenField hfQty = txt.FindControl("hfqty") as HiddenField;
            TextBox txtUnitCost = txt.FindControl("txtUnitCost") as TextBox;
            TextBox txtTot = txt.FindControl("txtTotalCost") as TextBox;
            txtTot.Text = ((Convert.ToInt32(hfQty.Value) * Convert.ToDecimal(txtUnitCost.Text))).ToString();

        }
        protected void ddlFSupplierType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            DropDownList ddlFSupplier = ddl.FindControl("ddlFSupplier") as DropDownList;
            BindSupplier(ddlFSupplier, Convert.ToInt32(ddl.SelectedValue));
        }
        protected void ddlSupplierType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            DropDownList ddlSupplier = ddl.FindControl("ddlSupplier") as DropDownList;
            BindSupplier(ddlSupplier, Convert.ToInt32(ddl.SelectedValue));
        }
}
}