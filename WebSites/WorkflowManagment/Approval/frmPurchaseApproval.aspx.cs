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
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.CoreDomain.Approval;
using log4net;
using System.Reflection;
using log4net.Config;
using System.IO;

namespace Chai.WorkflowManagment.Modules.Approval.Views
{
    public partial class frmPurchaseApproval : POCBasePage, IPurchaseApprovalView
    {
        private PurchaseApprovalPresenter _presenter;
        private static readonly ILog Log = LogManager.GetLogger("AuditTrailLog");
        private PurchaseRequest _purchaserequest;
        private int reqid;
        private int PurchaseId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                XmlConfigurator.Configure();
                PopProgressStatus();
                if (_presenter.CurrentPurchaseRequest != null)
                {
                    if (_presenter.CurrentPurchaseRequest.Id > 0)
                    {
                        PopApprovalStatus();
                    }
                }
            }
            this._presenter.OnViewLoaded();
            BindSearchPurchaseRequestGrid();
            ReturnFromBidAnalysis();
            if(_presenter.CurrentPurchaseRequest !=null)
            { 
            ChangeBidAnalysisLink();
            }
        
        }
        protected void BindJS()
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "workflowscripts", String.Format("<script language=\"JavaScript\" src=\"http://localhost/WorkflowManagment/WorkflowManagment.js\"></script>\n"));
        }
        [CreateNew]
        public PurchaseApprovalPresenter Presenter
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
                return "{BF5943CA-07E5-4817-93EC-DE39D67D5562}";
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
            get { return txtRequestNosearch.Text; }
        }
        public string RequestDate
        {
            get { return txtRequestDatesearch.Text; }
        }
   
        public int PurchaseRequestId
        {
            get
            {
                if (grvPurchaseRequestList.SelectedDataKey != null)
                {
                    return Convert.ToInt32(grvPurchaseRequestList.SelectedDataKey.Value);


                }
                else if (Convert.ToInt32(Request.QueryString["PurchaseRequestId"]) != 0)
                {
                    return Convert.ToInt32(Request.QueryString["PurchaseRequestId"]);
                }
                else if (Convert.ToInt32(Session["ReqID"]) != 0)
                {
                    return Convert.ToInt32(Session["ReqID"]);
                }

                else
                {
                    return 0;
                }
            }
            set
            {
                reqid = value;
            }

        }
        public string PnlStatus
        {
            get
            {
                if (Convert.ToString(Request.QueryString["PnlStatus"]) != null)
                    return Convert.ToString(Request.QueryString["PnlStatus"]);
                else
                    return "Disabled";
            }
        }
        private void PopApprovalStatus()
        {
            ddlApprovalStatus.Items.Clear();
            ddlApprovalStatus.Items.Add(new ListItem("Select Status", "0"));
            string[] s = Enum.GetNames(typeof(ApprovalStatus));

            for (int i = 0; i < s.Length; i++)
            {
                if (GetWillStatus().Substring(0, 3) == s[i].Substring(0, 3))
                {
                    ddlApprovalStatus.Items.Add(new ListItem(s[i].Replace('_', ' '), s[i].Replace('_', ' ')));

                }

            }
            ddlApprovalStatus.Items.Add(new ListItem(ApprovalStatus.Rejected.ToString().Replace('_', ' '), ApprovalStatus.Rejected.ToString().Replace('_', ' ')));

        }
        private string GetWillStatus()
        {
            ApprovalSetting AS = _presenter.GetApprovalSettingforProcess(RequestType.Purchase_Request.ToString().Replace('_', ' ').ToString(), _presenter.CurrentPurchaseRequest.TotalPrice);
            string will = "";
            foreach (ApprovalLevel AL in AS.ApprovalLevels)
            {
                if (AL.EmployeePosition.PositionName == "Superviser/Line Manager" || AL.EmployeePosition.PositionName == "Program Manager" && _presenter.CurrentPurchaseRequest.CurrentLevel == 1)
                {
                    will = "Approve";
                    break;
                    

                }
                else if (_presenter.GetUser(_presenter.CurrentPurchaseRequest.CurrentApprover).EmployeePosition.PositionName == AL.EmployeePosition.PositionName)
                {
                    will = AL.Will;

                }

            }
            return will;
        }
        private void PopProgressStatus()
        {
            string[] s = Enum.GetNames(typeof(ProgressStatus));

            for (int i = 0; i < s.Length; i++)
            {
                ddlProgressStatus.Items.Add(new ListItem(s[i].Replace('_', ' '), s[i].Replace('_', ' ')));

            }


        }
        private void BindPurchaseRequestStatus()
        {
            foreach (PurchaseRequestStatus PRS in _presenter.CurrentPurchaseRequest.PurchaseRequestStatuses)
            {
                if (PRS.WorkflowLevel == _presenter.CurrentPurchaseRequest.CurrentLevel && PRS.WorkflowLevel == _presenter.CurrentPurchaseRequest.CurrentLevel)
                {
                    ddlApprovalStatus.SelectedValue = PRS.ApprovalStatus;
                    txtRejectedReason.Text = PRS.RejectedReason;
                    btnApprove.Enabled = true;

                }
                if (_presenter.CurrentPurchaseRequest.CurrentLevel == _presenter.CurrentPurchaseRequest.PurchaseRequestStatuses.Count && (PRS.ApprovalStatus != null) && _presenter.CurrentPurchaseRequest.ProgressStatus == ProgressStatus.Completed.ToString())
                {
                    btnPurchaseOrder.Visible = true;
                }
            }
        }
        private void ShowPrint()
        {
            if (_presenter.CurrentPurchaseRequest.CurrentLevel == _presenter.CurrentPurchaseRequest.PurchaseRequestStatuses.Count && _presenter.CurrentPurchaseRequest.ProgressStatus == ProgressStatus.Completed.ToString())
            {
                btnPurchaseOrder.Visible = true;

            }

        }
        private void SavePurchaseRequestStatus()
        {

            foreach (PurchaseRequestStatus PRS in _presenter.CurrentPurchaseRequest.PurchaseRequestStatuses)
            {
                if ((PRS.Approver == _presenter.CurrentUser().Id || _presenter.CurrentUser().Id == (_presenter.GetAssignedJobbycurrentuser(PRS.Approver) != null ? _presenter.GetAssignedJobbycurrentuser(PRS.Approver).AssignedTo : 0)) && PRS.WorkflowLevel == _presenter.CurrentPurchaseRequest.CurrentLevel)
                {
                    PRS.ApprovalDate = DateTime.Now;
                    PRS.ApprovalStatus = ddlApprovalStatus.SelectedValue;
                    PRS.AssignedBy = _presenter.GetAssignedJobbycurrentuser(PRS.Approver) != null ? _presenter.GetAssignedJobbycurrentuser(PRS.Approver).AppUser.FullName : "";
                    PRS.RejectedReason = txtRejectedReason.Text;
                    if (PRS.ApprovalStatus != ApprovalStatus.Rejected.ToString())
                    {
                        if (_presenter.CurrentPurchaseRequest.CurrentLevel == _presenter.CurrentPurchaseRequest.PurchaseRequestStatuses.Count)
                            _presenter.CurrentPurchaseRequest.ProgressStatus = ProgressStatus.Completed.ToString();
                        GetNextApprover();
                        PRS.Approver = _presenter.CurrentUser().Id;
                        Log.Info(_presenter.GetUser(PRS.Approver).FullName + " has " + PRS.ApprovalStatus + " Purchase Request made by " + _presenter.GetUser(_presenter.CurrentPurchaseRequest.Requester).FullName);
                    }
                    else
                    {
                        _presenter.CurrentPurchaseRequest.ProgressStatus = ProgressStatus.Completed.ToString();
                        PRS.Approver = _presenter.CurrentUser().Id;
                        SendEmailRejected(PRS);
                        Log.Info(_presenter.GetUser(PRS.Approver).FullName + " has " + PRS.ApprovalStatus + " Purchase Request made by " + _presenter.GetUser(_presenter.CurrentPurchaseRequest.Requester).FullName);
                    }
                    break;
                }

            }

        }
        private void SendEmailRejected(PurchaseRequestStatus PRS)
        {
            EmailSender.Send(_presenter.GetUser(_presenter.CurrentPurchaseRequest.Requester).Email, "Purchase Request", "'" + "' Your Purchase Request No. '" + _presenter.CurrentPurchaseRequest.RequestNo + "' was Rejected for this reason '" + PRS.RejectedReason + "'");

            if (PRS.WorkflowLevel > 1)
            {
                for (int i = 0; i + 1 < PRS.WorkflowLevel; i++)
                {
                    EmailSender.Send(_presenter.GetUser(_presenter.CurrentPurchaseRequest.PurchaseRequestStatuses[i].Approver).Email, "Purchase Request Rejection", "'" + "' Purchase Request made by " + _presenter.GetUser(_presenter.CurrentPurchaseRequest.Requester).FullName + " was Rejected for this reason - '" + PRS.RejectedReason + "'");
                }
            }
        }
        private void GetNextApprover()
        {
            foreach (PurchaseRequestStatus PRS in _presenter.CurrentPurchaseRequest.PurchaseRequestStatuses)
            {
                if (PRS.ApprovalStatus == null)
                {
                    SendEmail(PRS);
                    _presenter.CurrentPurchaseRequest.CurrentApprover = PRS.Approver;
                    _presenter.CurrentPurchaseRequest.CurrentLevel = PRS.WorkflowLevel;
                    _presenter.CurrentPurchaseRequest.ProgressStatus = ProgressStatus.InProgress.ToString();
                    _presenter.CurrentPurchaseRequest.CurrentStatus = PRS.ApprovalStatus;
                    break;

                }
            }
        }
        private void SendEmail(PurchaseRequestStatus PRS)
        {
            if (_presenter.GetUser(PRS.Approver).IsAssignedJob != true)
            {
                EmailSender.Send(_presenter.GetUser(PRS.Approver).Email, "Purchase Request", _presenter.GetUser(_presenter.CurrentPurchaseRequest.Requester).FullName + "' Request for Purchase No '" + _presenter.CurrentPurchaseRequest.RequestNo + "'");
            }
            else
            {
                EmailSender.Send(_presenter.GetUser(_presenter.GetAssignedJobbycurrentuser(PRS.Approver).AssignedTo).Email, "Purchase Request", _presenter.GetUser(_presenter.CurrentPurchaseRequest.Requester).FullName + "' Request for Purchase No '" + _presenter.CurrentPurchaseRequest.RequestNo + "'");
            }
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                //if (_presenter.CurrentUser().EmployeePosition.PositionName == "Admin Assistant (Driver)" && _presenter.CurrentPurchaseRequest.TotalPrice > 500)
                //{
                if (_presenter.CurrentPurchaseRequest.ProgressStatus != ProgressStatus.Completed.ToString())
                {
                    SavePurchaseRequestStatus();
                    Session["PurchaseId"] = _presenter.CurrentPurchaseRequest.Id;
                    _presenter.SaveOrUpdatePurchaseRequest(_presenter.CurrentPurchaseRequest);
                    ShowPrint();
                    if (ddlApprovalStatus.SelectedValue != "Rejected")
                    {
                        Master.ShowMessage(new AppMessage("Purchase  Approval Processed ", Chai.WorkflowManagment.Enums.RMessageType.Info));
                    }
                    else
                    {
                        Master.ShowMessage(new AppMessage("Purchase  Approval Rejected ", Chai.WorkflowManagment.Enums.RMessageType.Info));
                    }
                    btnApprove.Enabled = false;
                    BindSearchPurchaseRequestGrid();
                    pnlApproval_ModalPopupExtender.Show();
                }
                //}
                //else
                //{
                //    Master.ShowMessage(new AppMessage("Please fill Bid Analysis worksheet before saving", Chai.WorkflowManagment.Enums.RMessageType.Error));
                //}
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("There is an error approving Purchase Request", Chai.WorkflowManagment.Enums.RMessageType.Error));
            }


        }
        private void BindBAAttachments()
        {
            if (_presenter.CurrentPurchaseRequest.BidAnalysises != null)
            {
                grvAttachments.DataSource = _presenter.CurrentPurchaseRequest.BidAnalysises.BAAttachments;
                grvAttachments.DataBind();
            }
        }
        protected void grvPurchaseRequestList_SelectedIndexChanged(object sender, EventArgs e)
        {

            _presenter.OnViewLoaded();
            PopApprovalStatus();
            BindPurchaseRequestStatus();
            BindBAAttachments();
            pnlApproval_ModalPopupExtender.Show();
            ChangeBidAnalysisLink();

            Session["PurchaseId"] = _presenter.CurrentPurchaseRequest.Id;

        }
        private void ReturnFromBidAnalysis()
        {
            if (PnlStatus == "Enabled")
            {
                pnlApproval_ModalPopupExtender.Show();
                _presenter.OnViewLoaded();
                BindBAAttachments();
            }
        }
        private void BindPurchaseRequestDetails()
        {
            dgPurchaseRequestDetail.DataSource = _presenter.CurrentPurchaseRequest.PurchaseRequestDetails;
            dgPurchaseRequestDetail.DataBind();
        }
        protected void grvPurchaseRequestList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {


        }
        protected void grvPurchaseRequestList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Button btnStatus = e.Row.FindControl("btnStatus") as Button;
            PurchaseRequest pr = e.Row.DataItem as PurchaseRequest;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (_presenter.GetUser(pr.CurrentApprover).EmployeePosition.PositionName == "Admin/HR Assisitance (Driver)" && pr.CurrentLevel == pr.PurchaseRequestStatuses.Count && pr.ProgressStatus == ProgressStatus.InProgress.ToString())
                {
                    btnStatus.BackColor = System.Drawing.ColorTranslator.FromHtml("#112552");
                    
                   
                }
                else if (pr.ProgressStatus == ProgressStatus.InProgress.ToString())
                {
                    btnStatus.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFF6C");

                }
                else if (pr.ProgressStatus == ProgressStatus.Completed.ToString())
                {
                    btnStatus.BackColor = System.Drawing.ColorTranslator.FromHtml("#FF7251");

                }
                //LinkButton db = (LinkButton)e.Row.Cells[5].Controls[0];
                //db.OnClientClick = "return confirm('Are you sure you want to delete this Recieve?');";
            }
            if (pr != null)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[1].Text = _presenter.GetUser(pr.Requester).FullName;
                }
            }
        }
        protected void grvPurchaseRequestList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvPurchaseRequestList.PageIndex = e.NewPageIndex;
            btnFind_Click(sender, e);
        }
        protected void btnFind_Click(object sender, EventArgs e)
        {

            BindSearchPurchaseRequestGrid();

            // pnlPopUpSearch_ModalPopupExtender.Show();
        }
        private void BindSearchPurchaseRequestGrid()
        {
            grvPurchaseRequestList.DataSource = _presenter.ListPurchaseRequests(txtRequestNosearch.Text, txtRequestDatesearch.Text, ddlProgressStatus.SelectedValue);
            grvPurchaseRequestList.DataBind();
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            _presenter.CancelPage();
        }
        protected void btnCancelPopup_Click(object sender, EventArgs e)
        {
            pnlApproval_ModalPopupExtender.Hide();
            Response.Redirect("frmPurchaseApproval.aspx");
            // pnlApproval.Visible = false;
        }
        protected void ddlApprovalStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlApprovalStatus.SelectedValue == "Rejected")
            {
                txtRejectedReason.Visible = true;

            }
            pnlApproval_ModalPopupExtender.Show();
        }
        private void ChangeBidAnalysisLink()
        {
            if (_presenter.CurrentPurchaseRequest.TotalPrice > 500)
            {
                //lnkbidanalysis.Visible = true;
                if (_presenter.CurrentUser().EmployeePosition.PositionName == "Admin/HR Assisitance (Driver)" || _presenter.CurrentUser().EmployeePosition.PositionName == "Admin Officer" || _presenter.CurrentUser().EmployeePosition.PositionName == "Operational Manager" || _presenter.CurrentUser().EmployeePosition.PositionName == "Country Director" || _presenter.CurrentUser().EmployeePosition.PositionName == "Finance Officer" || _presenter.CurrentUser().EmployeePosition.PositionName == "Finance Manager" || _presenter.GetUser(_presenter.CurrentPurchaseRequest.CurrentApprover).IsAssignedJob ==true)
                {
                    if (String.IsNullOrEmpty(_presenter.CurrentPurchaseRequest.PurchaseRequestStatuses[0].ApprovalStatus))
                    {
                        lnkbidanalysis.Visible = false;
                    }
                    else
                    {
                        lnkbidanalysis.Visible = true;
                    }

                    if (_presenter.CurrentUser().EmployeePosition.PositionName == "Admin Officer" || _presenter.CurrentUser().EmployeePosition.PositionName == "Operational Manager" || _presenter.CurrentUser().EmployeePosition.PositionName == "Country Director" || _presenter.CurrentUser().EmployeePosition.PositionName == "Finance Officer" || _presenter.CurrentUser().EmployeePosition.PositionName == "Finance Manager")
                    {
                        lnkbidanalysis.Text = "Review Bid Analysis";

                    }
                    else
                    {
                        lnkbidanalysis.Text = "Prepare Bid Analysis";
                    }
                }
                else
                {

                    lnkbidanalysis.Visible = false;
                }

                if (_presenter.CurrentPurchaseRequest.BidAnalysises == null && _presenter.CurrentUser().EmployeePosition.PositionName == "Admin/HR Assisitance (Driver)")
                {
                    pnlInfo.Visible = true;
                    btnApprove.Enabled = false;
                }

            }
            else
            {
                lnkbidanalysis.Visible = false;
            }
            if (_presenter.CurrentPurchaseRequest.CurrentLevel == _presenter.CurrentPurchaseRequest.PurchaseRequestStatuses.Count)
            {
                lnkbidanalysis.Visible = false;
            }
            if (_presenter.CurrentUser().EmployeePosition.PositionName == "Admin/HR Assisitance (Driver)")
            {
                lblApprovalStatus.Text = "Bid Status";
            }

        }
        protected void btnPrint_Click(object sender, EventArgs e)
        {

        }
        protected void dgPurchaseRequestDetail_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

        }
        protected void lnkbidanalysis_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("frmPurchaseApprovalDetail.aspx?PurchaseRequestId={0}", _presenter.CurrentPurchaseRequest.Id));
        }
        protected void grvPurchaseRequestList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Page")
            {
                reqid = (int)grvPurchaseRequestList.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                Session["ReqID"] = reqid;
                _presenter.CurrentPurchaseRequest = _presenter.GetPurchaseRequestById(reqid);
                if (e.CommandName == "ViewItem")
                {
                    reqid = Convert.ToInt32(grvPurchaseRequestList.DataKeys[0].Value);

                    pnlDetail_ModalPopupExtender.Show();
                    BindItemDetal();
                }
            }
        }
        private void BindItemDetal()
        {
            dgPurchaseRequestDetail.DataSource = _presenter.CurrentPurchaseRequest.PurchaseRequestDetails;
            dgPurchaseRequestDetail.DataBind();
        }
        protected void btnCancelPopup2_Click(object sender, EventArgs e)
        {
            pnlDetail.Visible = false;
        }
        protected void btnPurchaseOrder_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("frmPurchaseOrder.aspx?PurchaseRequestId={0}", Convert.ToInt32(Session["PurchaseId"])));
        }
        protected void DownloadFile(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            Response.ContentType = ContentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
            Response.WriteFile(filePath);
            Response.End();
        }
        
    }
}