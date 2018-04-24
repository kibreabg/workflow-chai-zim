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
using log4net;
using System.Reflection;
using log4net.Config;
using Chai.WorkflowManagment.CoreDomain.Requests;

namespace Chai.WorkflowManagment.Modules.Approval.Views
{
    public partial class frmSoleVendorApproval : POCBasePage, ISoleVendorApprovalView
    {
        private SoleVendorApprovalPresenter _presenter;
        private static readonly ILog Log = LogManager.GetLogger("AuditTrailLog");
        private SoleVendorRequest _solevendorrequest;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                XmlConfigurator.Configure();
                PopProgressStatus();
            }

            //BindJS();
            this._presenter.OnViewLoaded();


            BindSearchSoleVendorRequestGrid();
            if (_presenter.CurrentSoleVendorRequest.Id != 0)
            {
                BindSoleVendorRequestforprint();
            }
            //btnPrint.Attributes.Add("onclick", "javascript:Clickheretoprint('divprint'); return false;");

        }
        //protected void BindJS()
        //{
        //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "workflowscripts", String.Format("<script language=\"JavaScript\" src=\"http://localhost/WorkflowManagment/WorkflowManagment.js\"></script>\n"));
        //}
        [CreateNew]
        public SoleVendorApprovalPresenter Presenter
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
                return "{282224A8-DCCA-4FED-AAB1-BEB6A5AA0653}";
            }
        }
        public CoreDomain.Requests.SoleVendorRequest SoleVendorRequest
        {
            get
            {
                return _solevendorrequest;
            }
            set
            {
                _solevendorrequest = value;
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
        public int SoleVendorRequestId
        {
            get
            {
                if (grvLeaveRequestList.SelectedDataKey != null)
                {
                    return Convert.ToInt32(grvLeaveRequestList.SelectedDataKey.Value);
                }
                else
                {
                    return 0;
                }
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
            ApprovalSetting AS = _presenter.GetApprovalSettingforProcess(RequestType.SoleVendor_Request.ToString().Replace('_', ' ').ToString(), 0);
            string will = "";
            foreach (ApprovalLevel AL in AS.ApprovalLevels)
            {
                if ((AL.EmployeePosition.PositionName == "Superviser/Line Manager" || AL.EmployeePosition.PositionName == "Program Manager") && _presenter.CurrentSoleVendorRequest.CurrentLevel == 1)
                {
                    will = "Approve";
                    break;

                }
                else if (_presenter.GetUser(_presenter.CurrentSoleVendorRequest.CurrentApprover).EmployeePosition.PositionName == AL.EmployeePosition.PositionName)
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
        private void BindSoleVendorRequestStatus()
        {
            foreach (SoleVendorRequestStatus SVRS in _presenter.CurrentSoleVendorRequest.SoleVendorRequestStatuses)
            {
                if (SVRS.WorkflowLevel == _presenter.CurrentSoleVendorRequest.CurrentLevel && _presenter.CurrentSoleVendorRequest.ProgressStatus != ProgressStatus.Completed.ToString())
                {
                    ddlApprovalStatus.SelectedValue = SVRS.ApprovalStatus;
                    txtRejectedReason.Text = SVRS.RejectedReason;
                    btnApprove.Enabled = true;

                }

            }
        }
        private void ShowPrint()
        {
            if (_presenter.CurrentSoleVendorRequest.CurrentLevel == _presenter.CurrentSoleVendorRequest.SoleVendorRequestStatuses.Count && _presenter.CurrentSoleVendorRequest.ProgressStatus == ProgressStatus.Completed.ToString())
            {
                btnPrint.Enabled = true;
                btnPurchaseOrder.Enabled = true;
                SendEmailToRequester();

            }

        }
        private void SendEmail(SoleVendorRequestStatus SVRS)
        {
            if (_presenter.GetUser(SVRS.Approver).IsAssignedJob != true)
            {
                EmailSender.Send(_presenter.GetUser(SVRS.Approver).Email, "Sole Vendor Request", (_presenter.GetUser(_presenter.CurrentSoleVendorRequest.AppUser.Id).FullName).ToUpper() + " Requests for sole Vendor with Sole Vendor Request No. - '" + (_presenter.CurrentSoleVendorRequest.RequestNo).ToUpper() + "'");
            }
            else
            {
                EmailSender.Send(_presenter.GetUser(_presenter.GetAssignedJobbycurrentuser(SVRS.Approver).AssignedTo).Email, "Sole Vendor Request", (_presenter.GetUser(_presenter.CurrentSoleVendorRequest.AppUser.Id).FullName).ToUpper() + " Requests for Leave with Leave Request No. - '" + (_presenter.CurrentSoleVendorRequest.RequestNo).ToUpper() + "'");
            }




        }
        private void SendEmailRejected(SoleVendorRequestStatus SVRS)
        {
            EmailSender.Send(_presenter.GetUser(_presenter.CurrentSoleVendorRequest.AppUser.Id).Email, "Sole Vendor Request Rejection", "Your Sole Vendor Request with Sole Vendor Request No. " + (_presenter.CurrentSoleVendorRequest.RequestNo).ToUpper() + " was Rejected for this reason - '" + (SVRS.RejectedReason).ToUpper() + "'");

            if (SVRS.WorkflowLevel > 1)
            {
                for (int i = 0; i + 1 < SVRS.WorkflowLevel; i++)
                {
                    EmailSender.Send(_presenter.GetUser(_presenter.CurrentSoleVendorRequest.SoleVendorRequestStatuses[i].Approver).Email, "Sole Vendor Request Rejection", "Leave Request with Leave Request No. - " + (_presenter.CurrentSoleVendorRequest.RequestNo).ToUpper() + " made by " + (_presenter.GetUser(_presenter.CurrentSoleVendorRequest.AppUser.Id).FullName).ToUpper() + " was Rejected for this reason - '" + (SVRS.RejectedReason).ToUpper() + "'");
                }
            }
        }
        private void SendEmailToRequester()
        {
            EmailSender.Send(_presenter.GetUser(_presenter.CurrentSoleVendorRequest.AppUser.Id).Email, "Sole Vendor Request ", "Your Sole Vendor Request with Sole Vendor Request No. - '" + (_presenter.CurrentSoleVendorRequest.RequestNo).ToUpper() + "' was Completed.");
        }
        private void GetNextApprover()
        {
            foreach (SoleVendorRequestStatus SVRS in _presenter.CurrentSoleVendorRequest.SoleVendorRequestStatuses)
            {
                if (SVRS.ApprovalStatus == null)
                {
                    SendEmail(SVRS);
                    _presenter.CurrentSoleVendorRequest.CurrentApprover = SVRS.Approver;
                    _presenter.CurrentSoleVendorRequest.CurrentLevel = SVRS.WorkflowLevel;
                    _presenter.CurrentSoleVendorRequest.ProgressStatus = ProgressStatus.InProgress.ToString();
                    _presenter.CurrentSoleVendorRequest.CurrentStatus = SVRS.ApprovalStatus;
                    break;

                }
            }
        }
        private void SaveSoleVendorRequestStatus()
        {
            foreach (SoleVendorRequestStatus SVRS in _presenter.CurrentSoleVendorRequest.SoleVendorRequestStatuses)
            {
                if ((SVRS.Approver == _presenter.CurrentUser().Id || _presenter.CurrentUser().Id == (_presenter.GetAssignedJobbycurrentuser(SVRS.Approver) != null ? _presenter.GetAssignedJobbycurrentuser(SVRS.Approver).AssignedTo : 0)) && SVRS.WorkflowLevel == _presenter.CurrentSoleVendorRequest.CurrentLevel)
                {
                    SVRS.ApprovalStatus = ddlApprovalStatus.SelectedValue;
                    SVRS.RejectedReason = txtRejectedReason.Text;
                    SVRS.ApprovalDate = DateTime.Today.Date;
                    SVRS.AssignedBy = _presenter.GetAssignedJobbycurrentuser(SVRS.Approver) != null ? _presenter.GetAssignedJobbycurrentuser(SVRS.Approver).AppUser.FullName : "";
                    if (SVRS.ApprovalStatus != ApprovalStatus.Rejected.ToString())
                    {

                        if (_presenter.CurrentSoleVendorRequest.CurrentLevel == _presenter.CurrentSoleVendorRequest.SoleVendorRequestStatuses.Count)
                        {
                            _presenter.CurrentSoleVendorRequest.CurrentApprover = SVRS.Approver;
                            _presenter.CurrentSoleVendorRequest.CurrentLevel = SVRS.WorkflowLevel;
                            _presenter.CurrentSoleVendorRequest.CurrentStatus = SVRS.ApprovalStatus;
                            _presenter.CurrentSoleVendorRequest.ProgressStatus = ProgressStatus.Completed.ToString();
                        }
                        GetNextApprover();
                        SVRS.Approver = _presenter.CurrentUser().Id;
                        Log.Info(_presenter.GetUser(SVRS.Approver).FullName + " has " + SVRS.ApprovalStatus + " Sole Vendor Request made by " + _presenter.GetUser(_presenter.CurrentSoleVendorRequest.AppUser.Id).FullName);
                    }
                    else
                    {
                        _presenter.CurrentSoleVendorRequest.CurrentApprover = SVRS.Approver;
                        _presenter.CurrentSoleVendorRequest.CurrentLevel = SVRS.WorkflowLevel;
                        _presenter.CurrentSoleVendorRequest.CurrentStatus = SVRS.ApprovalStatus;
                        _presenter.CurrentSoleVendorRequest.ProgressStatus = ProgressStatus.Completed.ToString();
                        SVRS.Approver = _presenter.CurrentUser().Id;
                        SendEmailRejected(SVRS);
                        Log.Info(_presenter.GetUser(SVRS.Approver).FullName + " has " + SVRS.ApprovalStatus + " Sole Vendor Request made by " + _presenter.GetUser(_presenter.CurrentSoleVendorRequest.AppUser.Id).FullName);
                    }
                    break;
                }

            }

        }
        private void EnableControls()
        {
            if (_presenter.CurrentSoleVendorRequest.CurrentLevel == _presenter.CurrentSoleVendorRequest.SoleVendorRequestStatuses.Count)
            {
                btnPurchaseOrder.Enabled = true;
                // SendEmailToRequester();
                btnPrint.Enabled = true;
            }
        } 
        protected void btnApprove_Click(object sender, EventArgs e)
        {



            try
            {
               
                    if (_presenter.CurrentSoleVendorRequest.ProgressStatus != ProgressStatus.Completed.ToString())
                    {
                        SaveSoleVendorRequestStatus();
                        Session["PurchaseId"] = _presenter.CurrentSoleVendorRequest.Id;
                        _presenter.SaveOrUpdateSoleVendorRequest(_presenter.CurrentSoleVendorRequest);
                        ShowPrint();
                        EnableControls();
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
                

               
             BindSoleVendorRequestforprint();
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("There is an error approving Purchase Request", Chai.WorkflowManagment.Enums.RMessageType.Error));
            }



            


        }
        private void BindSearchPurchaseRequestGrid()
        {
            grvLeaveRequestList.DataSource = _presenter.ListSoleVendorRequests(txtRequestNosearch.Text, txtRequestDatesearch.Text, ddlProgressStatus.SelectedValue);
            grvLeaveRequestList.DataBind();
        }
        protected void grvLeaveRequestList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Session["ApprovalLevel"] = true;

            //  Convert.ToInt32(grvLeaveRequestList.SelectedDataKey.Value);
            //pnlApproval.Visible = true;

            _presenter.OnViewLoaded();
            PopApprovalStatus();
            BindSoleVendorRequestStatus();
            
            //lblProjectIDDResult.Text = _presenter.CurrentVehicleRequest.Project.ProjectCode;
           // lblGrantIDResult.Text = _presenter.CurrentVehicleRequest.Grant.GrantCode;
            if (_presenter.CurrentSoleVendorRequest.ProgressStatus == ProgressStatus.Completed.ToString())
            {
                btnApprove.Enabled = false;
                //PrintTransaction();
            }

            pnlApproval_ModalPopupExtender.Show();

        }
       
        protected void grvLeaveRequestList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {


        }
        protected void grvLeaveRequestList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
           /* Button btnStatus = e.Row.FindControl("btnStatus") as Button;
            LeaveRequest LR = e.Row.DataItem as LeaveRequest;
            if (LR != null)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        e.Row.Cells[1].Text = _presenter.GetUser(LR.Requester).FullName;
                    }
                }
                if (LR.ProgressStatus == ProgressStatus.InProgress.ToString())
                {
                    btnStatus.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFF6C");

                }
                else if (LR.ProgressStatus == ProgressStatus.Completed.ToString())
                {
                    btnStatus.BackColor = System.Drawing.ColorTranslator.FromHtml("#FF7251");

                }
            }*/

        }
        protected void grvLeaveRequestList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvLeaveRequestList.PageIndex = e.NewPageIndex;
            btnFind_Click(sender, e);
        }
        protected void btnFind_Click(object sender, EventArgs e)
        {

            BindSearchSoleVendorRequestGrid();

            // pnlPopUpSearch_ModalPopupExtender.Show();
        }
        private void BindSearchSoleVendorRequestGrid()
        {
            grvLeaveRequestList.DataSource = _presenter.ListSoleVendorRequests(txtRequestNosearch.Text, txtRequestDatesearch.Text, ddlProgressStatus.SelectedValue);
            grvLeaveRequestList.DataBind();
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Default.aspx");
        }
        protected void btnCancelPopup_Click(object sender, EventArgs e)
        {
            pnlApproval.Visible = false;
            pnlApproval_ModalPopupExtender.Hide();
        }
        protected void ddlApprovalStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlApprovalStatus.SelectedValue == "Rejected")
            {
                lblRejectedReason.Visible = true;
                txtRejectedReason.Visible = true;

            }
            pnlApproval_ModalPopupExtender.Show();
        }
        private void BindSoleVendorRequestforprint()
        {
            if (_presenter.CurrentSoleVendorRequest.Id > 0)
            {
                lblRequestNoresult.Text = _presenter.CurrentSoleVendorRequest.RequestNo;
                lblRequestedDateresult.Text = _presenter.CurrentSoleVendorRequest.RequestDate.ToString();
                lblCommodityServicepurchasedbyres.Text = _presenter.CurrentSoleVendorRequest.CommodityServicePurchasedby;
                lblProposedPurchasedpriceres.Text = _presenter.CurrentSoleVendorRequest.ProposedPurchasedPrice.ToString();
                lblProposedSupplierresp.Text = _presenter.CurrentSoleVendorRequest.Supplier.SupplierName;
              
                lblSoleSourceJustificationPreparedByresp.Text = _presenter.CurrentSoleVendorRequest.SoleSourceJustificationPreparedBy;
                lblSoleVendorJustificationTyperes.Text = _presenter.CurrentSoleVendorRequest.SoleVendorJustificationType;
                lblapprovalstatusres.Text = _presenter.CurrentSoleVendorRequest.CurrentStatus;
                lblRequesterres.Text = _presenter.GetUser(_presenter.CurrentSoleVendorRequest.AppUser.Id).FullName;
               

                grvStatuses.DataSource = _presenter.CurrentSoleVendorRequest.SoleVendorRequestStatuses;
                grvStatuses.DataBind();
            }
        }
        protected void grvStatuses_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (_presenter.CurrentSoleVendorRequest.SoleVendorRequestStatuses != null)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (_presenter.GetUser(_presenter.CurrentSoleVendorRequest.SoleVendorRequestStatuses[e.Row.RowIndex].Approver) != null)
                    {
                        e.Row.Cells[1].Text = _presenter.GetUser(_presenter.CurrentSoleVendorRequest.SoleVendorRequestStatuses[e.Row.RowIndex].Approver).FullName;
                    }
                }
            }
        }
        protected void btnPurchaseOrder_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("frmPurchaseOrderSoleVendor.aspx?SoleVendorRequestId={0}", Convert.ToInt32(Session["PurchaseId"])));
        }
}
}