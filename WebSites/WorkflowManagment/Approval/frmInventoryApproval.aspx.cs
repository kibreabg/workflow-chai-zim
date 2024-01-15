using Chai.WorkflowManagment.CoreDomain.Requests;
using Chai.WorkflowManagment.CoreDomain.Setting;
using Chai.WorkflowManagment.Enums;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.Shared.MailSender;
using log4net;
using log4net.Config;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Chai.WorkflowManagment.Modules.Approval.Views
{
    public partial class frmInventoryApproval : POCBasePage, IInventoryApprovalView
    {
        private InventoryApprovalPresenter _presenter;
        private static readonly ILog Log = LogManager.GetLogger("AuditTrailLog");
        private int reqID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                XmlConfigurator.Configure();
                PopProgressStatus();
                BindSearchInventoryRequestGrid();
            }
            this._presenter.OnViewLoaded();
            if (_presenter.CurrentInventoryRequest != null)
            {
                if (_presenter.CurrentInventoryRequest.Id != 0)
                {
                    PrintTransaction();
                }
            }
        }
        [CreateNew]
        public InventoryApprovalPresenter Presenter
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
                return "{e6b3a2ab-099d-45e4-92cd-365de54e9080}";
            }
        }
        #region Field Getters
        public int InventoryRequestId
        {
            get
            {
                if (grvInventoryRequestList.SelectedDataKey != null)
                {
                    return Convert.ToInt32(grvInventoryRequestList.SelectedDataKey.Value);
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion
        private void PopProgressStatus()
        {
            string[] s = Enum.GetNames(typeof(ProgressStatus));

            ddlSrchProgressStatus.Items.Clear();
            for (int i = 0; i < s.Length; i++)
            {
                ddlSrchProgressStatus.Items.Add(new ListItem(s[i].Replace('_', ' '), s[i].Replace('_', ' ')));
                ddlSrchProgressStatus.DataBind();
            }
        }
        private string GetWillStatus()
        {
            ApprovalSetting AS = _presenter.GetApprovalSetting(RequestType.Inventory_Request.ToString().Replace('_', ' ').ToString(), 0);
            string will = "";
            foreach (ApprovalLevel AL in AS.ApprovalLevels)
            {
                if (AL.EmployeePosition.PositionName == "Coordinator, Admin" && _presenter.CurrentInventoryRequest.CurrentLevel == 1)
                {
                    will = "Approve";
                    break;
                }
                else if (_presenter.GetUser(_presenter.CurrentInventoryRequest.CurrentApprover).EmployeePosition.PositionName == AL.EmployeePosition.PositionName)
                {
                    will = AL.Will;
                }

            }
            return will;
        }
        private void BindSearchInventoryRequestGrid()
        {
            grvInventoryRequestList.DataSource = _presenter.ListInventoryRequests(txtSrchRequestNo.Text, txtSrchRequestDate.Text, ddlSrchProgressStatus.SelectedValue);
            grvInventoryRequestList.DataBind();
        }
        private void BindInventoryRequestStatus()
        {
            foreach (InventoryRequestStatus irs in _presenter.CurrentInventoryRequest.InventoryRequestStatuses)
            {
                if (irs.WorkflowLevel == _presenter.CurrentInventoryRequest.CurrentLevel && _presenter.CurrentInventoryRequest.ProgressStatus != ProgressStatus.Completed.ToString())
                {
                    btnApprove.Enabled = true;
                }

                if (_presenter.CurrentInventoryRequest.CurrentLevel == _presenter.CurrentInventoryRequest.InventoryRequestStatuses.Count && !String.IsNullOrEmpty(irs.ApprovalStatus))
                {
                    btnPrint.Enabled = true;
                    btnApprove.Enabled = false;
                }
                else
                {
                    btnPrint.Enabled = false;
                    btnApprove.Enabled = true;
                }
            }
        }
        private void BindInventoryRequestDetails()
        {
            grvInventoryRequestDetails.DataSource = _presenter.CurrentInventoryRequest.InventoryRequestDetails;
            grvInventoryRequestDetails.DataBind();

        }
        private void ShowPrint()
        {
            if (_presenter.CurrentInventoryRequest.CurrentLevel == _presenter.CurrentInventoryRequest.InventoryRequestStatuses.Count)
            {
                btnPrint.Enabled = true;
            }
        }
        private void SendEmail(InventoryRequestStatus irs)
        {
            if (_presenter.GetUser(irs.Approver).IsAssignedJob != true)
            {
                EmailSender.Send(_presenter.GetUser(irs.Approver).Email, "Inventory Request", _presenter.GetUser(_presenter.CurrentInventoryRequest.Requester).FullName + " Requests for Inventory with Request No. " + (_presenter.CurrentInventoryRequest.RequestNo).ToUpper());
            }
            else
            {
                EmailSender.Send(_presenter.GetUser(_presenter.GetAssignedJobbycurrentuser(irs.Approver).AssignedTo).Email, "Inventory Request", _presenter.GetUser(_presenter.CurrentInventoryRequest.Requester).FullName + "Requests for Inventory with Request No." + (_presenter.CurrentInventoryRequest.RequestNo).ToUpper());
            }
        }
        private void SendEmailRejected(InventoryRequestStatus irs)
        {
            EmailSender.Send(_presenter.GetUser(_presenter.CurrentInventoryRequest.Requester).Email, "Inventory Request Rejection", "Your Inventory Request with Request No. - '" + (_presenter.CurrentInventoryRequest.RequestNo.ToString()).ToUpper() + " was Rejected by " + _presenter.CurrentUser().FullName + " for this reason - '" + (irs.RejectedReason).ToUpper() + "'");

            if (irs.WorkflowLevel > 1)
            {
                for (int i = 0; i + 1 < irs.WorkflowLevel; i++)
                {
                    EmailSender.Send(_presenter.GetUser(_presenter.CurrentInventoryRequest.InventoryRequestStatuses[i].Approver).Email, "Inventory Request Rejection", "Inventory Request with Request No. - '" + (_presenter.CurrentInventoryRequest.RequestNo.ToString()).ToUpper() + "' made by " + (_presenter.GetUser(_presenter.CurrentInventoryRequest.Requester).FullName).ToUpper() + " was Rejected by " + _presenter.CurrentUser().FullName + " for this reason - '" + (irs.RejectedReason).ToUpper() + "'");
                }
            }
        }
        private void GetNextApprover()
        {
            foreach (InventoryRequestStatus irs in _presenter.CurrentInventoryRequest.InventoryRequestStatuses)
            {
                if (irs.ApprovalStatus == null)
                {
                    _presenter.CurrentInventoryRequest.CurrentApprover = irs.Approver;
                    _presenter.CurrentInventoryRequest.CurrentLevel = irs.WorkflowLevel;
                    _presenter.CurrentInventoryRequest.ProgressStatus = ProgressStatus.InProgress.ToString();
                    SendEmail(irs);
                    break;
                }
            }
        }
        private void SaveInventoryRequestStatus()
        {
            foreach (InventoryRequestStatus PRRS in _presenter.CurrentInventoryRequest.InventoryRequestStatuses)
            {
                if ((PRRS.Approver == _presenter.CurrentUser().Id || _presenter.CurrentUser().Id == (_presenter.GetAssignedJobbycurrentuser(PRRS.Approver) != null ? _presenter.GetAssignedJobbycurrentuser(PRRS.Approver).AssignedTo : 0)) && PRRS.WorkflowLevel == _presenter.CurrentInventoryRequest.CurrentLevel)
                {
                    PRRS.ApprovalStatus = ddlApprovalStatus.SelectedValue;
                    PRRS.RejectedReason = txtRejectedReason.Text;
                    PRRS.ApprovalDate = Convert.ToDateTime(DateTime.Today.ToShortDateString());
                    if (PRRS.ApprovalStatus != ApprovalStatus.Rejected.ToString())
                    {
                        _presenter.CurrentInventoryRequest.ProgressStatus = ProgressStatus.Completed.ToString();
                        GetNextApprover();
                        PRRS.Approver = _presenter.CurrentUser().Id;
                        Log.Info(_presenter.GetUser(PRRS.Approver).FullName + " has " + PRRS.ApprovalStatus + " Inventory Request made by " + _presenter.GetUser(_presenter.CurrentInventoryRequest.Requester).FullName);
                    }
                    else
                    {
                        _presenter.CurrentInventoryRequest.ProgressStatus = ProgressStatus.Completed.ToString();
                        PRRS.Approver = _presenter.CurrentUser().Id;
                        SendEmailRejected(PRRS);
                        Log.Info(_presenter.GetUser(PRRS.Approver).FullName + " has " + PRRS.ApprovalStatus + " Inventory Request made by " + _presenter.GetUser(_presenter.CurrentInventoryRequest.Requester).FullName);
                    }
                    break;
                }

            }
        }
        protected void btnFind_Click(object sender, EventArgs e)
        {
            BindSearchInventoryRequestGrid();
        }
        private void PrintTransaction()
        {
            lblRequestNoResult.Text = _presenter.CurrentInventoryRequest.RequestNo.ToString();
            lblRequestedDateResult.Text = _presenter.CurrentInventoryRequest.RequestedDate.ToShortDateString();
            lblRequesterResult.Text = _presenter.GetUser(_presenter.CurrentInventoryRequest.Requester).FullName;
            lblSpecialNeedResult.Text = _presenter.CurrentInventoryRequest.SpecialNeed;
            lblDelivertoResult.Text = _presenter.CurrentInventoryRequest.DeliverTo;
            grvDetails.DataSource = _presenter.CurrentInventoryRequest.InventoryRequestDetails;
            grvDetails.DataBind();

            grvStatuses.DataSource = _presenter.CurrentInventoryRequest.InventoryRequestStatuses;
            grvStatuses.DataBind();


        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Default.aspx");
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                if (_presenter.CurrentInventoryRequest.ProgressStatus != ProgressStatus.Completed.ToString())
                {
                    SaveInventoryRequestStatus();
                    _presenter.SaveOrUpdateInventoryRequest(_presenter.CurrentInventoryRequest);
                    ShowPrint();
                    Master.ShowMessage(new AppMessage("Inventory Approval Processed", RMessageType.Info));
                    btnApprove.Enabled = false;
                    BindSearchInventoryRequestGrid();
                    ScriptManager.RegisterStartupScript(this, GetType(), "showApprovalModal", "showApprovalModal();", true);
                }
            }
            catch (Exception ex)
            {
                Master.ShowMessage(new AppMessage("Error: While Approving Inventory Request!", RMessageType.Error));
                ExceptionUtility.LogException(ex, ex.Source);
                ExceptionUtility.NotifySystemOps(ex, _presenter.CurrentUser().FullName);
            }
        }
        protected void grvInventoryRequestList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewItem")
            {
                reqID = (int)grvInventoryRequestList.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                _presenter.CurrentInventoryRequest = _presenter.GetInventoryRequestById(reqID);
                BindInventoryRequestDetails();
                ScriptManager.RegisterStartupScript(this, GetType(), "showDetailModal", "showDetailModal();", true);
            }
        }
        protected void grvInventoryRequestList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.OnViewLoaded();
            PopApprovalStatus();
            PrintTransaction();
            BindInventoryRequestStatus();
            txtRejectedReason.Visible = false;
            rfvRejectedReason.Enabled = false;
            ScriptManager.RegisterStartupScript(this, GetType(), "showApprovalModal", "showApprovalModal();", true);
        }
        private void PopApprovalStatus()
        {
            ddlApprovalStatus.Items.Clear();
            ddlApprovalStatus.Items.Add(new ListItem("Select Status", "0"));
            string[] s = Enum.GetNames(typeof(ApprovalStatus));

            for (int i = 0; i < s.Length; i++)
            {
                if (GetWillStatus().Substring(0, 2) == s[i].Substring(0, 2))
                {
                    if (s[i] != ApprovalStatus.Rejected.ToString())
                        ddlApprovalStatus.Items.Add(new ListItem(s[i].Replace('_', ' '), s[i].Replace('_', ' ')));
                }

            }
            ddlApprovalStatus.Items.Add(new ListItem(ApprovalStatus.Rejected.ToString().Replace('_', ' '), ApprovalStatus.Rejected.ToString().Replace('_', ' ')));

        }
        protected void grvInventoryRequestList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            InventoryRequest CSR = e.Row.DataItem as InventoryRequest;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].Text = _presenter.GetUser(CSR.Requester).FullName;
            }
        }
        protected void ddlApprovalStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlApprovalStatus.SelectedValue == "Rejected")
            {
                lblRejectedReason.Visible = true;
                txtRejectedReason.Visible = true;
                rfvRejectedReason.Enabled = true;
            }
            else
            {
                lblRejectedReason.Visible = false;
                txtRejectedReason.Visible = false;
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "showApprovalModal", "showApprovalModal();", true);
        }
        protected void grvInventoryRequestList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvInventoryRequestList.PageIndex = e.NewPageIndex;
            btnFind_Click(sender, e);
        }
        protected void grvStatuses_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (_presenter.CurrentInventoryRequest.InventoryRequestStatuses != null && e.Row.RowType == DataControlRowType.DataRow && _presenter.CurrentInventoryRequest.InventoryRequestStatuses[e.Row.RowIndex].Approver != 0)
                e.Row.Cells[1].Text = _presenter.GetUser(_presenter.CurrentInventoryRequest.InventoryRequestStatuses[e.Row.RowIndex].Approver).FullName;
        }
    }
}